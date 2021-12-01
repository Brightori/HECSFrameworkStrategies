using Components;
using HECSFramework.Core;
using HECSFramework.Documentation;
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
        public ExitStateNode Exit { get; private set;  }

        private StateMainSystem stateMainSystem = new StateMainSystem();
        private StateDataComponent stateData = new StateDataComponent();

        public override void Init()
        {
            stateMainSystem = new StateMainSystem();
            stateData = new StateDataComponent();

            stateEntity = new Entity("State " + name);
            stateEntity.AddHecsComponent(stateData);
            stateEntity.AddHecsSystem(stateMainSystem);
            stateMainSystem.Init(this);
            stateEntity.Init();
            base.Init();
        }

        public void Pause(IEntity pause)
        {
            stateData.Pause(pause);
        }

        public void Stop(IEntity entity)
        {
            stateData.RemoveFromState(entity);
        }

        public void UnPause(IEntity entity)
        {
            stateData.UnPause(entity);
        }
       
        public override void Execute(IEntity entity)
        {
            stateData.AddToState(entity);
            StartDecision.Execute(entity);
        }
    }

    public interface IState  
    {
        void Execute(IEntity entity);
        void Pause(IEntity entity);
        void UnPause(IEntity entity);
        void Stop(IEntity entity);
    }

    public enum StrategyState { Start, Run, Pause, Stop }
}