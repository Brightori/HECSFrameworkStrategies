using System;
using System.Collections.Generic;
using System.Reflection;
using HECSFramework.Core;
using NUnit.Framework;
using Strategies;
using UnityEngine;

public class ValueOutputsTests
{
    private readonly List<ScriptableObject> created = new List<ScriptableObject>();

    [TearDown]
    public void TearDown()
    {
        foreach (var node in created)
            UnityEngine.Object.DestroyImmediate(node);

        created.Clear();
    }

    private T Create<T>() where T : ScriptableObject
    {
        var node = ScriptableObject.CreateInstance<T>();
        created.Add(node);
        return node;
    }

    [Test]
    public void InReadsSingleOutputNode()
    {
        var node = Create<IntTestNode>();
        node.Result = 7;

        Assert.AreEqual(7, new In<int>(node, 0, null).Value(null));
        Assert.AreEqual(7, new In<int>(node, 1, null).Value(null));
    }

    [Test]
    public void InReadsEveryOutputOfSixOutputNode()
    {
        var node = Create<SixOutputsTestNode>();

        Assert.AreEqual("one", new In<string>(node, 1, "text").Value(null));
        Assert.AreEqual(2, new In<int>(node, 2, "count").Value(null));
        Assert.AreEqual(3f, new In<float>(node, 3, "ratio").Value(null));
        Assert.AreEqual(4, new In<int>(node, 4, "limit").Value(null));
        Assert.AreEqual(true, new In<bool>(node, 5, "flag").Value(null));
        Assert.AreEqual(6L, new In<long>(node, 6, "big").Value(null));
    }

    [Test]
    public void MultiOutputNodeStillFitsOldGenericSlot()
    {
        GenericNode<string> oldSlot = Create<SixOutputsTestNode>();

        Assert.AreEqual("one", oldSlot.Value(null));
    }

    [Test]
    public void ReferenceOutputIsReadableThroughBaseTypeSlot()
    {
        var node = Create<DerivedTestNode>();

        Assert.IsInstanceOf<DerivedValue>(new In<BaseValue>(node, 1, null).Value(null));
    }

    [Test]
    public void TypeMismatchThrowsReadableError()
    {
        var node = Create<SixOutputsTestNode>();

        var error = Assert.Throws<InvalidOperationException>(() => new In<float>(node, 2, "count").Value(null));
        StringAssert.Contains("output 2 is Int32", error.Message);
    }

    [Test]
    public void NotConnectedThrows()
    {
        Assert.IsFalse(new In<int>().IsConnected);
        Assert.Throws<InvalidOperationException>(() => new In<int>().Value(null));
    }

    [Test]
    public void MissingOutputThrows()
    {
        var node = Create<SixOutputsTestNode>();

        Assert.Throws<ArgumentOutOfRangeException>(() => new In<int>(node, 7, null).Value(null));
        Assert.Throws<ArgumentOutOfRangeException>(() => new In<int>(Create<IntTestNode>(), 2, null).Value(null));
    }

    [Test]
    public void RewiringInPlaceDropsCachedGetter()
    {
        var first = Create<IntTestNode>();
        first.Result = 1;
        var second = Create<SixOutputsTestNode>();

        var slot = new In<int>(first, 1, null);
        Assert.AreEqual(1, slot.Value(null));

        slot.Node = second;
        slot.Output = 4;
        Assert.AreEqual(4, slot.Value(null));

        slot.Output = 2;
        Assert.AreEqual(2, slot.Value(null));
    }

    [Test]
    public void OutputNamesAreReadableFromOverrides()
    {
        var type = typeof(SixOutputsTestNode);

        Assert.AreEqual("text", type.GetMethod(nameof(SixOutputsTestNode.Value)).GetCustomAttribute<OutputAttribute>(true).Name);
        Assert.AreEqual("big", type.GetMethod(nameof(SixOutputsTestNode.Value6)).GetCustomAttribute<OutputAttribute>(true).Name);
    }

    public class IntTestNode : GenericNode<int>
    {
        public int Result;

        public override string TitleOfNode { get; } = "IntTestNode";

        public override void Execute(Entity entity) { }

        public override int Value(Entity entity) => Result;
    }

    public class SixOutputsTestNode : GenericNode<string, int, float, int, bool, long>
    {
        public override string TitleOfNode { get; } = "SixOutputsTestNode";

        public override void Execute(Entity entity) { }

        [Output("text")] public override string Value(Entity entity) => "one";
        [Output("count")] public override int Value2(Entity entity) => 2;
        [Output("ratio")] public override float Value3(Entity entity) => 3f;
        [Output("limit")] public override int Value4(Entity entity) => 4;
        [Output("flag")] public override bool Value5(Entity entity) => true;
        [Output("big")] public override long Value6(Entity entity) => 6L;
    }

    public class BaseValue { }

    public class DerivedValue : BaseValue { }

    public class DerivedTestNode : GenericNode<DerivedValue>
    {
        public override string TitleOfNode { get; } = "DerivedTestNode";

        public override void Execute(Entity entity) { }

        public override DerivedValue Value(Entity entity) => new DerivedValue();
    }
}
