using System;
using HECSFramework.Core;

namespace Strategies
{
    [Documentation(Doc.HECS, Doc.Strategy,
        "Names a value output port of a multi-output GenericNode<T1..T6> in the strategy editor. " +
        "Put it on the Value..Value6 override: [Output(\"ammo\")] public override int Value2(Entity entity) => ...; " +
        "The name is the port label; without the attribute the port is labelled by the method name. " +
        "It is also saved into the In<T> slot on connection: when outputs are later reordered in code, the graph editor " +
        "rebinds the slot to the output with the same name on open and logs a warning; a renamed output only logs a warning.")]
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
