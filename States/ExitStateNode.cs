using System;
using Components;
using HECSFramework.Core;

namespace Strategies
{
    public class ExitStateNode : LogNode, IAddStateNode
    {
        public override string TitleOfNode { get; } = "Exit State";

        [Connection(ConnectionPointType.In, "Input")]
        public LogNode Input;

        [Connection(ConnectionPointType.Out, "On Exit")]
        public BaseDecisionNode CallNodesWhenExit;

        [NonSerialized]
        private HECSMask StateContextComponentMask = HMasks.GetMask<StateContextComponent>();

        private State currentState;

        protected override void Run(IEntity entity)
        {
            currentState.Stop(entity);
            CallNodesWhenExit?.Execute(entity);
            entity.GetHECSComponent<StateContextComponent>(ref StateContextComponentMask).ExitStateNodes.Pop().Execute(entity);
        }

        public void AddState(State state)
        {
            currentState = state;
        }
    }
}