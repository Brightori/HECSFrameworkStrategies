using System;
using HECSFramework.Core;

namespace Strategies
{
    [Documentation(Doc.HECS, Doc.Strategy, "base for a node with two value outputs: Value and Value2")]
    public abstract class GenericNode<T1, T2> : GenericNode<T1>
    {
        public abstract T2 Value2(Entity entity);

        public override Delegate GetOutput(int index)
            => index == 2 ? new Func<Entity, T2>(Value2) : base.GetOutput(index);
    }

    [Documentation(Doc.HECS, Doc.Strategy, "base for a node with three value outputs: Value, Value2, Value3")]
    public abstract class GenericNode<T1, T2, T3> : GenericNode<T1, T2>
    {
        public abstract T3 Value3(Entity entity);

        public override Delegate GetOutput(int index)
            => index == 3 ? new Func<Entity, T3>(Value3) : base.GetOutput(index);
    }

    [Documentation(Doc.HECS, Doc.Strategy, "base for a node with four value outputs: Value .. Value4")]
    public abstract class GenericNode<T1, T2, T3, T4> : GenericNode<T1, T2, T3>
    {
        public abstract T4 Value4(Entity entity);

        public override Delegate GetOutput(int index)
            => index == 4 ? new Func<Entity, T4>(Value4) : base.GetOutput(index);
    }

    [Documentation(Doc.HECS, Doc.Strategy, "base for a node with five value outputs: Value .. Value5")]
    public abstract class GenericNode<T1, T2, T3, T4, T5> : GenericNode<T1, T2, T3, T4>
    {
        public abstract T5 Value5(Entity entity);

        public override Delegate GetOutput(int index)
            => index == 5 ? new Func<Entity, T5>(Value5) : base.GetOutput(index);
    }

    [Documentation(Doc.HECS, Doc.Strategy, "base for a node with six value outputs: Value .. Value6")]
    public abstract class GenericNode<T1, T2, T3, T4, T5, T6> : GenericNode<T1, T2, T3, T4, T5>
    {
        public abstract T6 Value6(Entity entity);

        public override Delegate GetOutput(int index)
            => index == 6 ? new Func<Entity, T6>(Value6) : base.GetOutput(index);
    }
}
