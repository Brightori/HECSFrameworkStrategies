using Commands;
using Components;
using HECSFramework.Core;
using Strategies;
using System;

namespace Systems
{
    [Serializable]
    [RequiredAtContainer(typeof(AIStrategyComponent))]
    [Documentation(Doc.NPC, Doc.AI, Doc.HECS,  "This is default system for operate strategies on NPC")]
    public class AINPCSystem : BaseSystem, IAINPCSystem
    {
        private Strategy currentStrategy;
        private bool isNeedDecision;
        private bool isStoped;

        [Required] 
        public AIStrategyComponent aIStrategyComponent;

        public void CommandReact(NeedDecisionCommand command)
        {
            Owner.GetComponent<StateContextComponent>().StrategyState = StrategyState.Stop;
            isNeedDecision = true;
        }

        public override void InitSystem()
        {
            Owner.GetOrAddComponent<StateContextComponent>();
            currentStrategy = aIStrategyComponent.Strategy;
            currentStrategy?.Init();

            if (aIStrategyComponent.ManualStart)
            {
                isStoped = true;
                isNeedDecision = false;
                return;
            }

            if (currentStrategy == null)
                isStoped = true;

            isNeedDecision = true;
        }

        public void CommandReact(ChangeStrategyCommand command)
        {
            currentStrategy?.ForceStop(Owner);
            currentStrategy = command.Strategy;
            command.Strategy.Init();
            Owner.GetOrAddComponent<StateContextComponent>().ExitFromStates();
            isNeedDecision = true;
        }

        public void UpdateLocal()
        {
            if (!isNeedDecision || isStoped) return;
            isNeedDecision = false;
            currentStrategy.Execute(Owner);
        }

        public void CommandReact(SetDefaultStrategyCommand command)
        {
            currentStrategy?.ForceStop(Owner);
            currentStrategy = aIStrategyComponent.Strategy;
            Owner.GetOrAddComponent<StateContextComponent>().StrategyState = StrategyState.Stop;
            isNeedDecision = true;
        }

        public override void Dispose()
        {
            if (Owner.TryGetComponent(out StateContextComponent stateContextComponent))
                stateContextComponent.Dispose();

            isNeedDecision = false;
            isStoped = false;
        }

        public void CommandReact(ForceStartAICommand command)
        {
            isStoped = false;
            isNeedDecision = true;
        }

        public void CommandReact(ForceStopAICommand command)
        {
            currentStrategy?.ForceStop(Owner);
            Owner.GetOrAddComponent<StateContextComponent>().StrategyState = StrategyState.Stop;
            isNeedDecision = false;
            isStoped = true;
        }
    }

    public interface IAINPCSystem : ISystem, IUpdatable,
        IReactCommand<NeedDecisionCommand>,
        IReactCommand<SetDefaultStrategyCommand>,
        IReactCommand<ChangeStrategyCommand>,
        IReactCommand<ForceStopAICommand>,
        IReactCommand<ForceStartAICommand>
    {
    }
}