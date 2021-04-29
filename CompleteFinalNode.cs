using HECSFramework.Core;

namespace Strategies
{
    public class CompleteFinalNode : FinalDecision
    {
        public override string TitleOfNode => "Strategy Complete";

        public override void Execute(IEntity entity)
        {
        }
    }
}