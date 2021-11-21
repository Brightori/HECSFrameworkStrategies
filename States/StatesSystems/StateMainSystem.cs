using Components;
using HECSFramework.Core;
using Strategies;
using System;

namespace Systems
{
    public class StateMainSystem : BaseSystem, IUpdatable, IInitable<State>
    {
        private State state;
        private StateDataComponent dataComponent;

        public override void InitSystem()
        {
            Owner.TryGetHecsComponent(out dataComponent);
        }

        public void UpdateLocal()
        {
            dataComponent.UpdateCollection();

            var states = dataComponent.EntitiesInCurrentState;
            var count =  states.Count;

            for (int i = 0; i < count; i++)
            {
                state.Update.Execute(states[i]);
            }
        }

        public void Init(State state)
        {
            this.state = state;
        }
    }
}
