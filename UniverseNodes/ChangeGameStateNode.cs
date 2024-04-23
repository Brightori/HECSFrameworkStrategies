using HECSFramework.Core;


namespace Strategies
{
    [Documentation(Doc.Strategy, Doc.HECS, "ChangeGameStateNode")]
    public class ChangeGameStateNode : InterDecision
    {
        [Connection(ConnectionPointType.In, "<int> Game State")]
        public GenericNode<int> ToState;

        public override string TitleOfNode { get; } = "ChangeGameStateNode";

        protected override void Run(Entity entity)
        {
            entity.World.Command(new Commands.ForceGameStateTransitionGlobalCommand { GameState = ToState.Value(entity) });
            Next.Execute(entity);
        }
    }
}
