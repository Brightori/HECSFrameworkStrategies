using Commands;
using Components;
using HECSFramework.Core;

namespace Strategies
{
    public class ExitStateNode : LogNode
    {
        public override string TitleOfNode { get; } = "Exit State";

        [Connection(ConnectionPointType.In, "Input")]
        public LogNode Input;

        [Connection(ConnectionPointType.Out, "On Exit")]
        public BaseDecisionNode CallNodesWhenExit;
        private HECSMask StateContextComponentMask = HMasks.GetMask<StateContextComponent>();

        private State currentState;

        protected override void Run(IEntity entity)
        {
            currentState.Stop(entity);
            CallNodesWhenExit?.Execute(entity);

            EntityManager.Command(new WaitAndCallbackCommand { CallBack = React, CallBackWaiter = entity, Timer = 0.01f });
        }

        private void React(IEntity entity)
        {
            var exitNode = entity.GetHECSComponent<StateContextComponent>(ref StateContextComponentMask)?.ExitStateNode;
            exitNode?.OnExit(entity);
        }

        public void AddState(State state)
        {
            currentState = state;
        }
    }
}