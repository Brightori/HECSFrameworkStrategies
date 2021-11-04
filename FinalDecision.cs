using UnityEngine;

namespace Strategies
{
    public abstract class FinalDecision : BaseDecisionNode
    {
        [Connection(ConnectionPointType.In, "Input"), HideInInspector] public BaseDecisionNode node;
    }
}