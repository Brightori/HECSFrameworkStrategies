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
        [SerializeField] public float MaxWaitTime = 1;

        private WaitAndCallbackCommand waitCommand;
        private HECSMask StateContextComponent = HMasks.GetMask<StateContextComponent>();


        protected override void Run(IEntity entity)
        {
            if (entity.TryGetHecsComponent(StateContextComponent, out StateContextComponent stateContextComponent))
                stateContextComponent.StrategyState = StrategyState.Pause;

            waitCommand.Timer = Random.Range(WaitTime, MaxWaitTime);
            waitCommand.CallBackWaiter = entity;
            
            EntityManager.Command(waitCommand);
        }

        public void React(IEntity entity)
        {
            if (entity.TryGetHecsComponent(StateContextComponent, out StateContextComponent stateContextComponentAfter))
                stateContextComponentAfter.StrategyState = StrategyState.Run;

            Next.Execute(entity);
        }

        public void Init()
        {
            waitCommand.CallBack = React;
            waitCommand.Timer = WaitTime;
        }
    }
}