using Components;
using HECSFramework.Core;
using HECSFramework.Core.Helpers;

namespace Strategies
{
    [Documentation(Doc.HECS, Doc.Strategy, "SetIntCounterGenericNode")]
    public sealed class GetOrSetIntCounterGenericNode : InterDecision
    {
        public override string TitleOfNode { get; } = "Get Or Set Int Counter Generic";

        [Connection(ConnectionPointType.In, "<Entity> AdditonalEntity")]
        public GenericNode<Entity> AdditonalEntity;

        [Connection(ConnectionPointType.In, "<int> CounterID")]
        public GenericNode<int> CounterID;

        [Connection(ConnectionPointType.In, "<int> Value")]
        public GenericNode<int> Value;

        protected override void Run(Entity entity)
        {
            var needed = AdditonalEntity != null ? AdditonalEntity.Value(entity) : entity;

            needed.GetComponent<CountersHolderComponent>()
                .GetOrAddIntCounter(CounterID.Value(entity)).SetValue(Value.Value(entity));

            Next.Execute(entity);
        }
    }
}