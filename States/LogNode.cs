using Components;
using HECSFramework.Core;
using HECSFramework.Documentation;

namespace Strategies
{
    [Documentation(Doc.Strategy, "Это базовая нода, она логирует в отдельный компонент ноды по которым мы проходимся в стратегии")]
    public abstract class LogNode : BaseDecisionNode
    {
        private HECSMask stateInfoMask = HMasks.GetMask<StateInfoComponent>();

        public override void Execute(IEntity entity)
        {
#if UNITY_EDITOR
            var info = entity.GetOrAddComponent<StateInfoComponent>(stateInfoMask);
            info.StateStack.Push(this);
#endif
            ExecuteState(entity);
        }

        protected abstract void ExecuteState(IEntity entity);
    }
}