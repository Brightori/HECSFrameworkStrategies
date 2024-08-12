using System;
using Components;
using HECSFramework.Core;

namespace Systems
{
    [Serializable][Documentation(Doc.Strategy, Doc.HECS, "this system execute strategy on after init, and process all components with interface IStrategyOnInit")]
    public sealed class StrategyOnInitSystem : BaseSystem, IAfterEntityInit 
    {
        [Required]
        public StrategyOnInitComponent StrategyOnInitComponent;

        public void AfterEntityInit()
        {
            StrategyOnInitComponent.Strategy.Execute(Owner);

            using var needed = Owner.GetComponentsOfTypePooled<IStrategyOnInit>();

            for (int i = 0; i < needed.Count; i++)
            {
                needed.Items[i].Execute(Owner);
            }
        }

        public override void InitSystem()
        {
        }
    }
}