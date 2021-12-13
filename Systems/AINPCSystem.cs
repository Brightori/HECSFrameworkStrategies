using Commands;
using Components;
using HECSFramework.Core;
using Strategies;
using System;

namespace Systems
{
    [Serializable]
    [RequiredAtContainer(typeof(NavMeshAgentComponent), typeof(AIStrategyComponent))]
    [Documentation(Doc.NPC, Doc.AI, "Это система которая является основным мозгом для NPC")]
    public class AINPCSystem : BaseSystem, IAINPCSystem
    {
        private Strategy currentStrategy;
        private bool isNeedDecision;
        private bool isStoped;

        public void CommandReact(NeedDecisionCommand command)
        {
            isNeedDecision = true;
        }

        public override void InitSystem()
        {
            currentStrategy = Owner.GetAIStrategyComponent().Strategy;
            currentStrategy?.Init();
            isNeedDecision = true;
        }

        public void CommandReact(IsDeadCommand command)
        {
            if (Owner.TryGetHecsComponent(HMasks.StateContextComponent, out StateContextComponent stateContextComponent))
            {
                stateContextComponent.StrategyState = StrategyState.Stop;
                stateContextComponent.StateHolder.RemoveFromState(Owner);
                Owner.RemoveHecsComponent(stateContextComponent);
            }

            isStoped = true;
        }

        public void CommandReact(ChangeStrategyCommand command)
        {
            currentStrategy = command.Strategy;
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
            currentStrategy = Owner.GetAIStrategyComponent().Strategy;
            isNeedDecision = true;
        }
    }


    public interface IAINPCSystem : ISystem, IUpdatable,
        IReactCommand<NeedDecisionCommand>,
        IReactCommand<IsDeadCommand>,
        IReactCommand<SetDefaultStrategyCommand>,
        IReactCommand<ChangeStrategyCommand>
    {
    }
}