using HECSFramework.Core;

namespace Strategies
{
    public class GenericFloatProxy : GenericProxyNode<float>
    {
        public override string TitleOfNode { get; }
        public override void Execute(Entity entity)
        {
        }
    }
}