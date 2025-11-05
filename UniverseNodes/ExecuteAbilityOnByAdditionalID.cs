using Commands;
using HECSFramework.Core;

namespace Strategies
{
    [Documentation(Doc.HECS, Doc.Strategy, Doc.UniversalNodes, "we need this node when we want ")]
    public sealed class ExecuteAbilityOnByAdditionalID : InterDecision
    {
        [Connection(ConnectionPointType.In, "<Entity> Ability Owner")]
        public GenericNode<Entity> EntityWithAbility;

        [Connection(ConnectionPointType.In, "<Entity> Target")]
        public GenericNode<Entity> Target;

        [DropDownIdentifier("AdditionalAbilityIdentifier")]
        public int AbilityIndex;

        [ExposeField]
        public bool Enable;

        [ExposeField]
        public bool IgnorePredicates;

        public override string TitleOfNode { get; } = "ExecuteAbilityOnByAdditionalID";

        protected override void Run(Entity entity)
        {
            var abilityOwner = EntityWithAbility != null ? EntityWithAbility.Value(entity) : null;
            var target = Target != null ? Target.Value(entity) : null;


            EntityWithAbility.Value(entity).Command(new ExecuteAbilityByIDCommand
            {
                AbilityIndex = this.AbilityIndex,
                Enable = Enable,
                IgnorePredicates = IgnorePredicates,
                Owner = abilityOwner,
                Target = target,
            });

            Next.Execute(entity);
        }
    }
}