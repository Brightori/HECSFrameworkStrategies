using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Components;
using HECSFramework.Core;

namespace Strategies
{
    public sealed class SetWaitNode : SetNode
    {
        public override string TitleOfNode { get; } = "SetWaitNode by float";

        [Connection(ConnectionPointType.In, "<float> In")]
        public GenericNode<float> Time;

        protected override void Run(IEntity entity)
        {
            entity.GetOrAddComponent<WaitStateComponent>().CurrentWaitTimer = Time.Value(entity); ;
        }
    }
}
