using UnityEngine;

namespace Strategies
{
    public abstract class FinalDecision : BaseDecisionNode
    {
        [HideInInspector, IgnoreDraw]
        [Connection(ConnectionPointType.In, "Input")] 
        public BaseDecisionNode node;
    }
}