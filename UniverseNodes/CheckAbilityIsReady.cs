using Components;
using HECSFramework.Core;
using Strategies;

[Documentation(Doc.Strategy, Doc.HECS, "here we check predicates of needed ability, we try to take it from generic node, if entity not provided, we try to take ability from main strategy entity")]
public sealed class CheckAbilityIsReady : DilemmaDecision
{
    [Connection(ConnectionPointType.In, "<Entity> Target")]
    public GenericNode<Entity> Target;

    [Connection(ConnectionPointType.In, "<Entity> Ability Owner")]
    public GenericNode<Entity> AbilityOwner;

    public override string TitleOfNode { get; } = "Check Ability Is Ready";

    [AbilityIDDropDown]
    public int AbilityIndex;

    protected override void Run(Entity entity)
    {
        var abilityOwner = AbilityOwner?.Value(entity);

        if (abilityOwner != null)
        {
            if (!Ready(abilityOwner))
            {
                Negative.Execute(entity);
                return;
            }
        }
        else
        {
            if (!Ready(entity))
            {
                Negative.Execute(entity);
                return;
            }
        }

        Positive.Execute(entity);
    }

    private bool Ready(Entity entity)
    {
        if (entity.TryGetComponent(out AbilitiesHolderComponent abilitiesHolderComponent))
        {
            if (abilitiesHolderComponent.IndexToAbility.TryGetValue(AbilityIndex, out var ability))
            {
                if (ability.TryGetComponent(out AbilityPredicateComponent predicatesComponent))
                {
                    if (!predicatesComponent.TargetPredicates.IsReady(Target?.Value(entity), ability))
                    {
                        return false;
                    }

                    if (!predicatesComponent.AbilityPredicates.IsReady(ability))
                    {
                        return false;
                    }

                    if (!predicatesComponent.AbilityOwnerPredicates.IsReady(entity, Target?.Value(entity)))
                    {
                        return false;
                    }
                }
            }
        }
        
        return true;
    }
}