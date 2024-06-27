using HECSFramework.Core;

namespace Strategies
{
    [Documentation(Doc.HECS, Doc.UniversalNodes, Doc.Strategy, "this node provide basic math operations with A and B in generic int nodes")]
    public class MathIntOperationsNode : GenericNode<int>
    {
        [Connection(ConnectionPointType.In, "<int> ValueA")]
        public GenericNode<int> ValueA;

        [Connection(ConnectionPointType.In, "<int> ValueB")]
        public GenericNode<int> ValueB;

        [ExposeField]
        public ModifierCalculationType CalculationType;

        [ExposeField]
        public ModifierValueType ValueType;

        [Connection(ConnectionPointType.Out, "<int> Out")]
        public BaseDecisionNode Out;

        public override string TitleOfNode { get; } = "MathIntOperationsNode";

        public override void Execute(Entity entity)
        {
        }

        public override int Value(Entity entity)
        {
            return ModifiersCalculation.GetResult(ValueA.Value(entity), ValueB.Value(entity), CalculationType, ValueType);
        }
    }
}