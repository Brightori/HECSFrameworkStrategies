using HECSFramework.Core;


namespace Strategies
{
    [Documentation(Doc.HECS, Doc.UniversalNodes, Doc.Strategy, "this node return is entity alive or not")]
    public class IsEntityAlive : DilemmaDecision
    {
        public override string TitleOfNode { get; } = "IsEntityAlive";

        protected override void Run(Entity entity)
        {
            if (entity.IsAlive())
            {
                Positive.Execute(entity);
                return;
            }
            else
            {
                Negative.Execute(entity);
                return;
            }
        }
    }
}
