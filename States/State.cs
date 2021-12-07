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
    public partial class State : Strategy, IState, IInitable, IDecisionNode
    {
        private Entity stateEntity;

        public UpdateStateNode Update { get; private set; }
        public StartDecision StartDecision { get; private set; }
        public BaseDecisionNode ExitNode { get; private set; }

        private StateUpdateSystem stateMainSystem = new StateUpdateSystem();
        private StateDataComponent stateData = new StateDataComponent();

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
            nodes.OfType<IInitable>().ForEach(x => x.Init());
        }

        public void Pause(IEntity pause)
        {
            stateData.Pause(pause);
        }

        public void AddExitNode(BaseDecisionNode exit)
        {
            ExitNode = exit;
        }

        public void Stop(IEntity entity)
        {
            stateData.RemoveFromState(entity);
            entity.RemoveHecsComponent(HMasks.StateContextComponent);
        }

        public void UnPause(IEntity entity)
        {
            stateData.UnPause(entity);
        }

        public override void Execute(IEntity entity)
        {
            stateData.AddToState(entity);
            StartDecision.Execute(entity);
            entity.GetOrAddComponent<StateContextComponent>(HMasks.StateContextComponent).StateHolder = stateData;
            entity.GetStateContextComponent().StrategyState = StrategyState.Run;
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