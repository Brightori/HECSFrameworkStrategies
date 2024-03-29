﻿using HECSFramework.Core;

namespace Strategies
{
    [Documentation(Doc.Strategy, Doc.HECS, "AliveEntityCheckAlive")]
    public class AliveEntityCheckAliveAndNotDead : DilemmaDecision
    {
        [Connection(ConnectionPointType.In, "<AliveEntity> AliveEntity")]
        public GenericNode<AliveEntity> AliveEntity;
        public override string TitleOfNode { get; } = "AliveEntityCheckAliveAndNotDead";
        protected override void Run(Entity entity)
        {
            if (AliveEntity.Value(entity).IsAliveAndNotDead())
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
