using System;
using Components;
using HECSFramework.Core;

namespace Systems
{
    [Serializable][Documentation(Doc.Strategy, Doc.HECS, "this system execute strategy on after init ")]
    public sealed class StrategyOnInitSystem : BaseSystem, IAfterEntityInit 
    {
        [Required]
        public StrategyOnInitComponent StrategyOnInitComponent;

        public void AfterEntityInit()
        {
            StrategyOnInitComponent.Strategy.Execute(Owner);
        }

        public override void InitSystem()
        {
        }
    }
}