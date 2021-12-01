﻿using HECSFramework.Core;
using System;
using System.Collections.Generic;

namespace Strategies
{
    public class ExitStateNode : StateNode
    {
        public override string TitleOfNode { get; } = "Exit State";
        
        [Connection(ConnectionPointType.In, "Input")]
        public StateNode Input;

        [Connection(ConnectionPointType.Out, "On Exit")]
        public BaseDecisionNode CallNodesWhenExit;

        private BaseDecisionNode exitDecision;
        private State currentState;

        protected override void ExecuteState(IEntity entity)
        {
            currentState.Stop(entity);
            CallNodesWhenExit.Execute(entity);
            exitDecision.Execute(entity);
        }

        public void AddState(State state)
        {
            currentState = state;
        }
    }
}