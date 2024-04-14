using Components;
using HECSFramework.Core;

namespace Strategies
{
    [Documentation(Doc.HECS, Doc.Strategy, "SetOrAddIntCounterNode")]
    public sealed class SetOrAddIntCounterNode : InterDecision
    {
        public override string TitleOfNode { get; } = "Set or Add IntCounter";

        [DropDownIdentifier("CounterIdentifierContainer")]
        public int Counter;

        [ExposeField]
        public int Value;

        protected override void Run(Entity entity)
        {
            entity.GetComponent<CountersHolderComponent>()
                .GetOrAddIntCounter(Counter).SetValue(Value);

            Next.Execute(entity);
        }
    }
}