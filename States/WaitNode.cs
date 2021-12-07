using Commands;
using Components;
using HECSFramework.Core;
using UnityEngine;

namespace Strategies
{
    public delegate void WaitCallbackEntity(IEntity entity);

    public class WaitNode : InterDecision
    {
        public override string TitleOfNode { get; } = "Wait";

        [SerializeField] public float WaitTime = 1;

        protected override void Run(IEntity entity)
        {
            if (entity.TryGetHecsComponent(HMasks.StateContextComponent, out StateContextComponent stateContextComponent))
                stateContextComponent.StrategyState = StrategyState.Pause;

            EntityManager.Command(new WaitAndCallbackCommand { CallBackWaiter = entity, Timer = WaitTime, CallBack = React });
        }

        public void React(IEntity entity)
        {
            if (entity.TryGetHecsComponent(HMasks.StateContextComponent, out StateContextComponent stateContextComponentAfter))
                stateContextComponentAfter.StrategyState = StrategyState.Run;

            next.Execute(entity);
        }
    }
}