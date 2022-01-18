using Components;
using HECSFramework.Core;
using Strategies;

namespace Systems
{
    public class StateUpdateSystem : BaseSystem, IUpdatable, IInitable<State>
    {
        private State state;
        private StateDataComponent dataComponent;
        private HECSMask StateContextComponentMask = HMasks.GetMask<StateContextComponent>();

        public override void InitSystem()
        {
            Owner.TryGetHecsComponent(out dataComponent);
        }

        public void UpdateLocal()
        {
            if (dataComponent.State != StrategyState.Run)
                return;

            dataComponent.UpdateCollection();

            var states = dataComponent.EntitiesInCurrentState;
            var count = states.Count;

            for (int i = 0; i < count; i++)
            {
                var needed = states[i];

                if (needed.TryGetHecsComponent(StateContextComponentMask, out StateContextComponent stateContextComponent))
                {
                    if (stateContextComponent.StrategyState != StrategyState.Run) continue;
                    state.Update.Execute(needed);
                }
                else
                {
                    HECSDebug.LogError("нет стейт компонента у " + needed.ID + " " + state.name);
                }
            }
        }

        public void Init(State state)
        {
            this.state = state;
        }
    }
}
