﻿using Components;
using HECSFramework.Core;

namespace Strategies
{
    [Documentation(Doc.HECS, Doc.Counters, Doc.Strategy, "on this component we get cached value from component")]
    public sealed class GetFloatValueFromCacheCounter : GenericNode<float>
    {
        public override string TitleOfNode { get; } = "GetFloatValueFromCacheCounter";

        [DropDownIdentifier("CounterIdentifierContainer")]
        public int CounterID;

        private HECSMask cacheCountersMask = HMasks.GetMask<CacheCounterValuesComponent>();

        [Connection(ConnectionPointType.Out, "<float> Out")]
        public BaseDecisionNode Out;

        public override void Execute(IEntity entity)
        {
        }

        public override float Value(IEntity entity)
        {
            if (entity.TryGetHecsComponent(cacheCountersMask, out CacheCounterValuesComponent cacheCounterValuesComponent))
            {
                if (cacheCounterValuesComponent.Values.TryGetValue(CounterID, out var counterValue))
                {
                    return counterValue;
                }
            }

            return 0;
        }
    }
}