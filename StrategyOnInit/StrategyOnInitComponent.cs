using System;
using HECSFramework.Core;
using Strategies;
using Systems;

namespace Components
{
    [Feature("StartStrategyOnInit")]
    [RequiredAtContainer(typeof(StrategyOnInitSystem))]
    [Serializable][Documentation(Doc.HECS, Doc.Strategy,  "here we hold strategy to start on init")]
    public sealed class StrategyOnInitComponent : BaseComponent
    {
        public BaseStrategy Strategy;

        public override void Init()
        {
            Strategy.Init();
        }
    }
}