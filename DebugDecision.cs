using HECSFramework.Core;
using UnityEngine;

namespace Strategies
{
    [DrawInNode]
    public class DebugDecision : InterDecision
    {
        public bool PrintEntityGUID = false;
        public string DebugMessage = "Debug";
        public override string TitleOfNode { get; } = "Debug Node";

        protected override void Run(IEntity entity)
        {
#if UNITY_EDITOR
            HECSDebug.Log($"{entity.GUID.ToString()} -- {DebugMessage}");
#endif
            Next.Execute(entity);
        }
    }
}