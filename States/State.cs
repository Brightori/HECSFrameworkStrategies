using System;
using System.Linq;
using Components;
using HECSFramework.Core;
using Sirenix.Utilities;
using UnityEngine;

namespace Strategies
{
    [CreateAssetMenu(menuName = "Strategies/State")]
    [Documentation(Doc.Strategy, Doc.AI, "Это подвид стратегии - FSM")]
    public partial class State : BaseStrategy, IState, IInitable, IDecisionNode
    {
        public UpdateStateNode Update { get; private set; }
        public StartDecision StartDecision { get; private set; }

        public override void Init()
        {
            if (isInited) return;

            InitNodes();
        }

        private void InitNodes()
        {
            if (isInited) return;

            isInited = true;

            StartDecision = nodes.FirstOrDefault(x => x is StartDecision) as StartDecision;
            Update = nodes.FirstOrDefault(x => x is UpdateStateNode) as UpdateStateNode;
            nodes.OfType<IAddStateNode>().ForEach(x => x.AddState(this));
            nodes.OfType<SetStateNode>().ForEach(x => x.ExternalState = (true, this));
            nodes.OfType<IInitable>().ForEach(x => x.Init());
        }

        public void Pause(Entity pause)
        {
            pause.GetComponent<StateContextComponent>().StrategyState = StrategyState.Pause;
        }

        public void Stop(Entity entity)
        {
            entity.GetComponent<StateContextComponent>().StrategyState = StrategyState.Stop;
        }

        public void UnPause(Entity entity)
        {
            entity.GetComponent<StateContextComponent>().StrategyState = StrategyState.Run;
        }

        public override void Execute(Entity entity)
        {
            StartDecision?.Execute(entity);
        }

        public void Execute(Entity entity, IDecisionNode exitNode)
        {
            var context = entity.GetOrAddComponent<StateContextComponent>();
            context.CurrentState = this;
            context.ExitStateNodes.Push(exitNode);
            context.StrategyState = StrategyState.Run;
            Execute(entity);
        }
    }

    public interface IState
    {
        void Execute(Entity entity);
        void Pause(Entity entity);
        void UnPause(Entity entity);
        void Stop(Entity entity);
    }

    public interface IAddStateNode
    {
        void AddState(State state);
    }

    public enum StrategyState { Run, Pause, Stop }
}