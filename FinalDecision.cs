using UnityEngine;

namespace Strategies
{
    [NodeTypeAttribite("FinalNode")]
    public abstract class FinalDecision : LogNode
    {
        [HideInInspector, IgnoreDraw]
        [Connection(ConnectionPointType.In, "Input")] 
        public BaseDecisionNode node;
    }
}