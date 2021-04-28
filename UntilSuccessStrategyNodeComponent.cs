using HECSFrameWork;
using HECSFrameWork.Components;
using System;
using UnityEngine;

namespace Components
{
    [Serializable, BluePrint]
    public class UntilSuccessStrategyNodeComponent : IUntilSuccessStrategyNodeComponent
    {
        public IEntity Owner { get; set; }

        public ComponentID TypeID => ComponentID.UntilSuccessStrategyNodeComponentID;

        public BaseDecisionNode BaseDecisionNode { get; set; }

        public void Load(SaveData save)
        {
            JsonUtility.FromJsonOverwrite(save.Data[0] as string, this);
        }

        public SaveData Save()
        {
            return HECSFactory.JsonSaveData(this);
        }
    }

    public interface IUntilSuccessStrategyNodeComponent : IComponent, ISingleComponent, INotSaveble
    {
        BaseDecisionNode BaseDecisionNode { get; set; }
    }
}