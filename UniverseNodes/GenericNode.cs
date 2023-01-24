using HECSFramework.Core;

namespace Strategies
{
    [Documentation(Doc.HECS, Doc.Strategy, "this is base for providing value by other node")]
    public abstract class GenericNode<T> : BaseDecisionNode
    {
        public abstract T Value(Entity entity);
    }
}