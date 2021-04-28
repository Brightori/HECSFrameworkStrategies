using HECSFrameWork.Components;
using Loggers;
using UnityEngine;

public class StartDecision : BaseDecisionNode
{
    [Connection(ConnectionPointType.Link, "Start of Strategy")] public BaseDecisionNode startDecision;

    public override string TitleOfNode => "Start";

    public override void Execute(IEntity entity)
    {
        UnityLogger.Log(LoggerCategory.AI, "Executing " + TitleOfNode);
        startDecision.Execute(entity);
    }
}
