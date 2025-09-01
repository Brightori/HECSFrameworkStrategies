using Components;
using Cysharp.Threading.Tasks;
using HECSFramework.Core;
using Strategies;

[Documentation(Doc.Strategy, Doc.UniversalNodes, "WaitFloatGenericNode")]
public abstract class AsyncNode : InterDecision
{
    protected override async void Run(Entity entity)
    {
        bool onPause = false;

        if (!entity.TryGetComponent(out StateContextComponent stateContextComponent))
            return;

        var currentIteration = stateContextComponent.CurrentIteration;

        if (stateContextComponent.StrategyState == StrategyState.Run)
        {
            onPause = true;
            stateContextComponent.StrategyState = StrategyState.Pause;
        }

        var alive = new AliveEntity(entity);
        var strategyIndex = stateContextComponent.CurrentStrategyIndex;

        await WaitExectute(entity);

        if (!alive.IsAlive)
            return;

        if (strategyIndex != stateContextComponent.CurrentStrategyIndex)
            return;

        if (currentIteration != stateContextComponent.CurrentIteration)
            return;

        if (onPause)
            stateContextComponent.StrategyState = StrategyState.Run;

        Next.Execute(entity);
    }

    public abstract UniTask WaitExectute(Entity entity);
}