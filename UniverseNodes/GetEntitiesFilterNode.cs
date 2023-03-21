﻿using System;
using HECSFramework.Core;

namespace Strategies
{
    [Documentation(Doc.HECS, Doc.Strategy, "this strategy node set filter of entites, here we can setup include and exclude filters for entities filters")]
    public sealed class GetEntitiesFilterNode : GenericNode<EntitiesFilter>
    {
        [DrawEntitiesFilter]
        public Filter Include;

        [DrawEntitiesFilter]
        public Filter Exclude;

        public override string TitleOfNode { get; } = "GetEntitiesFilterNode";

        [Connection(ConnectionPointType.Out, "Out <EntitiesFilter>")]
        public BaseDecisionNode BaseDecisionNode;

        public override void Execute(Entity entity)
        {
        }

        public override EntitiesFilter Value(Entity entity)
        {
            return entity.World.GetFilter(Include, Exclude);
        }
    }
}