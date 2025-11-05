using Components;
using HECSFramework.Core;

namespace Strategies
{
    [Documentation(Doc.HECS, Doc.Strategy, Doc.Counters, "this node check - we have this counter id on entity or not")]
    public class IsHaveCounterByIDNode : DilemmaDecision
    {
        [Connection(ConnectionPointType.In, "<Entity> AdditionalEntity")]
        public GenericNode<Entity> AdditionalEntity;

        [Connection(ConnectionPointType.In, "<int> CounterID")]
        public GenericNode<int> CounterID;

        public override string TitleOfNode { get; } = "IsHaveCounterByIDNode";

        protected override void Run(Entity entity)
        {
            var localEntity = AdditionalEntity != null ? AdditionalEntity.Value(entity) : entity;

            if (localEntity.TryGetComponent(out CountersHolderComponent countersHolderComponent))
            {
                if (countersHolderComponent.Counters.ContainsKey(CounterID.Value(entity)))
                {
                    Positive.Execute(entity);
                    return;
                }
            }

            Negative.Execute(entity);
        }
    }
}