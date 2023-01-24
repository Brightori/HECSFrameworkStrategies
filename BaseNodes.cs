using HECSFramework.Core;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Strategies
{
    public abstract class BaseDecisionNode : ScriptableObject, IDecisionNode
    {
        public abstract string TitleOfNode { get; }
        [IgnoreDraw] public Vector2 coords;

        public abstract void Execute(Entity entity);
    }

    public abstract class InterDecision : LogNode
    {
        [Connection(ConnectionPointType.In, "Input")] [IgnoreDraw] public BaseDecisionNode parent;
        [Connection(ConnectionPointType.Out, "Next")] [IgnoreDraw] public BaseDecisionNode Next;
    }

    public abstract class SetNode : LogNode
    {
        [Connection(ConnectionPointType.Out, "Set")][IgnoreDraw] public BaseDecisionNode Set;
    }

    public abstract class DilemmaDecision : LogNode
    {
        [Connection(ConnectionPointType.In, "Input")] public BaseDecisionNode Input;
        [Connection(ConnectionPointType.Out, "Positive")] public BaseDecisionNode Positive;
        [Connection(ConnectionPointType.Out, "Negative")] public BaseDecisionNode Negative;
    }

    public interface IDecisionNode
    {
        void Execute(Entity entity);
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

    public enum ConnectionPointType { In, Out }
}