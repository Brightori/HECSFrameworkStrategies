using HECSFramework.Core;

namespace Strategies
{
    public class NegativeFinalDecision : FinalDecision, INegativeDecision
    {
        public override string TitleOfNode { get; } = "NegativeFinalDecision";
        public BaseDecisionNode Negative { get; set; }

        protected override void Run(IEntity entity)
        {
            Negative.Execute(entity);
        }
    }
}