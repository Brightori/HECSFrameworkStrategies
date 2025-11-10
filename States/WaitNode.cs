using Components;
using HECSFramework.Core;
using UnityEngine;

namespace Strategies
{
    public class WaitNode : InterDecision
    {
        public override string TitleOfNode { get; } = "Wait";

        [SerializeField] public float WaitTime = 1;
        [SerializeField] public float MaxWaitTime = 1;

        protected override async void Run(Entity entity)
        {
            if (!entity.TryGetComponent(out StateContextComponent stateContextComponent))
                return;

            stateContextComponent.StrategyState = StrategyState.Pause;
            var randomTime = Random.Range(WaitTime, MaxWaitTime);
            var alive = entity.GetAliveEntity();
            var gen = stateContextComponent.CurrentIteration;

            await new Wait(randomTime).RunJob(entity.World);

            if (!alive.IsAlive)
                return;

            if (stateContextComponent.StrategyState != StrategyState.Pause || stateContextComponent.CurrentIteration != gen)
                return;

            stateContextComponent.StrategyState = StrategyState.Run;
            Next.Execute(entity);
        }
    }

    public struct Wait : IHecsJob
    {
        public float RemainingTime;

        public Wait(float remainingTime)
        {
            RemainingTime = remainingTime;
        }

        public void Run()
        {
            RemainingTime -= Time.deltaTime;
        }

        public bool IsComplete()
        {
            return RemainingTime <= 0;
        }
    }
}