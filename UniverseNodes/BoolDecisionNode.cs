using HECSFramework.Core;


namespace Strategies
{
    [Documentation(Doc.HECS, Doc.Strategy, "this node select branch from bool input")]
    public sealed class BoolDecisionNode : DilemmaDecision
    {
        [Connection (ConnectionPointType.In, "<bool> Outer Decision")]
        public GenericNode<bool> OuterDecision;

        public override string TitleOfNode { get; } = "BoolDecision";

        protected override void Run(Entity entity)
        {
            if (OuterDecision.Value(entity))
            {
                Positive.Execute(entity);
                return;
            }
            else
            {
                Negative.Execute(entity);
                return;
            }
        }
    }
}
