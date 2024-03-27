using Components;
using HECSFramework.Core;

namespace Strategies
{
    [Documentation(Doc.Strategy, "GetIntCounterNode")]
    public sealed partial class GetIntCounterNode : GenericNode<int>
    {
        public override string TitleOfNode { get; } = "GetIntCounterNode";

        [Connection(ConnectionPointType.In, " <Entity> Additional Entity")]
        public GenericNode<Entity> AdditionalEntity;

        [Connection(ConnectionPointType.Out, " <int> Out")]
        public BaseDecisionNode Out;

        [DropDownIdentifier("CounterIdentifierContainer")]
        public int CounterIdentifier;

        public override void Execute(Entity entity)
        {
        }

        public override int Value(Entity entity)
        {
            var neededEntity = AdditionalEntity != null ? AdditionalEntity.Value(entity) : entity;
            return neededEntity.GetComponent<CountersHolderComponent>().GetCounter<ICounter<int>>(CounterIdentifier).Value;
        }
    }
}
