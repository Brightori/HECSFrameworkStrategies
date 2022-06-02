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
        public IDecisionNode ExitNode { get; private set; }
        
        private HECSMask StateContextComponentMask = HMasks.GetMask<StateContextComponent>();

        [NonSerialized] private bool isInited; //это чтобы избежать рекурсии при ссылке инит нод друг на друга

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

        public void Pause(IEntity pause)
        {
            pause.GetHECSComponent<StateContextComponent>(ref StateContextComponentMask).StrategyState = StrategyState.Pause;
        }

        public void Stop(IEntity entity)
        {
            entity.GetHECSComponent<StateContextComponent>(ref StateContextComponentMask).StrategyState = StrategyState.Stop;
        }

        public void UnPause(IEntity entity)
        {
            entity.GetHECSComponent<StateContextComponent>(ref StateContextComponentMask).StrategyState = StrategyState.Run;
        }

        public override void Execute(IEntity entity)
        {
            StartDecision?.Execute(entity);
        }

        public void Execute(IEntity entity, IDecisionNode exitNode)
        {
            SetupState(entity);
            ExitNode = exitNode;
            Execute(entity);
        }

        public void SetupState(IEntity entity)
        {
            var context = entity.GetOrAddComponent<StateContextComponent>(StateContextComponentMask);
            context.CurrentState = this;
            context.StrategyState = StrategyState.Run;
        }
    }

    public interface IState
    {
        void Execute(IEntity entity);
        void Pause(IEntity entity);
        void UnPause(IEntity entity);
        void Stop(IEntity entity);
    }

    public interface IAddStateNode
    {
        void AddState(State state);
    }

    public enum StrategyState { Run, Pause, Stop }
}