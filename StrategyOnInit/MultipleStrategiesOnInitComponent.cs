using System;
using HECSFramework.Core;
using Strategies;
using Systems;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Strategy, Doc.HECS, "here we can add default strategies whom will be executed on after init sequentially")]
    public sealed class MultipleStrategiesOnInitComponent : BaseComponent, IStrategyOnInit
    {
        public BaseStrategy[] Strategies = Array.Empty<BaseStrategy>();

        public override void Init()
        {
            foreach (var strategy in Strategies) 
            { 
                strategy.Init();
            }
        }

        public void Execute(Entity entity)
        {
            for (int i = 0; i < Strategies.Length; i++)
            {
                Strategies[i].Execute(Owner);
            }
        }
    }
}