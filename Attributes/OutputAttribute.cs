using System;

namespace Strategies
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public sealed class OutputAttribute : Attribute
    {
        public readonly string Name;

        public OutputAttribute(string name)
        {
            Name = name;
        }
    }
}
