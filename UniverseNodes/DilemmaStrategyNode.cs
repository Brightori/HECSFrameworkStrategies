using Components;
using HECSFramework.Core;

namespace Strategies
{
    [NodeTypeAttribite("InnerNode")]
    public class DilemmaStrategyNode : DilemmaDecision, IInitable 
    {
        public override string TitleOfNode { get; } = "Dilemma Strategy Node";

        [ExposeField]
        public DilemmaStrategy Strategy;

        protected override void Run(Entity entity)
        {
            var context =  entity.GetOrAddComponent<StateContextComponent>();

            context.EnterDilemmaStrategy(this, Strategy, Positive, Negative);

            Strategy.Execute(entity);
        }

        public void Init()
        {
            Strategy.Init();
        }
    }
}