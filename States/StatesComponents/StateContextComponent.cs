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
        public Stack<DilemmaStrategyExit> DilemmaDecisionNodes = new Stack<DilemmaStrategyExit>(3); //выход из текущего стейта, сеттим на входе в стейт

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
            DilemmaDecisionNodes.Clear();
        }

        public void Dispose()
        {
            ExitFromStates();
            CurrentState = null;
            EarlyUpdateNode = null;
        }

        internal void EnterDilemmaStrategy(DilemmaStrategyNode dilemmaStrategyNode, DilemmaStrategy strategy, BaseDecisionNode positive, BaseDecisionNode negative)
        {
            DilemmaDecisionNodes.Push(new DilemmaStrategyExit(dilemmaStrategyNode, strategy, positive, negative));
        }

        public void PositiveDilemmaExit()
        {
            if (DilemmaDecisionNodes.TryPop(out var result))
                result.Positive.Execute(Owner);
            else
                throw new Exception("we dont have exit");
        }

        public void NegativeDilemmaExit() 
        {
            if (DilemmaDecisionNodes.TryPop(out var result))
                result.Negative.Execute(Owner);
            else
                throw new Exception("we dont have exit");
        }
    }

    public struct DilemmaStrategyExit : IEquatable<DilemmaStrategyExit>
    {
        public DilemmaStrategyNode DillemaDecisionByStrategy;
        public DilemmaStrategy DilemmaStrategy;
        public BaseDecisionNode Positive;
        public BaseDecisionNode Negative;

        public DilemmaStrategyExit(DilemmaStrategyNode dillemaDecisionByStrategy, DilemmaStrategy dilemmaStrategy, BaseDecisionNode positive, BaseDecisionNode negative)
        {
            DillemaDecisionByStrategy = dillemaDecisionByStrategy;
            DilemmaStrategy = dilemmaStrategy;
            Positive = positive;
            Negative = negative;
        }

        public override bool Equals(object obj)
        {
            return obj is DilemmaStrategyExit exit &&
                   EqualityComparer<DilemmaStrategyNode>.Default.Equals(DillemaDecisionByStrategy, exit.DillemaDecisionByStrategy) &&
                   EqualityComparer<DilemmaStrategy>.Default.Equals(DilemmaStrategy, exit.DilemmaStrategy) &&
                   EqualityComparer<BaseDecisionNode>.Default.Equals(Positive, exit.Positive) &&
                   EqualityComparer<BaseDecisionNode>.Default.Equals(Negative, exit.Negative);
        }

        public bool Equals(DilemmaStrategyExit other)
        {
            return EqualityComparer<DilemmaStrategyNode>.Default.Equals(DillemaDecisionByStrategy, other.DillemaDecisionByStrategy) &&
                  EqualityComparer<DilemmaStrategy>.Default.Equals(DilemmaStrategy, other.DilemmaStrategy) &&
                  EqualityComparer<BaseDecisionNode>.Default.Equals(Positive, other.Positive) &&
                  EqualityComparer<BaseDecisionNode>.Default.Equals(Negative, other.Negative);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DillemaDecisionByStrategy, DilemmaStrategy, Positive, Negative);
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