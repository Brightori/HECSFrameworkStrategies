using Components;
using Cysharp.Threading.Tasks;
using HECSFramework.Core;
using UnityEngine;

namespace Strategies
{
    public class WaitNode : InterDecision, IInitable
    {
        public override string TitleOfNode { get; } = "Wait";

        [SerializeField] public float WaitTime = 1;
        private int waitForInMs;

        protected override async void Run(IEntity entity)
        {
            if (entity.TryGetHecsComponent(HMasks.StateContextComponent, out StateContextComponent stateContextComponent))
                stateContextComponent.State = StrategyState.Pause;

            await UniTask.Delay(waitForInMs);

            if (entity.TryGetHecsComponent(HMasks.StateContextComponent, out StateContextComponent stateContextComponentAfter))
                stateContextComponentAfter.State = StrategyState.Run;

            next.Execute(entity);
        }

        public void Init()
        {
            waitForInMs = (int)(WaitTime * 1000);
        }
    }
}