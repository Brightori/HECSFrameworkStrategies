using System;
using HECSFramework.Core;

namespace Strategies
{
    [Documentation(Doc.HECS, Doc.Strategy, "Value node with two outputs: Value (output 1, T1) and Value2 (output 2, T2). " + MultiGenericNodeDoc.Usage)]
    public abstract class GenericNode<T1, T2> : GenericNode<T1>
    {
        public abstract T2 Value2(Entity entity);

        public override Delegate GetOutput(int index)
            => index == 2 ? new Func<Entity, T2>(Value2) : base.GetOutput(index);
    }

    [Documentation(Doc.HECS, Doc.Strategy, "Value node with three outputs: Value (T1), Value2 (T2), Value3 (T3). " + MultiGenericNodeDoc.Usage)]
    public abstract class GenericNode<T1, T2, T3> : GenericNode<T1, T2>
    {
        public abstract T3 Value3(Entity entity);

        public override Delegate GetOutput(int index)
            => index == 3 ? new Func<Entity, T3>(Value3) : base.GetOutput(index);
    }

    [Documentation(Doc.HECS, Doc.Strategy, "Value node with four outputs: Value (T1) .. Value4 (T4). " + MultiGenericNodeDoc.Usage)]
    public abstract class GenericNode<T1, T2, T3, T4> : GenericNode<T1, T2, T3>
    {
        public abstract T4 Value4(Entity entity);

        public override Delegate GetOutput(int index)
            => index == 4 ? new Func<Entity, T4>(Value4) : base.GetOutput(index);
    }

    [Documentation(Doc.HECS, Doc.Strategy, "Value node with five outputs: Value (T1) .. Value5 (T5). " + MultiGenericNodeDoc.Usage)]
    public abstract class GenericNode<T1, T2, T3, T4, T5> : GenericNode<T1, T2, T3, T4>
    {
        public abstract T5 Value5(Entity entity);

        public override Delegate GetOutput(int index)
            => index == 5 ? new Func<Entity, T5>(Value5) : base.GetOutput(index);
    }

    [Documentation(Doc.HECS, Doc.Strategy, "Value node with six outputs: Value (T1) .. Value6 (T6). " + MultiGenericNodeDoc.Usage)]
    public abstract class GenericNode<T1, T2, T3, T4, T5, T6> : GenericNode<T1, T2, T3, T4, T5>
    {
        public abstract T6 Value6(Entity entity);

        public override Delegate GetOutput(int index)
            => index == 6 ? new Func<Entity, T6>(Value6) : base.GetOutput(index);
    }

    internal static class MultiGenericNodeDoc
    {
        public const string Usage =
            "Each ValueN override is a separate output port; outputs may share a type. " +
            "Name every output with [Output(\"name\")] placed on its override; the name is the port label and must be unique within the node. " +
            "Without it the port is labelled by the method name (Value2). " +
            "Output 1 also connects to old GenericNode<T1> slots; every output connects to In<T> slots of a matching type " +
            "(exact type for value types, the type or a base type for reference types). " +
            "No Out field is needed: the editor builds output ports from the ValueN methods.\n" +
            "Example:\n" +
            "public sealed class WeaponStateNode : GenericNode<Entity, int, int>\n" +
            "{\n" +
            "    public override string TitleOfNode { get; } = \"WeaponState\";\n" +
            "\n" +
            "    public override void Execute(Entity entity) { }\n" +
            "\n" +
            "    [Output(\"owner\")]\n" +
            "    public override Entity Value(Entity entity) => entity;\n" +
            "\n" +
            "    [Output(\"ammo\")]\n" +
            "    public override int Value2(Entity entity) => 30;\n" +
            "\n" +
            "    [Output(\"maxAmmo\")]\n" +
            "    public override int Value3(Entity entity) => 60;\n" +
            "}";
    }
}
