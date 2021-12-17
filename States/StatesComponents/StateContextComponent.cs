using HECSFramework.Core;
using HECSFramework.Documentation;
using Strategies;
using System;

namespace Components
{
    [Serializable, Documentation(Doc.Tag, Doc.AI, Doc.State, "Это компонент в котором мы храним контекст относительно стейта")]
    public class StateContextComponent : BaseComponent, IDisposable
    {
        public StrategyState StrategyState = StrategyState.Run;
        
        public StateDataComponent StateHolder;
        public StateDataComponent PreviousStateHolder;
        
        public SetStateNode ExitStateNode;
        
        public void ExitFromState()
        {
            StateHolder = null;
            StrategyState = StrategyState.Stop;
            ExitStateNode = null;
        }

        public void Dispose()
        {
            ExitFromState();
        }
    }
}