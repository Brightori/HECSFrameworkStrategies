using HECSFramework.Core;
using UnityEngine;

namespace Strategies
{
    public class DebugDecision : InterDecision
    {
        [SerializeField] public string DebugMessage = "Debug";
        public override string TitleOfNode { get; } = "Debug Node";

        protected override void Run(IEntity entity)
        {
#if UNITY_EDITOR
            HECSDebug.Log(DebugMessage);
#endif
            Next.Execute(entity);
        }
    }
}