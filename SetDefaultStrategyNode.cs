using Commands;
using HECSFramework.Core;

namespace Strategies
{
    public class SetDefaultStrategyNode : FinalDecision
    {
        public override string TitleOfNode { get; } = "Set Default Strategy";

        protected override void Run(IEntity entity)
        {
            entity.Command(new SetDefaultStrategyCommand());
        }
    }
}