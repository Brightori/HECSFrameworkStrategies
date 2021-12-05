using HECSFramework.Core;
using System;
using System.Collections.Generic;

namespace Strategies
{
    public class ExitStateNode : LogNode
    {
        public override string TitleOfNode { get; } = "Exit State";
        
        [Connection(ConnectionPointType.In, "Input")]
        public LogNode Input;

        [Connection(ConnectionPointType.Out, "On Exit")]
        public BaseDecisionNode CallNodesWhenExit;

        private State currentState;

        protected override void Run(IEntity entity)
        {
            currentState.Stop(entity);
            CallNodesWhenExit?.Execute(entity);
            currentState.ExitNode?.Execute(entity);
        }

        public void AddState(State state)
        {
            currentState = state;
        }
    }
}