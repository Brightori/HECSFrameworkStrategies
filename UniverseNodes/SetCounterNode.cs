using Components;
using HECSFramework.Core;
using HECSFramework.Unity;
using UnityEngine;

namespace Strategies
{
    public sealed class SetFloatCounterNode : InterDecision
    {
        public override string TitleOfNode { get; } = "Set Float Counter";

        [SerializeField]
        public CounterIdentifierContainer CounterIdentifier;

        [SerializeField]
        public float Value;

        private HECSMask CounterHolderMask = HMasks.GetMask<CountersHolderComponent>();

        protected override void Run(IEntity entity)
        {
            entity.GetHECSComponent<CountersHolderComponent>(ref CounterHolderMask)
                .GetCounter<ICounter<float>>(CounterIdentifier.Id).SetValue(Value);
            
            Next.Execute(entity);
        }
    }
}