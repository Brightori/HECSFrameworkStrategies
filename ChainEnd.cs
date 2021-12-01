using HECSFramework.Core;
using HECSFramework.Documentation;

namespace Strategies
{
    [Documentation(Doc.AI, Doc.Strategy, "Это нода пустышка которую мы ставим в конце цепочки для того чтобы не проверять на налл каждую следующую ноду  ")]
    public class ChainEnd : StateNode
    {
        [Connection(ConnectionPointType.In, "Input")]
        public BaseDecisionNode inputNode;

        public override string TitleOfNode => "Chain End";

        protected override void ExecuteState(IEntity entity)
        {
        }
    }
}