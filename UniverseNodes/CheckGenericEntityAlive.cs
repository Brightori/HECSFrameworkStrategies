using HECSFramework.Core;

namespace Strategies
{
    public class CheckGenericEntityAlive : DilemmaDecision
    {
        [Connection(ConnectionPointType.In, "<Entity>")]
        public GenericNode<Entity> InEntity;

        public override string TitleOfNode { get; } = "CheckEntityAlive";

        protected override void Run(Entity entity)
        {
            if (InEntity.Value(entity).IsAlive())
                Positive.Execute(entity);
            else
                Negative.Execute(entity);
        }
    }
}