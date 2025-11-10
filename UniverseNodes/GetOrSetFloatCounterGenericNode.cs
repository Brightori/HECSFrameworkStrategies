using Components;
using HECSFramework.Core;
using HECSFramework.Core.Helpers;

namespace Strategies
{
    [Documentation(Doc.HECS, Doc.Strategy, "SetFloatCounterGenericNode")]
    public sealed class GetOrSetFloatCounterGenericNode : InterDecision
    {
        public override string TitleOfNode { get; } = "Get Or Set Float Counter Generic";

        [Connection(ConnectionPointType.In, "<Entity> AdditonalEntity")]
        public GenericNode<Entity> AdditonalEntity;

        [Connection(ConnectionPointType.In, "<int> CounterID")]
        public GenericNode<int> CounterID;

        [Connection(ConnectionPointType.In, "<float> Value")]
        public GenericNode<float> Value;

        protected override void Run(Entity entity)
        {
            var needed = AdditonalEntity != null ? AdditonalEntity.Value(entity) : entity;

            needed.GetComponent<CountersHolderComponent>()
                .GetOrAddFloatCounter(CounterID.Value(entity)).SetValue(Value.Value(entity));

            Next.Execute(entity);
        }
    }
}