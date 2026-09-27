using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HECSFramework.Core;
using HECSFramework.Core.Helpers;
using Sirenix.Utilities;
using Strategies;
using UnityEditor;
using UnityEngine;

public enum PortSlot
{
    FlowIn,
    NodeRefIn,
    LegacyValueIn,
    InSlot,
    FlowOut,
    ValueOutField,
    MetaOut,
    MethodOut,
}

public sealed class PortInfo
{
    public MemberInfo Member;
    public ConnectionPointType Direction;
    public PortSlot Slot;
    public Type ValueType;
    public int OutputIndex;
    public string OutputName;
    public string Label;

    public bool IsValue => Slot is PortSlot.LegacyValueIn or PortSlot.InSlot or PortSlot.ValueOutField or PortSlot.MetaOut or PortSlot.MethodOut;
    public bool IsFieldBacked => Member is FieldInfo;
    public FieldInfo Field => Member as FieldInfo;
}

public readonly struct RestoredEdge
{
    public readonly BaseDecisionNode Consumer;
    public readonly PortInfo In;
    public readonly BaseDecisionNode Source;
    public readonly BaseDecisionNode Provider;
    public readonly PortInfo Out;
    public readonly string Status;

    public RestoredEdge(BaseDecisionNode consumer, PortInfo input, BaseDecisionNode source, BaseDecisionNode provider, PortInfo output, string status)
    {
        Consumer = consumer;
        In = input;
        Source = source;
        Provider = provider;
        Out = output;
        Status = status;
    }

    public bool IsDrawn => Provider != null && Out != null;
}

// Port model shared by the strategy graph editor and StrategyConnectionsReport, so the report checks
// exactly what the graph draws. Ports of old nodes (field-backed, no In<T>, not on a GenericNode with
// two or more type arguments) keep the pre-In<T> connection rules and edge restoration verbatim:
// old strategies must behave in the editor exactly as before.
public static class StrategyPorts
{
    private static readonly Type[] GenericNodeDefinitions =
    {
        typeof(GenericNode<>), typeof(GenericNode<,>), typeof(GenericNode<,,>),
        typeof(GenericNode<,,,>), typeof(GenericNode<,,,,>), typeof(GenericNode<,,,,,>),
    };

    private static readonly Dictionary<Type, List<PortInfo>> cache = new Dictionary<Type, List<PortInfo>>();

    private enum LegacyDecision { Reject, ConnectBoth, ConnectMeta, SkipKeepEdge }

    public static IReadOnlyList<PortInfo> GetPorts(Type nodeType)
    {
        if (!cache.TryGetValue(nodeType, out var ports))
            cache[nodeType] = ports = BuildPorts(nodeType);

        return ports;
    }

    public static Type[] ValueTypesOf(Type type)
    {
        for (var current = type; current != null; current = current.BaseType)
        {
            if (current.IsGenericType && Array.IndexOf(GenericNodeDefinitions, current.GetGenericTypeDefinition()) >= 0)
                return current.GetGenericArguments();
        }

        return null;
    }

    public static bool IsMultiOutput(Type nodeType) => ValueTypesOf(nodeType)?.Length >= 2;

    private static List<PortInfo> BuildPorts(Type nodeType)
    {
        var ports = new List<PortInfo>();
        var nodeValueTypes = ValueTypesOf(nodeType);

        foreach (var member in nodeType.GetMembers())
        {
            if (member is not FieldInfo field)
                continue;

            foreach (var attribute in member.GetCustomAttributes())
            {
                if (attribute is ConnectionAttribute connection)
                    ports.Add(FieldPort(field, connection, nodeValueTypes));
            }
        }

        if (nodeValueTypes != null && nodeValueTypes.Length >= 2)
        {
            var hasFieldMainOutput = ports.Any(x => x.Slot == PortSlot.ValueOutField);

            for (int index = hasFieldMainOutput ? 2 : 1; index <= nodeValueTypes.Length; index++)
            {
                var method = nodeType.GetMethod(index == 1 ? "Value" : "Value" + index, new[] { typeof(Entity) });

                if (method == null)
                    continue;

                var name = method.GetCustomAttribute<OutputAttribute>(true)?.Name ?? method.Name;
                var valueType = nodeValueTypes[index - 1];

                ports.Add(new PortInfo
                {
                    Member = method,
                    Direction = ConnectionPointType.Out,
                    Slot = PortSlot.MethodOut,
                    ValueType = valueType,
                    OutputIndex = index,
                    OutputName = name,
                    Label = $"<{FriendlyName(valueType)}> {name}",
                });
            }
        }

        return ports;
    }

