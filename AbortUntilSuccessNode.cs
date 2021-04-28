using HECSFrameWork.Components;
using Loggers;

public class AbortUntilSuccessNode : InterDecision
{
    public override string TitleOfNode => "Abort until success node";

    public override void Execute(IEntity entity)
    {
        UnityLogger.Log(LoggerCategory.AI, "Executing " + TitleOfNode);
        entity.RemoveHecsComponent(ComponentID.UntilSuccessStrategyNodeComponentID);
        next.Execute(entity);
    }
}
