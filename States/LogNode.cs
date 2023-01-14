using Components;
using HECSFramework.Core;

namespace Strategies
{
    [Documentation(Doc.Strategy, "Это базовая нода, она логирует в отдельный компонент ноды по которым мы проходимся в стратегии")]
    public abstract class LogNode : BaseDecisionNode
    {
        private readonly HECSMask stateInfoMask = HMasks.GetMask<StateInfoComponent>();

        public override void Execute(IEntity entity)
        {
#if UNITY_EDITOR
            var info = entity.GetOrAddComponent<StateInfoComponent>(stateInfoMask);

            if (info.NeedInfo)
                info.StateStack.Add(this);
#endif
            Run(entity);
        }

        protected abstract void Run(IEntity entity);
    }
}