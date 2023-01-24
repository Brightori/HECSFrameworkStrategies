using HECSFramework.Core;

namespace Strategies
{
    [DrawInNode]
    public class DebugDecision : InterDecision
    {
        public bool PrintEntityGUID = false;
        public string DebugMessage = "Debug";
        public override string TitleOfNode { get; } = "Debug Node";

        protected override void Run(Entity entity)
        {
#if UNITY_EDITOR
            HECSDebug.LogWarning($"{entity.GUID.ToString()} -- {DebugMessage}");
#endif
            Next.Execute(entity);
        }
    }
}