using HECSFramework.Core;

namespace Strategies
{
    public class UpdateStateNode : StateNode
    {
        public override string TitleOfNode { get; } = "Update";
        
        [Connection(ConnectionPointType.Out, "Update every tick")]
        public StateNode Update;

        protected override void ExecuteState(IEntity entity)
        {
            Update.Execute(entity);
        }
    }
}