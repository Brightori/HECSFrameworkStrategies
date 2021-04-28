using HECSFramework.Core;

public class AbortUntilSuccessNode : InterDecision
{
    public override string TitleOfNode => "Abort until success node";


    public override void Execute(IEntity entity)
    {
        throw new System.NotImplementedException();
    }
}
