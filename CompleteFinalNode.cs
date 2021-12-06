using Commands;
using HECSFramework.Core;

namespace Strategies
{
    public class CompleteFinalNode : FinalDecision
    {
        public override string TitleOfNode => "Strategy Complete";

        protected override void Run(IEntity entity)
        {
            entity.Command(new NeedDecisionCommand());
        }
    } 
}