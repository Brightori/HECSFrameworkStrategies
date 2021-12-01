using Components;
using HECSFramework.Core;

namespace Strategies
{
    public abstract class StateNode : BaseDecisionNode
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