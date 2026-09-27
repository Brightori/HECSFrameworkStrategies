using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Strategies;
using UnityEditor;
using UnityEngine;

public static class StrategyConnectionsReport
{
    private const string MenuRoot = "HECS Options/Helpers/Strategies/Connections Snapshot/";
    private const string LogPrefix = "[StrategyConnections] ";
    private const string Separator = "\t=>\t";

    private static readonly string Folder = Path.Combine("Library", "HECS");
    private static readonly string SnapshotPath = Path.Combine(Folder, "StrategyConnections.tsv");
    private static readonly string DiffPath = Path.Combine(Folder, "StrategyConnections.diff.tsv");

    [MenuItem(MenuRoot + "Take")]
    public static void Take()
    {
        var report = Build();
        Directory.CreateDirectory(Folder);
        File.WriteAllLines(SnapshotPath, report.ToLines());
        Debug.Log(LogPrefix + $"snapshot saved to {SnapshotPath}. {report.Summary}");
        report.LogWarnings();
    }

    [MenuItem(MenuRoot + "Compare")]
    public static void Compare()
    {
        if (!File.Exists(SnapshotPath))
        {
            Debug.LogWarning(LogPrefix + $"no snapshot at {SnapshotPath}, take one first");
            return;
        }

        var before = Parse(File.ReadAllLines(SnapshotPath));
        var report = Build();
        var after = Parse(report.ToLines());

        var diff = new List<string>();
        var counts = new SortedDictionary<string, int[]>(StringComparer.Ordinal);

        foreach (var record in before)
        {
            var bucket = Bucket(counts, record.Key);

            if (!after.TryGetValue(record.Key, out var now))
            {
                bucket[2]++;
                diff.Add($"REMOVED\t{record.Key}\t{record.Value}");
            }
            else if (now != record.Value)
            {
                bucket[1]++;
                diff.Add($"CHANGED\t{record.Key}\t{record.Value}\t->\t{now}");
            }
            else
                bucket[0]++;
        }

        foreach (var record in after)
        {
            if (before.ContainsKey(record.Key))
                continue;

            Bucket(counts, record.Key)[3]++;
            diff.Add($"ADDED\t{record.Key}\t{record.Value}");
        }

        Directory.CreateDirectory(Folder);
        File.WriteAllLines(DiffPath, diff);

        var perKind = string.Join("; ", counts.Select(x => $"{x.Key}: same {x.Value[0]}, changed {x.Value[1]}, removed {x.Value[2]}, added {x.Value[3]}"));

        if (diff.Count == 0)
            Debug.Log(LogPrefix + $"no differences. {perKind}");
        else
            Debug.LogWarning(LogPrefix + $"{diff.Count} differences, see {DiffPath}. {perKind}");

        report.LogWarnings();
    }

    private static int[] Bucket(SortedDictionary<string, int[]> counts, string key)
    {
        var kind = key.Substring(0, key.IndexOf('\t'));

        if (!counts.TryGetValue(kind, out var bucket))
            counts[kind] = bucket = new int[4];

        return bucket;
    }

