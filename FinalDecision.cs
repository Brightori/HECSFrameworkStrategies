using UnityEngine;

public abstract class FinalDecision : BaseDecisionNode
{
    [Connection(ConnectionPointType.In, "Input")] public BaseDecisionNode node;
}