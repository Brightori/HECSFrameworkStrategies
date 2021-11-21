using Components;
using HECSFramework.Core;
using System;
using System.Collections.Generic;

namespace Strategies
{
    public class ExitStateNode : StateNode
    {
        public override string TitleOfNode { get; } = "Exit State";
        
        [Connection(ConnectionPointType.In, "Input")]
        public StateNode Input;

        [Connection(ConnectionPointType.In, "On Exit")]
        public List<StateNode> CallNodesWhenExit;

        private BaseDecisionNode exitDecision;
        private State currentState;

        protected override void ExecuteState(IEntity entity)
        {
            currentState.Stop(entity);
        }

        public void AddState(State state)
        {
            currentState = state;
        }
    }

    public abstract class StateNode : BaseDecisionNode
    {
        private HECSMask stateInfoMask = HMasks.GetMask<StateInfoComponent>();

        public override void Execute(IEntity entity)
        {
#if UNITY_EDITOR
            var info = entity.GetOrAddComponent<StateInfoComponent>(stateInfoMask);
            info.StateStack.Push(this);
#endif
            ExecuteState(entity);
        }

        protected abstract void ExecuteState(IEntity entity);
    }
}