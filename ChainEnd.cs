using HECSFramework.Core;
using HECSFramework.Documentation;

namespace Strategies
{
    [Documentation(Doc.AI, Doc.Strategy, "Это нода пустышка которую мы ставим в конце цепочки для того чтобы не проверять на налл каждую следующую ноду, отличает от Complete ноды тем, что также содержит дебаг функционал для стака выполнения стейта  ")]
    public class ChainEnd : LogNode
    {
        [Connection(ConnectionPointType.In, "Input")]
        public BaseDecisionNode inputNode;

        public override string TitleOfNode => "Chain End";

        protected override void ExecuteState(IEntity entity)
        {
        }
    }
}