using Components;
using HECSFramework.Core;

namespace Strategies
{
    public class WaitFloatGenericNode : InterDecision
    {
        public override string TitleOfNode { get; } = "WaitFloatGenericNode";

        [Connection(ConnectionPointType.In, "<float> Time")]
        public GenericNode<float> TimeForWait;
      
        protected override async void Run(Entity entity)
        {
            bool isSetted = false;

            if (entity.TryGetComponent(out StateContextComponent stateContextComponent) && stateContextComponent.StrategyState == StrategyState.Run)
            {
                stateContextComponent.StrategyState = StrategyState.Pause;
                isSetted = true;
            }
                

            var alive = new AliveEntity(entity);
            var strategyIndex = stateContextComponent.CurrentStrategyIndex;

            await new Wait(TimeForWait.Value(entity)).RunJob(entity.World);

            if (!alive.IsAlive)
                return;

            if (isSetted && stateContextComponent.StrategyState != StrategyState.Pause)
                return;

            if (strategyIndex != stateContextComponent.CurrentStrategyIndex)
                return;

            if (entity.TryGetComponent(out StateContextComponent stateContextComponentAfter) && isSetted)
                stateContextComponentAfter.StrategyState = StrategyState.Run;

            Next.Execute(entity);
        }
    }
}