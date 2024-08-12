using HECSFramework.Core;

namespace Systems
{
    public interface IStrategyOnInit
    {
        void Execute(Entity entity);
    }
}