using System;
using Components;
using HECSFramework.Core;
using Strategies;

[Documentation(Doc.HECS, Doc.Strategy, "This node get base value of counter, if counter doesnt have base value, this node throw exeption")]
public class GetMaxFloatValueFromCounterNode : GenericNode<float>
{
    [Connection(ConnectionPointType.In, "<Entity> Additional Entity")]
    public GenericNode<Entity> GetAdditionalEntity;

    public override string TitleOfNode { get; } = "GetMaxFloatValueFromCounterNode";

    [Connection(ConnectionPointType.Out, "<float> Out")]
    public BaseDecisionNode Out;

    [DropDownIdentifier("CounterIdentifierContainer")]
    public int CounterID = 0;


    public override void Execute(Entity entity)
    {
    }

    public override float Value(Entity entity)
    {
        var needed = GetAdditionalEntity != null ? GetAdditionalEntity.Value(entity) : entity;

        if (needed.TryGetComponent(out CountersHolderComponent countersHolderComponent))
        {
            if (countersHolderComponent.TryGetCounter<ICounter<float>>(CounterID, out var counter))
            {
                if (counter is IMaxValue<float > maxValue)
                    return maxValue.MaxValue;
                else
                {
                    HECSDebug.LogError($"counter id {CounterID} on {entity.ID}");
                    return counter.Value;
                }
            }
        }

        throw new Exception($"{entity.ID} doesnt have counters holder component");
    }
}
