namespace Strategies
{
    public abstract class DillemaState : LogNode
    {
        [Connection(ConnectionPointType.In, "Input" )]
        public BaseDecisionNode Input;

        [Connection(ConnectionPointType.Out, "Positive")]
        public BaseDecisionNode Positive;
        [Connection(ConnectionPointType.Out, "Negative")]
        public BaseDecisionNode Negative;
    }
}