    private static Dictionary<string, string> Parse(IEnumerable<string> lines)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var line in lines)
        {
            if (line.Length == 0 || line[0] == '#')
                continue;

            var index = line.IndexOf(Separator, StringComparison.Ordinal);

            if (index < 0)
                continue;

            result[line.Substring(0, index)] = line.Substring(index + Separator.Length);
        }

        return result;
    }

    private static Report Build()
    {
        var report = new Report();
        var paths = new SortedSet<string>(StringComparer.Ordinal);

        foreach (var type in TypeCache.GetTypesDerivedFrom<BaseStrategy>())
        {
            if (type.IsAbstract)
                continue;

            foreach (var guid in AssetDatabase.FindAssets("t:" + type.Name, new[] { "Assets" }))
                paths.Add(AssetDatabase.GUIDToAssetPath(guid));
        }

        foreach (var path in paths)
        {
            if (AssetDatabase.LoadMainAssetAtPath(path) is BaseStrategy strategy)
                Collect(report, path, strategy);
        }

        return report;
    }

    private static void Collect(Report report, string path, BaseStrategy strategy)
    {
        report.Strategies++;

        var nullEntries = strategy.nodes.Count(x => x == null);

        if (nullEntries > 0)
            report.Add($"NULLNODES\t{path}", nullEntries.ToString(), ref report.NullNodes);

        var listed = new HashSet<BaseDecisionNode>(strategy.nodes.Where(x => x != null));
        var metaChildren = new HashSet<BaseDecisionNode>(strategy.Metanodes.Where(x => x.Child != null).Select(x => x.Child));

        foreach (var node in AssetDatabase.LoadAllAssetsAtPath(path).OfType<BaseDecisionNode>())
        {
            report.Nodes++;

            if (!listed.Contains(node) && !metaChildren.Contains(node))
                report.Add($"ORPHAN\t{path}\t{LocalId(node)}", node.GetType().Name, ref report.Orphans);

            foreach (var port in Ports(node.GetType()))
            {
                var value = $"{node.GetType().Name}\t{port.Direction}\t{(listed.Contains(node) ? "listed" : "unlisted")}\t{TargetOf(port.Member, node, path)}";
                report.Add($"FIELD\t{path}\t{LocalId(node)}\t{port.Member.Name}", value, ref report.Fields);
            }
        }

        CollectEdges(report, path, strategy);
    }

    // Edges come from the same StrategyPorts.RestoreEdges the graph editor draws from.
    private static void CollectEdges(Report report, string path, BaseStrategy strategy)
    {
        foreach (var edge in StrategyPorts.RestoreEdges(strategy))
        {
            var key = $"EDGE\t{path}\t{LocalId(edge.Consumer)}\t{edge.In.Member.Name}";
            var from = edge.IsDrawn ? $"{Id(edge.Provider, path)}.{edge.Out.Member.Name}" : Id(edge.Source, path);

            if (!report.Add(key, $"{edge.Status}\t{from}", ref report.Edges))
                continue;

            if (edge.IsDrawn)
                report.DrawnEdges++;
            else
                report.HiddenEdges++;
        }
    }

    private static IEnumerable<(MemberInfo Member, ConnectionPointType Direction)> Ports(Type type)
    {
        foreach (var member in type.GetMembers())
        {
            foreach (var attribute in member.GetCustomAttributes())
            {
                if (attribute is ConnectionAttribute connection)
                    yield return (member, connection.ConnectionPointType);
            }
        }
    }

    private static string TargetOf(MemberInfo member, BaseDecisionNode node, string path)
    {
        var value = member is FieldInfo field ? field.GetValue(node) : (member as PropertyInfo)?.GetValue(node);

        if (value is IInSlot slot)
            return slot.Node == null ? "null" : $"{Id(slot.Node, path)}:out{slot.Output}";

        return Id(value as BaseDecisionNode, path);
    }

    private static long LocalId(UnityEngine.Object obj)
    {
        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out _, out long localId);
        return localId;
    }

    private static string Id(UnityEngine.Object obj, string ownerPath)
    {
        if (obj == null)
            return "null";

        var id = $"{obj.GetType().Name}#{LocalId(obj)}";
        var objPath = AssetDatabase.GetAssetPath(obj);
        return objPath == ownerPath ? id : $"{id}@{objPath}";
    }

    private sealed class Report
    {
        public int Strategies;
        public int Nodes;
        public int Fields;
        public int Edges;
        public int DrawnEdges;
        public int HiddenEdges;
        public int Orphans;
        public int NullNodes;

        private readonly SortedDictionary<string, string> records = new SortedDictionary<string, string>(StringComparer.Ordinal);

        public string Summary =>
            $"strategies {Strategies}, nodes {Nodes}, connection fields {Fields}, edges {Edges} (drawn {DrawnEdges}, hidden {HiddenEdges}), orphan nodes {Orphans}, strategies with null node entries {NullNodes}";

        public bool Add(string key, string value, ref int counter)
        {
            if (records.ContainsKey(key))
                return false;

            records.Add(key, value);
            counter++;
            return true;
        }

        public IEnumerable<string> ToLines()
        {
            yield return $"# {DateTime.Now:yyyy-MM-dd HH:mm:ss} Unity {Application.unityVersion}. {Summary}";

            foreach (var record in records)
                yield return record.Key + Separator + record.Value;
        }

        public void LogWarnings()
        {
            if (HiddenEdges > 0 || Orphans > 0 || NullNodes > 0)
                Debug.LogWarning(LogPrefix + $"hidden edges {HiddenEdges}, orphan nodes {Orphans}, strategies with null node entries {NullNodes}. Search the snapshot for 'hidden:', ORPHAN, NULLNODES");
        }
    }
}
