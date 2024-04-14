using System;

namespace Strategies
{

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class MetaNodeAttribute : Attribute
    {
        //we take node from meta node field
        public string MetaField;

        //we connect to inner field of generic 
        public string GenericField;
    }

    [NodeTypeAttribite("Meta")]
    public abstract class MetaNodeBase : BaseDecisionNode
    {
    }

    [Serializable]
    public struct NodeToMetaNode
    {
        public BaseDecisionNode Parent;
        public BaseDecisionNode Child;
        public string FieldName;
    }
}