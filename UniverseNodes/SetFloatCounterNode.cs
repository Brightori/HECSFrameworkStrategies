using Components;
using HECSFramework.Core;
using HECSFramework.Unity;
using UnityEngine;

namespace Strategies
{
    public sealed class SetFloatCounterNode : InterDecision
    {
        public override string TitleOfNode { get; } = "Set Float Counter";

        [DropDownIdentifier("CounterIdentifierContainer")]
        public int Counter;

        [SerializeField]
        public float Value;

        private HECSMask CounterHolderMask = HMasks.GetMask<CountersHolderComponent>();

        protected override void Run(IEntity entity)
        {
            entity.GetHECSComponent<CountersHolderComponent>(ref CounterHolderMask)
                .GetCounter<ICounter<float>>(Counter).SetValue(Value);
            
            Next.Execute(entity);
        }
    }
}