using Commands;
using Components;
using HECSFramework.Core;
using UnityEngine;

namespace Strategies
{
    public delegate void WaitCallbackEntity(IEntity entity);

    public class WaitNode : InterDecision, IInitable
    {
        public override string TitleOfNode { get; } = "Wait";

        [SerializeField] public float WaitTime = 1;

        private WaitAndCallbackCommand cacheTest;


        protected override void Run(IEntity entity)
        {
            if (entity.TryGetHecsComponent(HMasks.StateContextComponent, out StateContextComponent stateContextComponent))
                stateContextComponent.StrategyState = StrategyState.Pause;

            cacheTest.CallBackWaiter = entity;

            EntityManager.Command(cacheTest);
        }

        public void React(IEntity entity)
        {
            if (entity.TryGetHecsComponent(HMasks.StateContextComponent, out StateContextComponent stateContextComponentAfter))
                stateContextComponentAfter.StrategyState = StrategyState.Run;

            next.Execute(entity);
        }

        public void Init()
        {
            cacheTest.CallBack = React;
            cacheTest.Timer = WaitTime;
        }
    }
}