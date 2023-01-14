using System;
using Components;
using HECSFramework.Core;
using Strategies;

namespace Systems
{
    [Documentation(Doc.AI, Doc.Strategy, Doc.State, Doc.HECS, "это глобальная система которая отвечает за апдейт состояний")]
    public class StateUpdateSystem : BaseSystem, IUpdatable
    {
        private HECSMask StateContextComponentMask = HMasks.GetMask<StateContextComponent>();
        private HECSList<IEntity> statesEntities;
        private HECSMask ContextComponent = HMasks.GetMask<StateContextComponent>();

        public override void InitSystem()
        {
            statesEntities = Owner.World.Filter(ContextComponent);
        }

        public void UpdateLocal()
        {
            var count = statesEntities.Count;

            for (int i = 0; i < count; i++)
            {
                var needed = statesEntities.Data[i];

                if (needed.TryGetHecsComponent(StateContextComponentMask, out StateContextComponent stateContextComponent))
                {
                    if (stateContextComponent.StrategyState != StrategyState.Run) continue;
                    stateContextComponent.CurrentState.Update.Execute(needed);
                }
            }
        }
    }
}