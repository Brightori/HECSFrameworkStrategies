using HECSFramework.Core;
using HECSFramework.Documentation;
using Strategies;
using System;

namespace Components
{
    [Serializable, Documentation(Doc.Tag, Doc.AI, Doc.State, "Это компонент в котором мы храним контекст относительно стейта")]
    public class StateContextComponent : BaseComponent
    {
        public StrategyState StrategyState = StrategyState.Run;
        public StateDataComponent StateHolder;
    }
}