    private static PortInfo FieldPort(FieldInfo field, ConnectionAttribute connection, Type[] nodeValueTypes)
    {
        var port = new PortInfo
        {
            Member = field,
            Direction = connection.ConnectionPointType,
            Label = connection.NameOfField,
            OutputName = field.Name,
        };

        var fieldType = field.FieldType;

        if (connection.ConnectionPointType == ConnectionPointType.In)
        {
            if (connection.Kind == PortKind.Flow)
                port.Slot = PortSlot.FlowIn;
            else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(In<>))
            {
                port.Slot = PortSlot.InSlot;
                port.ValueType = fieldType.GetGenericArguments()[0];

                if (!port.Label.Contains("<"))
                    port.Label = $"<{FriendlyName(port.ValueType)}> {port.Label}";
            }
            else if (ValueTypesOf(fieldType) is { } slotTypes)
            {
                port.Slot = PortSlot.LegacyValueIn;
                port.ValueType = slotTypes[0];
            }
            else if (fieldType == typeof(BaseDecisionNode) || !typeof(BaseDecisionNode).IsAssignableFrom(fieldType))
                port.Slot = PortSlot.FlowIn;
            else
            {
                port.Slot = PortSlot.NodeRefIn;
                port.ValueType = fieldType;
            }

            return port;
        }

        if (connection.Kind == PortKind.Flow)
            port.Slot = PortSlot.FlowOut;
        else if (field.GetCustomAttribute<MetaNodeAttribute>(true) != null)
        {
            var metaTypes = ValueTypesOf(fieldType);
            port.Slot = metaTypes != null ? PortSlot.MetaOut : PortSlot.FlowOut;
            port.ValueType = metaTypes?[0];
            port.OutputIndex = metaTypes != null ? 1 : 0;
        }
        else if (nodeValueTypes != null && (connection.Kind == PortKind.Value || fieldType == typeof(BaseDecisionNode)))
        {
            port.Slot = PortSlot.ValueOutField;
            port.ValueType = nodeValueTypes[0];
            port.OutputIndex = 1;
        }
        else
            port.Slot = PortSlot.FlowOut;

