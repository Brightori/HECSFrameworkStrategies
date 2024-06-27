using Components;
using HECSFramework.Core;

namespace Strategies
{
    [Documentation(Doc.Strategy, "GetIntGenericCounterNode")]
    public sealed partial class GetIntGenericCounterNode : GenericNode<int>
    {
        public override string TitleOfNode { get; } = "GetIntGenericCounterNode";

        [Connection(ConnectionPointType.In, "<Entity> Additional Entity")]
        public GenericNode<Entity> AdditionalEntity;

        [Connection(ConnectionPointType.Out, "<int> Out")]
        public BaseDecisionNode Out;

        [Connection(ConnectionPointType.In, "<int> CounterID")]
        public GenericNode<int> CounterId;

        public override void Execute(Entity entity)
        {
        }

        public override int Value(Entity entity)
        {
            var neededEntity = AdditionalEntity != null ? AdditionalEntity.Value(entity) : entity;
            return neededEntity.GetComponent<CountersHolderComponent>().GetCounter<ICounter<int>>(CounterId.Value(entity)).Value;
        }
    }
}
