using Commands;
using HECSFramework.Core;

namespace Strategies
{
    [Documentation(Doc.Strategy, "Эта нода завершения стратегии, она посылает команду перезапуска стратегии, если нужно чтобы стратегия не перезапускалась - нужна другая нода")]
    public class CompleteFinalNode : FinalDecision
    {
        public override string TitleOfNode => "Strategy Complete";

        protected override void Run(IEntity entity)
        {
            entity.Command(new NeedDecisionCommand());
        }
    } 
}