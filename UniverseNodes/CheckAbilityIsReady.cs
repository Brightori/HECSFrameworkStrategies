using System;
using Components;
using HECSFramework.Core;
using Strategies;

[Documentation(Doc.Strategy, Doc.HECS, "here we check predicates of needed ability")]
public sealed class CheckAbilityIsReady : DilemmaDecision
{
    [Connection(ConnectionPointType.In, "<IEntity> Target")]
    public GenericNode<IEntity> Target;
    public override string TitleOfNode { get; } = "Check Ability Is Ready";
    
    [NonSerialized]    
    private HECSMask predicatesMask = HMasks.GetMask<AbilityPredicateComponent>();

    [NonSerialized]
    private HECSMask abilitiesHolderMask = HMasks.GetMask<AbilitiesHolderComponent>();

    [AbilityIDDropDown]
    public int AbilityIndex;

    protected override void Run(IEntity entity)
    {
        if (entity.TryGetHecsComponent(abilitiesHolderMask, out AbilitiesHolderComponent abilitiesHolderComponent))
        {
            if (abilitiesHolderComponent.IndexToAbility.TryGetValue(AbilityIndex, out var ability))
            {
                if (ability.TryGetHecsComponent(predicatesMask, out AbilityPredicateComponent predicatesComponent))
                {
                    if (!predicatesComponent.TargetPredicates.IsReady(Target?.Value(entity), ability))
                    {
                        Negative.Execute(entity);
                        return;
                    }

                    if (!predicatesComponent.AbilityPredicates.IsReady(ability))
                    {
                        Negative.Execute(entity);
                        return;
                    }

                    if (!predicatesComponent.AbilityOwnerPredicates.IsReady(entity, Target?.Value(entity)))
                    {
                        Negative.Execute(entity);
                        return;
                    }
                }
            }
        }

        Positive.Execute(entity);
    }
}