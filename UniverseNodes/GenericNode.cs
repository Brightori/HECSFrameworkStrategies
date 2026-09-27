using System;
using HECSFramework.Core;

namespace Strategies
{
    [NodeTypeAttribite("Generic")]
    [Documentation(Doc.HECS, Doc.Strategy, "this is base for providing value by other node")]
    public abstract class GenericNode<T> : BaseDecisionNode, IGenericNode<T>, IValueOutputs
    {
        public abstract T Value(Entity entity);

        public virtual Delegate GetOutput(int index)
        {
            if (index <= 1)
                return new Func<Entity, T>(Value);

            throw new ArgumentOutOfRangeException(nameof(index), index, $"{GetType().Name} has no output {index}");
        }
    }

    // Non-generic so In<T> can reach any node's outputs without knowing its type arguments;
    // a generic GetOutput<T> would be a generic virtual method, which IL2CPP may not have AOT code for.
    [Documentation(Doc.HECS, Doc.Strategy, "Access to a node's value outputs by 1-based index (1 = Value, 2 = Value2 ..); implemented by every GenericNode, used by In<T>")]
    public interface IValueOutputs
    {
        Delegate GetOutput(int index);
    }

    public delegate T ProxyGet<T>(Entity entity);

    public abstract class GenericProxyNode<T> : GenericNode<T>
    {
        public ProxyGet<T> Proxy;

        public override T Value(Entity entity)
        {
            return Proxy.Invoke(entity);
        }
    }

    public abstract class ConvertNode<T, U>  : GenericNode<U>
    {
        [Connection(ConnectionPointType.In, "In")]
        public GenericNode<T> From;

        [Connection(ConnectionPointType.Out, "Out")]
        public BaseDecisionNode To; 
    }

    public interface IGenericNode<T>
    {
        abstract T Value(Entity entity);
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = true)]
    public class NodeTypeAttribite : Attribute
    {
        public string NodeType;

        public NodeTypeAttribite(string nodeType)
        {
            NodeType = nodeType;
        }
    }
}