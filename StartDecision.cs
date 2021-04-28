using HECSFramework.Core;

public class StartDecision : BaseDecisionNode
{
    [Connection(ConnectionPointType.Link, "Start of Strategy")] public BaseDecisionNode startDecision;

    public override string TitleOfNode => "Start";

    public override void Execute(IEntity entity)
    {
        startDecision.Execute(entity);
    }
}
