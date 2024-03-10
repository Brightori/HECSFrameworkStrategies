using Components;
using HECSFramework.Core;
using UnityEngine;

namespace Strategies
{
    public sealed class SetorAddFloatCounterNode : InterDecision
    {
        public override string TitleOfNode { get; } = "Set or Add FloatCounter";

        [DropDownIdentifier("CounterIdentifierContainer")]
        public int Counter;

        [SerializeField]
        public float Value;

        protected override void Run(Entity entity)
        {
            entity.GetComponent<CountersHolderComponent>()
                .GetOrAddFloatCounter(Counter).SetValue(Value);

            Next.Execute(entity);
        }
    }
}