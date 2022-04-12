using HECSFramework.Core;

namespace Strategies
{
    public class DebugDecision : InterDecision
    {
        [UnityEngine.SerializeField] public string DebugMessage = "Debug";
        public override string TitleOfNode { get; } = "Debug Node";

        protected override void Run(IEntity entity)
        {
            HECSDebug.Log(DebugMessage);
            Next.Execute(entity);
        }
    }
}