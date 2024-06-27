using HECSFramework.Core;

namespace Strategies
{
    public class GenericBoolProxy : GenericProxyNode<bool>
    {
        public override string TitleOfNode { get; }
        public override void Execute(Entity entity)
        {
        }
    }
}