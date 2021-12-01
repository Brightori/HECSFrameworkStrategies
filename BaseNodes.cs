using Components;
using HECSFramework.Core;
using System;
using UnityEngine;

namespace Strategies
{
    public abstract class BaseDecisionNode : ScriptableObject, IDecisionNode
    {
        public abstract string TitleOfNode { get; }
        public Vector2 coords;

        public abstract void Execute(IEntity entity);
    }

    /// <summary>
    /// общая логика - лочим стратегию на этой ноде, проверяем условие по которому мы считаем что логика успешно завершена, 
    /// и вызываем метод Success
    /// в противном случае дергаем ноду proccess, подразумевается что мы там что то делаем что в итоге приведет к успеху
    /// </summary>
    public abstract class UntilSuccesNode : BaseDecisionNode
    {
        [Connection(ConnectionPointType.In, "Input")] public BaseDecisionNode parent;
        [Connection(ConnectionPointType.Link, "When Success")] public BaseDecisionNode whenSuccess;
        [Connection(ConnectionPointType.Link, "Proccess")] public BaseDecisionNode proccess;

        public override void Execute(IEntity entity)
        {
            if (entity.ContainsMask<UntilSuccessStrategyNodeComponent>())
                LocalExecute(entity);
            else
            {
                entity.AddHecsComponent(new UntilSuccessStrategyNodeComponent { BaseDecisionNode = this });
                LocalExecute(entity);
            }
        }

        protected void Success(IEntity entity)
        {
            entity.RemoveHecsComponent<UntilSuccessStrategyNodeComponent>();
            whenSuccess.Execute(entity);
        }

        public abstract void LocalExecute(IEntity entity);
    }

    public abstract class InterDecision : BaseDecisionNode
    {
        [Connection(ConnectionPointType.In, "Input")] [IgnoreDraw] public BaseDecisionNode parent;
        [Connection(ConnectionPointType.Link, "Next")] [IgnoreDraw] public BaseDecisionNode next;
    }

    public abstract class DilemmaDecision : BaseDecisionNode
    {
        [Connection(ConnectionPointType.In, "Input")] public BaseDecisionNode parent;
        [Connection(ConnectionPointType.Link, "Positive")] public BaseDecisionNode positive;
        [Connection(ConnectionPointType.Link, "Negative")] public BaseDecisionNode negative;
    }

    public interface IDecisionNode
    {
        void Execute(IEntity entity);
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ConnectionAttribute : Attribute
    {
        public ConnectionPointType ConnectionPointType;
        public string NameOfField = "Field";

        public ConnectionAttribute(ConnectionPointType connectionPointType, string nameOfField)
        {
            ConnectionPointType = connectionPointType;
            NameOfField = nameOfField;
        }
    }

    public enum ConnectionPointType { In, Out, Link, InSingle }
}