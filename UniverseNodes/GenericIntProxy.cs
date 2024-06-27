using HECSFramework.Core;

namespace Strategies
{
    public class GenericIntProxy : GenericProxyNode<int>
    {
        public override string TitleOfNode { get; }
        public override void Execute(Entity entity)
        {
        }
    }
}