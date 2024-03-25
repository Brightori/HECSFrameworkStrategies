using System;
using Cysharp.Threading.Tasks;
using HECSFramework.Core;
using Strategies;

namespace Components
{
    [Feature("StartStrategyOnInit")]
    [Serializable][Documentation(Doc.HECS, Doc.Strategy,  "here we hold strategy to start on init")]
    public sealed class StrategyOnInitComponent : BaseComponent
    {
        public BaseStrategy Strategy;

        public override void Init()
        {
            Strategy.Init();
        }

        public override async void AfterInit()
        {
            await UniTask.DelayFrame(1);
            Strategy.Execute(Owner);
        }
    }
}