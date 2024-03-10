using System;
using System.Collections.Generic;
using HECSFramework.Core;
using Strategies;

namespace Components
{
    [Serializable, Documentation(Doc.Tag, Doc.AI, Doc.State, "Это компонент всю ключевую информацию про состояние персонажа")]
    public class StateContextComponent : BaseComponent, IDisposable
    {
        public StrategyState StrategyState = StrategyState.Stop;
        public State CurrentState;
        public IDecisionNode EarlyUpdateNode;
        public int CurrentStrategyIndex;
        public int CurrentIteration;

        public Stack<StateExit> ExitStateNodes = new Stack<StateExit>(3); //выход из текущего стейта, сеттим на входе в стейт

        public void EnterState(SetStateNode node)
        {
            ExitStateNodes.Push(new StateExit { PreviousState = CurrentState, SetStateNode = node, ExitNode = node.Exit });
        }

        public void ExitState(Entity entity)
        {
            if (ExitStateNodes.TryPop(out var result))
            {
                if (result.PreviousState != null)
                {
                    CurrentState = result.PreviousState;
                    EarlyUpdateNode = result.SetStateNode.EarlyUpdateNodes;
                    StrategyState = StrategyState.Run;
                }
                
                result.ExitNode.Execute(entity);
            }
        }

        public void ExitFromStates()
        {
            StrategyState = StrategyState.Stop;
            ExitStateNodes.Clear();
        }

        public void Dispose()
        {
            ExitFromStates();
            CurrentState = null;
            EarlyUpdateNode = null;
        }
    }

    public struct StateExit : IEquatable<StateExit>
    {
        public State PreviousState;
        public SetStateNode SetStateNode;
        public IDecisionNode ExitNode;

        public override bool Equals(object obj)
        {
            return obj is StateExit exit &&
                   EqualityComparer<State>.Default.Equals(PreviousState, exit.PreviousState) &&
                   EqualityComparer<SetStateNode>.Default.Equals(SetStateNode, exit.SetStateNode) &&
                   EqualityComparer<IDecisionNode>.Default.Equals(ExitNode, exit.ExitNode);
        }

        public bool Equals(StateExit exit)
        {
            return EqualityComparer<State>.Default.Equals(PreviousState, exit.PreviousState) &&
                    EqualityComparer<SetStateNode>.Default.Equals(SetStateNode, exit.SetStateNode) &&
                    EqualityComparer<IDecisionNode>.Default.Equals(ExitNode, exit.ExitNode);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PreviousState, SetStateNode, ExitNode);
        }
    }
}