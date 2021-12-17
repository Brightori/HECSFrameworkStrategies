using Components;
using HECSFramework.Core;
using HECSFramework.Documentation;
using Sirenix.Utilities;
using System;
using System.Linq;
using Systems;
using UnityEngine;

namespace Strategies
{
    [CreateAssetMenu(menuName = "Strategies/State")]
    [Documentation(Doc.Strategy, Doc.AI, "Это подвид стратегии - FSM")]
    public partial class State : BaseStrategy, IState, IInitable, IDecisionNode
    {
        private Entity stateEntity;

        public UpdateStateNode Update { get; private set; }
        public StartDecision StartDecision { get; private set; }

        private StateUpdateSystem stateMainSystem = new StateUpdateSystem();
        private StateDataComponent stateData = new StateDataComponent();
        private HECSMask StateContextComponentMask = HMasks.GetMask<StateContextComponent>();


        [NonSerialized] private bool isInited; //это чтобы избежать рекурсии при ссылке инит нод друг на друга

        public override void Init()
        {
            if (isInited) return;

            InitNodes();
            stateMainSystem = new StateUpdateSystem();
            stateData = new StateDataComponent();

            stateEntity = new Entity("State " + name);
            stateEntity.AddHecsComponent(stateData);
            stateEntity.AddHecsSystem(stateMainSystem);
            stateMainSystem.Init(this);
            stateEntity.GenerateGuid();
            stateEntity.Init();
        }

        private void InitNodes()
        {
            if (isInited) return;

            isInited = true;

            StartDecision = nodes.FirstOrDefault(x => x is StartDecision) as StartDecision;
            Update = nodes.FirstOrDefault(x => x is UpdateStateNode) as UpdateStateNode;
            nodes.OfType<ExitStateNode>().ForEach(x => x.AddState(this));
            nodes.OfType<SetStateNode>().ForEach(x => x.ExternalState = (true,this));
            nodes.OfType<IInitable>().ForEach(x => x.Init());
        }

        public void Pause(IEntity pause)
        {
            stateData.Pause(pause);
        }

        public void Stop(IEntity entity)
        {
            stateData.RemoveFromState(entity);
            entity.RemoveHecsComponent(StateContextComponentMask);
        }

        public void UnPause(IEntity entity)
        {
            stateData.UnPause(entity);
        }

        public override void Execute(IEntity entity)
        {
            SetupState(entity);
            StartDecision?.Execute(entity);
        }

        public void Execute(IEntity entity, SetStateNode exitNode)
        {
            Execute(entity);
            entity.GetHECSComponent<StateContextComponent>(ref StateContextComponentMask).ExitStateNode = exitNode;
        }

        public void SetupState(IEntity entity)
        {
            stateData.AddToState(entity);

            var context = entity.GetOrAddComponent<StateContextComponent>(StateContextComponentMask);

            if (context.StrategyState == StrategyState.Run)
                context.ExitFromState();

            context.StateHolder = stateData;
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

    public enum StrategyState { Run, Pause, Stop }
}