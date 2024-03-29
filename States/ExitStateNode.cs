﻿using System;
using Components;
using HECSFramework.Core;

namespace Strategies
{
    [NodeTypeAttribite("FinalNode")]
    public class ExitStateNode : LogNode, IAddStateNode
    {
        public override string TitleOfNode { get; } = "Exit State";

        [Connection(ConnectionPointType.In, "Input")]
        public BaseDecisionNode Input;

        [Connection(ConnectionPointType.Out, "On Exit")]
        public BaseDecisionNode CallNodesWhenExit;

        private State currentState;

        protected override void Run(Entity entity)
        {
            currentState.Stop(entity);
            CallNodesWhenExit?.Execute(entity);
            entity.GetComponent<StateContextComponent>().ExitState(entity);
        }

        public void AddState(State state)
        {
            currentState = state;
        }
    }
}