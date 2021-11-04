using UnityEngine;

namespace Strategies
{
    public abstract class FinalDecision : BaseDecisionNode
    {
        [HideInInspector]
        [Connection(ConnectionPointType.In, "Input")] 
        public BaseDecisionNode node;
    }
}