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

        [Required] 
        public AIStrategyComponent aIStrategyComponent;
        
        private HECSMask StateContextComponentMask = HMasks.GetMask<StateContextComponent>();

        public void CommandReact(NeedDecisionCommand command)
        {
            Owner.GetHECSComponent<StateContextComponent>(ref StateContextComponentMask).StrategyState = StrategyState.Stop;
            isNeedDecision = true;
        }

        public override void InitSystem()
        {
            Owner.GetOrAddComponent<StateContextComponent>();
            currentStrategy = aIStrategyComponent.Strategy;
            currentStrategy?.Init();

            if (currentStrategy == null)
                isStoped = true;

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
            command.Strategy.Init();
            Owner.GetOrAddComponent<StateContextComponent>(StateContextComponentMask).StrategyState = StrategyState.Stop;
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
            Owner.GetOrAddComponent<StateContextComponent>(StateContextComponentMask).StrategyState = StrategyState.Stop;
            isNeedDecision = true;
        }

        public override void Dispose()
        {
            if (Owner.TryGetHecsComponent(StateContextComponentMask, out StateContextComponent stateContextComponent))
                stateContextComponent.Dispose();
        }

        public void CommandReact(ForceStartAICommand command)
        {
            isStoped = false;
            isNeedDecision = true;
        }

        public void CommandReact(ForceStopAICommand command)
        {
            Owner.GetOrAddComponent<StateContextComponent>(StateContextComponentMask).StrategyState = StrategyState.Stop;
            isNeedDecision = false;
            isStoped = true;
        }
    }

    public interface IAINPCSystem : ISystem, IUpdatable,
        IReactCommand<NeedDecisionCommand>,
        IReactCommand<IsDeadCommand>,
        IReactCommand<SetDefaultStrategyCommand>,
        IReactCommand<ChangeStrategyCommand>,
        IReactCommand<ForceStopAICommand>,
        IReactCommand<ForceStartAICommand>
    {
    }
}