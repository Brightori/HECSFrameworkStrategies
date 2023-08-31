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
            if (entity.TryGetComponent(out StateContextComponent stateContextComponent))
                stateContextComponent.StrategyState = StrategyState.Pause;

            var alive = new AliveEntity(entity);
            await new Wait(TimeForWait.Value(entity)).RunJob(entity.World);

            if (!alive.IsAlive || stateContextComponent.StrategyState != StrategyState.Pause)
                return;

            if (entity.TryGetComponent(out StateContextComponent stateContextComponentAfter))
                stateContextComponentAfter.StrategyState = StrategyState.Run;

            Next.Execute(entity);
        }
    }
}