using System;
using HECSFramework.Core;

namespace Strategies
{
    [Serializable]
    [Documentation(Doc.Strategy, "this node checks => entity have this component or not")]
    [HECSResolver]
    public class IsHaveComponentNode : DilemmaDecision, IInitable
    {
        public override string TitleOfNode { get; } = "Is Have Component";

        [ComponentMaskDropDown]
        [Field(0, typeof(HECSMaskResolver))]
        public HECSMask ComponentMask;

        protected override void Run(Entity entity)
        {
            if (entity.ContainsMask(ComponentMask.TypeHashCode))
                Positive.Execute(entity);
            else
                Negative.Execute(entity);
        }

        //we can have situation when mask have right typecode but wrong index, and we should validate hecsmask anyway,
        //mby this component not available anymore
        public void Init()
        {
            if (TypesMap.GetComponentInfo(ComponentMask.TypeHashCode, out var mask))
            {
                ComponentMask = mask.ComponentsMask;
            }
            else
                HECSDebug.LogError("is have component node contains wrong mask " + ComponentMask.TypeHashCode);
        }
    }
}