using Components;
using HECSFrameWork;
using HECSFrameWork.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Reflection.Emit;
using Loggers;

[CreateAssetMenu]
public class Strategy : ScriptableObject
{
    public List<BaseDecisionNode> nodes = new List<BaseDecisionNode>(16);

    private BaseDecisionNode start;

    public void StartStrategy(IEntity entity)
    {
        UnityLogger.Log(LoggerCategory.AI, "Strategy inside start==>>>>>>> " + name);

        if (entity.IsHaveComponents(ComponentID.UntilSuccessStrategyNodeComponentID))
        {
            entity[ComponentID.UntilSuccessStrategyNodeComponentID].As<IUntilSuccessStrategyNodeComponent>().BaseDecisionNode.Execute(entity);
            return;
        }

        if (start == null)
            start = nodes.FirstOrDefault(x => x is StartDecision);

        if (start == null)
        {
            Debug.LogAssertion("нет стартовой ноды у "+ this.name);
            return;
        }

        start.Execute(entity);
    }
}

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
        if (entity.IsHaveComponents(ComponentID.UntilSuccessStrategyNodeComponentID))
            LocalExecute(entity);
        else
        {
            entity.AddHecsComponent(new UntilSuccessStrategyNodeComponent { BaseDecisionNode = this });
            LocalExecute(entity);
        }
    }

    protected void Success(IEntity entity)
    {
        entity.RemoveHecsComponent(ComponentID.UntilSuccessStrategyNodeComponentID);
        whenSuccess.Execute(entity);
    }

    public abstract void LocalExecute(IEntity entity);
}

public abstract class InterDecision : BaseDecisionNode
{
    [Connection(ConnectionPointType.In, "Input")] public BaseDecisionNode parent;
    [Connection(ConnectionPointType.Link, "Next")] public BaseDecisionNode next;
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