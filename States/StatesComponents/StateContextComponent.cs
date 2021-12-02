using HECSFramework.Core;
using HECSFramework.Documentation;
using Strategies;
using System;

namespace Components
{
    [Serializable, Documentation(Doc.Tag, Doc.AI, Doc.State, "Это компонент в котором мы храним контекст относительно стейта")]
    public class StateContextComponent : BaseComponent
    {
        public StrategyState State = StrategyState.Run;
        public Guid StateGuid;
    }
}