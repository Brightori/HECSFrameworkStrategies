using Components;
using HECSFramework.Core;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HECSFramework.Documentation;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Strategies
{
    [CreateAssetMenu]
    [Documentation(Doc.GameLogic, "Это корневой объект для визуального редактора Decision Tree")]
    public class Strategy : ScriptableObject
    {
        private int indexCash = -1;

        public Strategy GetCopy => Instantiate(this);

        public int StrategyIndex
        {
            get
            {
                if (indexCash == -1)
                    indexCash = IndexGenerator.GenerateIndex(name);

                return indexCash;
            }
        }

#if UNITY_EDITOR //это для проброса в эдитор
        public static event Action<Strategy, string> GetWindow;
#endif
        public List<BaseDecisionNode> nodes = new List<BaseDecisionNode>(16);
        private BaseDecisionNode start;

        public void Execute(IEntity entity)
        {
            if (start == null)
                start = nodes.FirstOrDefault(x => x is StartDecision);

            if (start == null)
            {
                Debug.LogAssertion("нет стартовой ноды у " + this.name);
                return;
            }

            start.Execute(entity);
        }

#if UNITY_EDITOR
        [Button(ButtonSizes.Large)]
        private void OpenStrategy()
        {
            var path = AssetDatabase.GetAssetPath(this);
            GetWindow.Invoke(this, path);
        }
#endif
    }

    public abstract class BaseDecisionNode : ScriptableObject, IDecisionNode
    {
        public abstract string TitleOfNode { get; }
        [HideInInspector] public Vector2 coords;

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