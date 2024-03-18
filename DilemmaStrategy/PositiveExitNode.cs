using Components;
using HECSFramework.Core;

namespace Strategies
{
    [Documentation(Doc.Strategy, Doc.HECS, "we use this node when we want make positive decision on dilemma strategy" )]
    public sealed class PositiveExitNode : FinalDecision
    {
        public override string TitleOfNode { get; } = "PositiveExitNode";

        protected override void Run(Entity entity)
        {
            entity.GetComponent<StateContextComponent>().PositiveDilemmaExit();
        }
    }
}
