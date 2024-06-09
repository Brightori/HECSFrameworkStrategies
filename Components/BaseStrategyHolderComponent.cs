using System;
using HECSFramework.Core;
using Strategies;

namespace Components
{
    [Serializable][Documentation(Doc.Strategy, Doc.Holder, "this is parent for simple strategies holder, when we need hold one strategy to some one way logic purpose")]
    public abstract class BaseStrategyHolderComponent : BaseComponent
    {
        public Strategy Strategy;

        public override void Init()
        {
            Strategy.Init();
        }
    }
}