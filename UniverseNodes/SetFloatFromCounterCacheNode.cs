using Components;
using HECSFramework.Core;

namespace Strategies
{
    [Documentation(Doc.HECS, Doc.Strategy, "This node save current value of counter to cache component, we can read it later, for compare or set this value again")]
    public sealed class SetFloatFromCounterCacheNode : GenericNode<float>
    {
        public override string TitleOfNode { get; } = "SetFloatFromCounterCacheNode";

        [DropDownIdentifier("CounterIdentifierContainer")]
        public int CounterID;

        private HECSMask CacheCountersMask = HMasks.GetMask<CacheCounterValuesComponent>();
        private HECSMask CounterHolderMask = HMasks.GetMask<CountersHolderComponent>();

        public override float Value(IEntity entity)
        {
            if (entity.TryGetHecsComponent(CacheCountersMask, out CacheCounterValuesComponent cacheCounterValuesComponent))
            {
                if (cacheCounterValuesComponent.Values.TryGetValue(CounterID, out var counter))
                {
                    return counter;
                }
            }
            
            return 0;
        }

        public override void Execute(IEntity entity)
        {
        }
    }
}