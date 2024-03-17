using Commands;
using HECSFramework.Core;namespace Strategies{
    [Documentation(Doc.Strategy, Doc.Abilities, "we use this node when we want execute ability system on straight way")]
    public class ExecuteAbilityByCommandNode : InterDecision
    {
        [Connection(ConnectionPointType.In, "<Entity> Target")]
        public GenericNode<Entity> Target;
        [Connection(ConnectionPointType.In, "<Entity> Owner")]
        public GenericNode<Entity> Owner;

        [ExposeField]
        public bool Enabled = true;

        [ExposeField]
        public bool IgnorePredicates = true;

        public override string TitleOfNode { get; } = "ExecuteAbilityByCommand";
        protected override void Run(Entity entity)
        {
            Owner.Value(entity).Command(new ExecuteAbilityCommand
            {
                Owner = Owner.Value(entity),
                Enabled = this.Enabled,
                IgnorePredicates = this.IgnorePredicates,
                Target = Target.Value(entity)
            });

            Next.Execute(entity);
        }
    }}