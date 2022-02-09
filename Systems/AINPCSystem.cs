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

        [Required] private AIStrategyComponent aIStrategyComponent;
        private HECSMask StateContextComponentMask = HMasks.GetMask<StateContextComponent>();

        public void CommandReact(NeedDecisionCommand command)
        {
            Owner.GetStateContextComponent().StrategyState = StrategyState.Stop;
            isNeedDecision = true;
        }

        public override void InitSystem()
        {
            Owner.TryGetHecsComponent(out aIStrategyComponent);
            currentStrategy = aIStrategyComponent.Strategy;
            currentStrategy?.Init();
            isNeedDecision = true;
        }

        public void CommandReact(IsDeadCommand command)
        {
            if (Owner.TryGetHecsComponent(StateContextComponentMask, out StateContextComponent stateContextComponent))
                stateContextComponent.StrategyState = StrategyState.Stop;

            isStoped = true;
        }

        public void CommandReact(ChangeStrategyCommand command)
        {
            currentStrategy = command.Strategy;
            Owner.GetStateContextComponent().StrategyState = StrategyState.Stop;
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
            currentStrategy = aIStrategyComponent.Strategy;
            Owner.GetStateContextComponent().StrategyState = StrategyState.Stop;
            isNeedDecision = true;
        }

        public override void Dispose()
        {
            if (Owner.TryGetHecsComponent(StateContextComponentMask, out StateContextComponent stateContextComponent))
                stateContextComponent.Dispose();
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