        return port;
    }

    public static bool IsNewPort(PortInfo port, BaseDecisionNode node)
        => port.Slot is PortSlot.InSlot or PortSlot.MethodOut || IsMultiOutput(node.GetType());

    public static BaseDecisionNode LinkedNode(PortInfo port, BaseDecisionNode node)
    {
        if (port.Field == null)
            return null;

        var value = port.Field.GetValue(node);
        return value is IInSlot slot ? slot.Node : value as BaseDecisionNode;
    }

    public static bool CanConnect(PortInfo output, BaseDecisionNode outputNode, PortInfo input, BaseDecisionNode inputNode)
    {
        if (output.Direction != ConnectionPointType.Out || input.Direction != ConnectionPointType.In || outputNode == inputNode)
            return false;

        if (!IsNewPort(output, outputNode) && !IsNewPort(input, inputNode))
        {
            var decision = DecideLegacy(output, outputNode, input);
            return decision == LegacyDecision.ConnectBoth || decision == LegacyDecision.ConnectMeta;
        }

        switch (input.Slot)
        {
            case PortSlot.InSlot:
                return output.IsValue && output.ValueType != null && IsReadableAs(output.ValueType, input.ValueType);

            case PortSlot.LegacyValueIn:
                var provider = ProviderOf(output, outputNode, out var index);
                return provider != null && index == 1 && input.Field.FieldType.IsInstanceOfType(provider);

            case PortSlot.NodeRefIn:
                return output.Slot == PortSlot.FlowOut && input.ValueType.IsInstanceOfType(outputNode);

            default:
                return output.Slot == PortSlot.FlowOut;
        }
    }

    // Returns false only for the pre-In<T> quirk where the old editor accepted the edge visually
    // but wrote nothing (a flow output of a node marked [NodeTypeAttribite("Generic")]).
    public static bool Connect(PortInfo output, BaseDecisionNode outputNode, PortInfo input, BaseDecisionNode inputNode)
    {
        if (!IsNewPort(output, outputNode) && !IsNewPort(input, inputNode))
        {
            switch (DecideLegacy(output, outputNode, input))
            {
                case LegacyDecision.ConnectMeta:
                    input.Field.SetValue(inputNode, output.Field.GetValue(outputNode));
                    AddContext(output, outputNode, input, inputNode);
                    return true;

                case LegacyDecision.ConnectBoth:
                    output.Field.SetValue(outputNode, inputNode);
                    input.Field.SetValue(inputNode, outputNode);
                    AddContext(output, outputNode, input, inputNode);
                    return true;

                case LegacyDecision.SkipKeepEdge:
                    return false;

                default:
                    throw new InvalidOperationException("nodes not match");
            }
        }

        if (!CanConnect(output, outputNode, input, inputNode))
            throw new InvalidOperationException("nodes not match");

        if (input.Slot == PortSlot.InSlot)
        {
            var provider = ProviderOf(output, outputNode, out var index);
            input.Field.SetValue(inputNode, Activator.CreateInstance(input.Field.FieldType, provider, index, output.OutputName));
        }
        else if (input.Slot == PortSlot.LegacyValueIn)
        {
            input.Field.SetValue(inputNode, ProviderOf(output, outputNode, out _));

            if (output.Slot == PortSlot.ValueOutField)
                output.Field.SetValue(outputNode, inputNode);
        }
        else
        {
            output.Field.SetValue(outputNode, inputNode);
            input.Field.SetValue(inputNode, outputNode);
        }

        AddContext(output, outputNode, input, inputNode);
        return true;
    }

    public static void Disconnect(PortInfo input, BaseDecisionNode inputNode, PortInfo output, BaseDecisionNode outputNode)
    {
        if (input.Field == null)
            return;

        var previous = LinkedNode(input, inputNode);

        if (input.Slot == PortSlot.InSlot)
            input.Field.SetValue(inputNode, Activator.CreateInstance(input.Field.FieldType));
        else
            input.Field.SetValue(inputNode, null);

        if (output != null && outputNode != null && (previous == outputNode || input.Slot == PortSlot.InSlot))
            outputNode.ConnectionContexts.Remove(new ConnectionContext { Out = output.Member.Name, In = input.Member.Name });

        // A flow link runs through the output field, so a deleted wire kept executing while this stayed set.
        if (output != null && outputNode != null && output.Field != null && output.Slot != PortSlot.MetaOut
            && output.Field.GetValue(outputNode) as BaseDecisionNode == inputNode)
        {
            output.Field.SetValue(outputNode, null);
            EditorUtility.SetDirty(outputNode);
        }

        EditorUtility.SetDirty(inputNode);
    }

    // An In<T> slot stores the output index; the [Output] name saved with it catches outputs that were
    // reordered in code. Returns one message per slot that was rebound or could not be matched.
    public static List<string> ReconcileOutputNames(BaseStrategy strategy)
    {
        var messages = new List<string>();
        var drawn = DrawnNodes(strategy);
        var rebound = false;

        foreach (var consumer in drawn)
        {
            foreach (var input in GetPorts(consumer.GetType()))
            {
                if (input.Slot != PortSlot.InSlot)
                    continue;

                var slot = (IInSlot)input.Field.GetValue(consumer);

                if (slot.Node == null || string.IsNullOrEmpty(slot.OutputName) || !drawn.Contains(slot.Node))
                    continue;

                var current = ValueOutput(slot.Node.GetType(), Math.Max(1, slot.Output));

                if (current != null && current.OutputName == slot.OutputName)
                    continue;

                var match = GetPorts(slot.Node.GetType()).FirstOrDefault(x =>
                    x.Direction == ConnectionPointType.Out && x.OutputIndex > 0 && x.Slot != PortSlot.MetaOut
                    && x.OutputName == slot.OutputName && IsReadableAs(x.ValueType, input.ValueType));

                var where = $"{strategy.name}: {consumer.GetType().Name}.{input.Member.Name} <- {slot.Node.GetType().Name}";

                if (match != null)
                {
                    input.Field.SetValue(consumer, Activator.CreateInstance(input.Field.FieldType, slot.Node, match.OutputIndex, match.OutputName));
                    EditorUtility.SetDirty(consumer);
                    rebound = true;
                    messages.Add($"{where}: output '{slot.OutputName}' moved from {slot.Output} to {match.OutputIndex}, slot rebound");
                }
                else
                    messages.Add($"{where}: output '{slot.OutputName}' not found, slot stays on output {slot.Output} '{current?.OutputName}'");
            }
        }

        if (rebound)
            AssetDatabase.SaveAssets();

        return messages;
    }

    public static List<RestoredEdge> RestoreEdges(BaseStrategy strategy)
    {
        var edges = new List<RestoredEdge>();
        var drawn = DrawnNodes(strategy);

        foreach (var consumer in drawn)
        {
            foreach (var input in GetPorts(consumer.GetType()))
            {
                if (input.Direction != ConnectionPointType.In)
                    continue;

                if (input.Slot == PortSlot.InSlot)
                    RestoreInSlot(strategy, drawn, consumer, input, edges);
                else
                    RestoreLegacy(strategy, drawn, consumer, input, edges);
            }
        }

        return edges;
    }

    // Same node set and order as the StrategyGraphView constructor: the first StartDecision, then every
    // other non-null node.
    public static List<BaseDecisionNode> DrawnNodes(BaseStrategy strategy)
    {
        var drawn = new List<BaseDecisionNode>();
        var start = strategy.nodes.FirstOrDefault(x => x is StartDecision);

        if (start != null)
            drawn.Add(start);

        drawn.AddRange(strategy.nodes.Where(x => x != null && !(x is StartDecision)));
        return drawn;
    }

    private static void RestoreLegacy(BaseStrategy strategy, List<BaseDecisionNode> drawn, BaseDecisionNode consumer, PortInfo input, List<RestoredEdge> edges)
    {
        var source = input.Field.GetValue(consumer) as BaseDecisionNode;

        if (source == null)
            return;

        if (drawn.Contains(source))
        {
            var output = FieldOutputPointingTo(source, consumer);

            if (output == null && input.Slot == PortSlot.LegacyValueIn && IsMultiOutput(source.GetType()))
                output = MainValueOutput(source.GetType());

            edges.Add(output != null
                ? new RestoredEdge(consumer, input, source, source, output, "drawn")
                : new RestoredEdge(consumer, input, source, null, null, "hidden:no-backlink"));
            return;
        }

        edges.Add(RestoreThroughMeta(strategy, drawn, consumer, input, source));
    }

    private static void RestoreInSlot(BaseStrategy strategy, List<BaseDecisionNode> drawn, BaseDecisionNode consumer, PortInfo input, List<RestoredEdge> edges)
    {
        var slot = (IInSlot)input.Field.GetValue(consumer);
        var source = slot.Node;

        if (source == null)
            return;

        if (drawn.Contains(source))
        {
            var output = ValueOutput(source.GetType(), Math.Max(1, slot.Output));

            edges.Add(output != null
                ? new RestoredEdge(consumer, input, source, source, output, "drawn")
                : new RestoredEdge(consumer, input, source, null, null, "hidden:output-not-found"));
            return;
        }

        edges.Add(RestoreThroughMeta(strategy, drawn, consumer, input, source));
    }

    private static RestoredEdge RestoreThroughMeta(BaseStrategy strategy, List<BaseDecisionNode> drawn, BaseDecisionNode consumer, PortInfo input, BaseDecisionNode source)
    {
        var meta = strategy.Metanodes.FirstOrDefault(x => x.Child == source);

        if (meta.Parent == null)
            return new RestoredEdge(consumer, input, source, null, null, "hidden:source-not-drawn");

        if (!drawn.Contains(meta.Parent))
            return new RestoredEdge(consumer, input, source, null, null, "hidden:meta-parent-not-drawn");

        var output = FieldOutputPointingTo(meta.Parent, source);

        return output != null
            ? new RestoredEdge(consumer, input, source, meta.Parent, output, "drawn:meta")
            : new RestoredEdge(consumer, input, source, null, null, "hidden:meta-port-not-found");
    }

    private static PortInfo FieldOutputPointingTo(BaseDecisionNode node, BaseDecisionNode target)
        => GetPorts(node.GetType()).FirstOrDefault(x => x.Direction == ConnectionPointType.Out && x.IsFieldBacked && x.Field.GetValue(node) as BaseDecisionNode == target);

    private static PortInfo MainValueOutput(Type nodeType) => ValueOutput(nodeType, 1);

    private static PortInfo ValueOutput(Type nodeType, int index)
    {
        var ports = GetPorts(nodeType);

        return ports.FirstOrDefault(x => x.Slot == PortSlot.ValueOutField && x.OutputIndex == index)
               ?? ports.FirstOrDefault(x => x.Slot == PortSlot.MethodOut && x.OutputIndex == index);
    }

    private static BaseDecisionNode ProviderOf(PortInfo output, BaseDecisionNode outputNode, out int index)
    {
        switch (output.Slot)
        {
            case PortSlot.MetaOut:
                index = 1;
                return output.Field.GetValue(outputNode) as BaseDecisionNode;

            case PortSlot.ValueOutField:
            case PortSlot.MethodOut:
                index = output.OutputIndex;
                return outputNode;

            default:
                index = 0;
                return null;
        }
    }

    private static bool IsReadableAs(Type outputType, Type slotType)
        => outputType.IsValueType || slotType.IsValueType ? outputType == slotType : slotType.IsAssignableFrom(outputType);

    private static void AddContext(PortInfo output, BaseDecisionNode outputNode, PortInfo input, BaseDecisionNode inputNode)
    {
        outputNode.ConnectionContexts.AddOrRemoveElement(new ConnectionContext { Out = output.Member.Name, In = input.Member.Name }, true);
        EditorUtility.SetDirty(outputNode);
        EditorUtility.SetDirty(inputNode);
    }

    // Verbatim port of the connection rules the graph editor used before In<T> (StrategyGraphView.Changes
    // + IsValidConnect), including its quirks, so old nodes connect exactly as they always did.
    private static LegacyDecision DecideLegacy(PortInfo output, BaseDecisionNode outputNode, PortInfo input)
    {
        try
        {
            var inputType = input.Field.FieldType;
            var outType = output.Field.FieldType;
            var check = outputNode.GetType().InheritsFrom(inputType);

            if (IsValidLegacyConnect(input.Field, output.Field, outputNode))
                return output.Field.GetCustomAttribute<MetaNodeAttribute>(true) != null ? LegacyDecision.ConnectMeta : LegacyDecision.ConnectBoth;

            if (check && inputType != typeof(BaseDecisionNode))
                return LegacyDecision.ConnectBoth;

            if (outType == typeof(BaseDecisionNode) && inputType == typeof(BaseDecisionNode))
            {
                var attr = outputNode.GetType().GetCustomAttribute<NodeTypeAttribite>(true);
                return attr != null && attr.NodeType == "Generic" ? LegacyDecision.SkipKeepEdge : LegacyDecision.ConnectBoth;
            }

            return LegacyDecision.Reject;
        }
        catch (Exception)
        {
            return LegacyDecision.Reject;
        }
    }

    private static bool IsValidLegacyConnect(FieldInfo input, FieldInfo output, BaseDecisionNode innerNode)
    {
        var attr = output.GetAttribute<ConnectionAttribute>();
        var comment = attr.NameOfField.ToLower();

        var metaNode = output.GetCustomAttribute<MetaNodeAttribute>(true);

        if (metaNode != null)
        {
            if (output.FieldType.IsCastableTo(input.FieldType))
                return true;
        }

        if (input.FieldType.IsGenericType)
        {
            var neededType = innerNode.GetType();

            if (neededType.IsGenericType || neededType.InheritsFrom(typeof(GenericNode<>)))
            {
                var arg = neededType.BaseType.GetGenericArguments().Single();

                foreach (var g in input.FieldType.GetGenericArguments())
                {
                    if (arg.InheritsFrom(g))
                        return true;
                }
            }

            return false;
        }

        if (comment.Contains("<") && comment.Contains(">"))
            return false;

        return true;
    }

    public static string FriendlyName(Type type)
    {
        if (!type.IsGenericType)
            return StrategyGraphView.FromDotNetTypeToCSharpType(type.Name);

        var name = type.Name.Substring(0, type.Name.IndexOf('`'));
        return $"{name}<{string.Join(", ", type.GetGenericArguments().Select(FriendlyName))}>";
    }

    // Value port colours show type compatibility and stay apart from both the node background colours
    // (node kind) and the default blue of flow ports, which is why no type and no derived hue is blue.
    private static readonly Dictionary<Type, Color> knownPortColors = new Dictionary<Type, Color>
    {
        { typeof(bool), new Color(0.93f, 0.33f, 0.33f) },
        { typeof(int), new Color(0.40f, 0.92f, 0.70f) },
        { typeof(Vector3), new Color(0.98f, 0.92f, 0.40f) },
        { typeof(float), new Color(0.62f, 0.90f, 0.30f) },
        { typeof(string), new Color(0.95f, 0.55f, 0.85f) },
        { typeof(Entity), new Color(0.72f, 0.62f, 1.00f) },
    };

    public static Color PortColorOf(Type valueType)
    {
        if (knownPortColors.TryGetValue(valueType, out var color))
            return color;

        // FNV-1a over the full name: string.GetHashCode is not guaranteed stable between runs.
        uint hash = 2166136261;

        foreach (var c in valueType.FullName ?? valueType.Name)
            hash = (hash ^ c) * 16777619;

        var hue = hash % 300;

        if (hue >= 170)
            hue += 60;

        return Color.HSVToRGB(hue / 360f, 0.45f, 0.95f);
    }
}
