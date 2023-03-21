using Components;
using HECSFramework.Core;

namespace Strategies
{
    [Documentation(Doc.Animation, Doc.Strategy, "here we set animation bool parameter")]
    public class SetAnimationBoolParametr : InterDecision
    {
        [AnimParameterDropDown]
        public int boolWithIdentifier;

        [ExposeField]
        public bool Value = true;

        public override string TitleOfNode { get; } = "Set Animation bool";

        protected override void Run(Entity entity)
        {
            entity.GetComponent<AnimatorStateComponent>().State.SetBool(boolWithIdentifier, Value);
            Next.Execute(entity);
        }
    }
}