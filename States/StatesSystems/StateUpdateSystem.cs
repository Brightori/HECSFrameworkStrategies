using Components;
using HECSFramework.Core;
using Strategies;

namespace Systems
{
    [Documentation(Doc.AI, Doc.Strategy, Doc.State, Doc.HECS, "это глобальная система которая отвечает за апдейт состояний")]
    public class StateUpdateSystem : BaseSystem, IUpdatable
    {
        private EntitiesFilter statesEntities;

        public override void InitSystem()
        {
            //todo filter
            statesEntities = Owner.World.GetFilter<StateContextComponent>();
        }

        public void UpdateLocal()
        {
            var count = statesEntities.Count;

            foreach (var needed in statesEntities)
            {
                if (needed.TryGetComponent(out StateContextComponent stateContextComponent))
                {
                    if (stateContextComponent.StrategyState != StrategyState.Run) continue;
                    stateContextComponent.CurrentState.Update.Execute(needed);
                }
            }
        }
    }
}