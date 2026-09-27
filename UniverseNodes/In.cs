using System;
using HECSFramework.Core;

namespace Strategies
{
    [Serializable]
    public struct In<T>
    {
        public BaseDecisionNode Node;

        // 1-based to match Value, Value2 .. Value6; 0 (the default) also means the main Value.
        public int Output;

        public string OutputName;

        private Func<Entity, T> getter;
        private BaseDecisionNode resolvedNode;
        private int resolvedOutput;

        public In(BaseDecisionNode node, int output, string outputName)
        {
            Node = node;
            Output = output;
            OutputName = outputName;
            getter = null;
            resolvedNode = null;
            resolvedOutput = 0;
        }

        public bool IsConnected => Node != null;

        public T Value(Entity entity)
        {
            // Unity can rewrite Node/Output in place (undo, editor rewiring during play),
            // so the cached getter is valid only for the pair it was resolved from.
            if (getter == null || !ReferenceEquals(resolvedNode, Node) || resolvedOutput != Output)
                Resolve();

            return getter(entity);
        }

        private void Resolve()
        {
            if (Node == null)
                throw new InvalidOperationException($"In<{typeof(T).Name}> is not connected");

            if (Node is not IValueOutputs outputs)
                throw new InvalidOperationException($"{Node.name} ({Node.GetType().Name}) has no value outputs, In<{typeof(T).Name}> cannot read it");

            var output = outputs.GetOutput(Output);

            if (output is not Func<Entity, T> typed)
                throw new InvalidOperationException($"{Node.name} ({Node.GetType().Name}) output {Output} is {output.Method.ReturnType.Name}, In<{typeof(T).Name}> cannot read it");

            getter = typed;
            resolvedNode = Node;
            resolvedOutput = Output;
        }
    }
}
