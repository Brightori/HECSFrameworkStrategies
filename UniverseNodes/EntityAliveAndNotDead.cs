using Components;
using HECSFramework.Core;


namespace Strategies
{
    [Documentation(Doc.Strategy, Doc.HECS, "here we check alive entity or not, and have it isdeadtag or not")]
    public class EntityAliveAndNotDead : DilemmaDecision
    {
        public override string TitleOfNode { get; } = "EntityAliveAndNotDead";

        protected override void Run(Entity entity)
        {
            if (entity.IsAlive() && !entity.ContainsMask<IsDeadTagComponent>())
            {
                Positive.Execute(entity);
                return;
            }
            else
            {
                Negative.Execute(entity);
                return;
            }
        }
    }
}
