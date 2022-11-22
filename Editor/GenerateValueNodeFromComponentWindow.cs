using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HECSFramework.Core;
using HECSFramework.Core.Generator;
using HECSFramework.Unity;
using HECSFramework.Unity.Editor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Strategies;
using UnityEditor;
using static UnityEngine.EventSystems.EventTrigger;

public class GenerateValueNodeFromComponentWindow : OdinEditorWindow
{
    [OnValueChanged(nameof(ClearField))]
    [ValueDropdown(nameof(GetComponents))]
    public Type Component;

    [HideIf("@Component == null")]
    [ValueDropdown(nameof(GetFields))]
    public string Field;

    private const string StrategyNodes = "/StrategyNodes/";

    [MenuItem("HECS Options/Helpers/Generate strategies nodes for components")]
    public static void GetGenerateValueNodeFromComponentWindow()
    {
        GetWindow<GenerateValueNodeFromComponentWindow>();
    }

    private IEnumerable<Type> GetComponents()
    {
        var components = new TypesProvider().TypeToComponentIndex.Keys.ToList();
        return components;
    }

    private void ClearField()
    {
        Field = string.Empty;
    }

    private IEnumerable<string> GetFields()
    {
        if (Component == null)
            return default;



        var fields = Component.GetFields(BindingFlags.Public | BindingFlags.Instance);

        var list = new List<string>(16);

        foreach (var f in fields)
        {
            list.Add(f.Name);
        }

        return list;
    }

    [Button]
    [HideIf("@string.IsNullOrEmpty(Field)")]
    private void GenerateNode()
    {
        var field = Component.GetField(Field);

        var tree = new TreeSyntaxNode();
        var usings = new TreeSyntaxNode();

        tree.Add(usings);
        tree.Add(new UsingSyntax("HECSFramework.Core"));
        tree.Add(new UsingSyntax(field.FieldType.Namespace));

        tree.Add(new NameSpaceSyntax("Strategies"));
        tree.Add(new LeftScopeSyntax());

        tree.Add(new TabSimpleSyntax(1, $"public sealed class {Component.Name}Get{Field} : GenericNode<{field.FieldType.Name}>"));
        tree.Add(new LeftScopeSyntax(1));
        tree.Add(new TabSimpleSyntax(2, $"public override string TitleOfNode {CParse.LeftScope} get; {CParse.RightScope} = {CParse.Quote}{Component.Name}Get{Field}{CParse.Quote};"));

        tree.Add(new TabSimpleSyntax(2, $"[Connection(ConnectionPointType.Out, {CParse.Quote}<{field.FieldType.Name}> Out{CParse.Quote})]"));
        tree.Add(new TabSimpleSyntax(2, "public BaseDecisionNode Out;"));

        tree.Add(new TabSimpleSyntax(2, "public override void Execute(IEntity entity)"));
        tree.Add(new LeftScopeSyntax(2));
        tree.Add(new RightScopeSyntax(2));

        tree.Add(new TabSimpleSyntax(2, $"public override {field.FieldType.Name} Value(IEntity entity)"));
        tree.Add(new LeftScopeSyntax(2));
        tree.Add(new TabSimpleSyntax(3, $"return entity.Get{Component.Name}().{Field}; "));
        tree.Add(new RightScopeSyntax(2));

        tree.Add(new RightScopeSyntax(1));
        tree.Add(new RightScopeSyntax());

        InstallHECS.CheckFolder(InstallHECS.ScriptPath + InstallHECS.HECSGenerated + StrategyNodes);
        InstallHECS.SaveToFile(tree.ToString(), InstallHECS.ScriptPath + InstallHECS.HECSGenerated + StrategyNodes + $"{Component.Name}Get{Field}.cs");
        AssetDatabase.SaveAssets();
    }
}

public class ProjectileComponentGetProjectile : GenericNode<EntityContainer>
{
    public override string TitleOfNode { get; } = "ProjectileComponentGetProjectile";


    [Connection(ConnectionPointType.Out, "<float> Out")]
    public BaseDecisionNode Out;

    public override void Execute(IEntity entity)
    {
    }

    public override EntityContainer Value(IEntity entity)
    {
        return entity.GetAbilityProjectileHolderComponent().Projectile;
    }
}

[Serializable]
public struct FieldInfoName
{
    public string Name;
    public FieldInfo Field;
}
