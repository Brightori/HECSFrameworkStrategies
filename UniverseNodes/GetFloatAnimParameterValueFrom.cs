using System;
using Components;
using HECSFramework.Core;
using Strategies;

public sealed class GetFloatAnimParameterValueFrom : GenericNode<float>
{
    public override string TitleOfNode { get; } = "Get Float AnimParameter";

    [Connection(ConnectionPointType.Out, "<float> out")]
    public BaseDecisionNode Connect;

    [AnimParameterDropDown]
    public int AnimIdentifer;

    [NonSerialized]
    private HECSMask animatorStateComponentMask = HMasks.GetMask<AnimatorStateComponent>();

    public override void Execute(IEntity entity)
    {
    }

    public override float Value(IEntity entity)
    {
        if (entity.TryGetHecsComponent(animatorStateComponentMask, out AnimatorStateComponent animatorStateComponent))
        {
            if (animatorStateComponent.State.TryGetFloat(AnimIdentifer, out var parameter))
            {
                return parameter.Value;
            }
        }

        return 0;
    }
}
