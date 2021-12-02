using Cysharp.Threading.Tasks;
using HECSFramework.Core;
using HECSFramework.Documentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Strategies
{
    public class WaitStateNode : InterDecision, IInitable
    {
        [SerializeField] public float WaitTime = 1;
        private int waitForInMs;

        protected override async void ExecuteState(IEntity entity)
        {
            await UniTask.Delay(waitForInMs);
            next.Execute(entity);
        }

        //private async Task Wait(float )

        public override string TitleOfNode { get; } = "Wait";

        public void Init()
        {
            waitForInMs = (int)(WaitTime * 1000);
        }
    }
}