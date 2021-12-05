using HECSFramework.Core;

namespace Strategies
{
    public class SetStateNode : BaseDecisionNode, IInitable
    {
        [Connection(ConnectionPointType.In, "Input")]
        public BaseDecisionNode Input;

        [Connection(ConnectionPointType.Out, "Exit")]
        public BaseDecisionNode Exit;

        [UnityEngine.SerializeField]
        public State State;

        public override string TitleOfNode { get; } = "Set State";

        public override void Execute(IEntity entity)
        {
            State.Execute(entity);
        }

        public void Init()
        {
            State.Init();
            State.AddExitNode(Exit);
        }
    }
}
