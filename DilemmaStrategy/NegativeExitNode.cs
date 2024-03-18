using Components;
using HECSFramework.Core;

namespace Strategies
{
    [Documentation(Doc.Strategy, Doc.HECS, "we use this node when we want make negative decision on dilemma strategy")]
    public sealed class NegativeExitNode : FinalDecision
    {
        public override string TitleOfNode { get; } = "NegativeExitNode";

        protected override void Run(Entity entity)
        {
            entity.GetComponent<StateContextComponent>().NegativeDilemmaExit();
        }
    }
}
