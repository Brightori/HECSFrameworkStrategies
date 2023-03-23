using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Components;
using HECSFramework.Core;

namespace Strategies
{
    public class GetCurrentAnimationLenghtNode : GenericNode<float>
    {
        public override string TitleOfNode { get; } = "GetCurrentAnimationTimeNode";

        [Connection(ConnectionPointType.Out, "<float>")]
        public BaseDecisionNode Out;

        [ExposeField]
        public int AnimationLayer;

        public override void Execute(Entity entity)
        {
        }

        public override float Value(Entity entity)
        {
            if (entity.TryGetComponent(out AnimatorStateComponent animatorStateComponent))
            {
                var currentState = animatorStateComponent.Animator.GetCurrentAnimatorStateInfo(AnimationLayer);
                return currentState.length;
            }
            else
            {
                HECSDebug.LogWarning($"{entity.ID} {entity.GUID} doent have animator state component");
                return 0;
            }
        }
    }
}
