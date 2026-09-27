using System;
using System.Collections.Generic;
using System.Linq;
using HECSFramework.Core;
using NUnit.Framework;
using Strategies;
using UnityEngine;

public class StrategyPortsTests
{
    private readonly List<ScriptableObject> created = new List<ScriptableObject>();

    [TearDown]
    public void TearDown()
    {
        foreach (var obj in created)
            UnityEngine.Object.DestroyImmediate(obj);

        created.Clear();
    }

    private T Create<T>() where T : ScriptableObject
    {
        var obj = ScriptableObject.CreateInstance<T>();
        created.Add(obj);
        return obj;
    }

    private static PortInfo Port(Type type, string member) => StrategyPorts.GetPorts(type).Single(x => x.Member.Name == member);

    [Test]
    public void HybridGenericNodeKeepsFlowOutputs()
    {
        var type = typeof(CheckGenericEntityAlive);

        Assert.AreEqual(PortSlot.FlowOut, Port(type, "Positive").Slot);
        Assert.AreEqual(PortSlot.FlowOut, Port(type, "Negative").Slot);
        Assert.AreEqual(PortSlot.FlowIn, Port(type, "Input").Slot);
        Assert.AreEqual(PortSlot.ValueOutField, Port(type, "CheckedEntity").Slot);
        Assert.AreEqual(typeof(Entity), Port(type, "CheckedEntity").ValueType);
        Assert.AreEqual(PortSlot.LegacyValueIn, Port(type, "InEntity").Slot);
    }

    [Test]
    public void SplitNodeHasTwoMainValueOutputs()
    {
        foreach (var name in new[] { "A", "B" })
        {
            var port = Port(typeof(SplitEntityNode), name);
            Assert.AreEqual(PortSlot.ValueOutField, port.Slot);
            Assert.AreEqual(1, port.OutputIndex);
            Assert.AreEqual(typeof(Entity), port.ValueType);
        }
    }

    [Test]
    public void MetaNodeFieldIsValueOutput()
    {
        var port = Port(typeof(GetButtonNode), "GameObject");

        Assert.AreEqual(PortSlot.MetaOut, port.Slot);
        Assert.AreEqual(typeof(GameObject), port.ValueType);
    }

    [Test]
    public void OldNodesGetNoMethodPortsAndKeepTheirLabels()
    {
        Assert.IsFalse(StrategyPorts.GetPorts(typeof(IntOutTestNode)).Any(x => x.Slot == PortSlot.MethodOut));
        Assert.AreEqual("<int>", Port(typeof(CompareIntNode), "IntValue").Label);
        Assert.AreEqual(PortSlot.LegacyValueIn, Port(typeof(CompareIntNode), "IntValue").Slot);
    }

    [Test]
    public void MultiOutputNodeGetsNamedMethodPorts()
    {
        var outputs = StrategyPorts.GetPorts(typeof(ValueOutputsTests.SixOutputsTestNode)).Where(x => x.Slot == PortSlot.MethodOut).ToList();

        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6 }, outputs.Select(x => x.OutputIndex));
        CollectionAssert.AreEqual(
            new[] { "<string> text", "<int> count", "<float> ratio", "<int> limit", "<bool> flag", "<long> big" },
            outputs.Select(x => x.Label));
    }

    [Test]
    public void InFieldIsTypedInputWithGeneratedLabel()
    {
        var port = Port(typeof(ConsumerTestNode), "Count");

        Assert.AreEqual(PortSlot.InSlot, port.Slot);
        Assert.AreEqual(typeof(int), port.ValueType);
        Assert.AreEqual("<int> Count", port.Label);
    }

    [Test]
    public void CompatibilityFollowsOutputTypeAndIndex()
    {
        var six = Create<ValueOutputsTests.SixOutputsTestNode>();
        var intOut = Create<IntOutTestNode>();
        var consumer = Create<ConsumerTestNode>();
        var sixType = typeof(ValueOutputsTests.SixOutputsTestNode);
        var count = Port(typeof(ConsumerTestNode), "Count");
        var old = Port(typeof(ConsumerTestNode), "Old");
        var flow = Port(typeof(ConsumerTestNode), "Input");

        Assert.IsTrue(StrategyPorts.CanConnect(Port(sixType, "Value2"), six, count, consumer));
        Assert.IsFalse(StrategyPorts.CanConnect(Port(sixType, "Value3"), six, count, consumer));
        Assert.IsFalse(StrategyPorts.CanConnect(Port(sixType, "Value"), six, count, consumer));
        Assert.IsFalse(StrategyPorts.CanConnect(Port(sixType, "Value2"), six, old, consumer));
        Assert.IsFalse(StrategyPorts.CanConnect(Port(sixType, "Value2"), six, flow, consumer));
        Assert.IsTrue(StrategyPorts.CanConnect(Port(typeof(IntOutTestNode), "Out"), intOut, old, consumer));
        Assert.IsTrue(StrategyPorts.CanConnect(Port(typeof(IntOutTestNode), "Out"), intOut, count, consumer));
    }

    [Test]
    public void ConnectingInSlotWritesNodeIndexAndNameAndRestores()
    {
        var six = Create<ValueOutputsTests.SixOutputsTestNode>();
        var consumer = Create<ConsumerTestNode>();
        var strategy = Create<Strategy>();
        strategy.nodes.Add(six);
        strategy.nodes.Add(consumer);
        var output = Port(typeof(ValueOutputsTests.SixOutputsTestNode), "Value2");

        StrategyPorts.Connect(output, six, Port(typeof(ConsumerTestNode), "Count"), consumer);

        Assert.AreSame(six, consumer.Count.Node);
        Assert.AreEqual(2, consumer.Count.Output);
        Assert.AreEqual("count", consumer.Count.OutputName);
        Assert.AreEqual(2, consumer.Count.Value(null));

        var edge = StrategyPorts.RestoreEdges(strategy).Single();
        Assert.IsTrue(edge.IsDrawn);
        Assert.AreSame(six, edge.Provider);
        Assert.AreSame(output, edge.Out);
    }

    [Test]
    public void OldConnectionKeepsBacklinkAndRestoresOnTheSamePort()
    {
        var split = Create<SplitEntityNode>();
        var check = Create<CheckGenericEntityAlive>();
        var strategy = Create<Strategy>();
        strategy.nodes.Add(split);
        strategy.nodes.Add(check);
        var outputB = Port(typeof(SplitEntityNode), "B");

        StrategyPorts.Connect(outputB, split, Port(typeof(CheckGenericEntityAlive), "InEntity"), check);

        Assert.AreSame(check, split.B);
        Assert.AreSame(split, check.InEntity);
        Assert.AreSame(outputB, StrategyPorts.RestoreEdges(strategy).Single().Out);
    }

    [Test]
    public void MultiOutputMainValueFeedsOldSlot()
    {
        var node = Create<IntFloatTestNode>();
        var consumer = Create<ConsumerTestNode>();
        var strategy = Create<Strategy>();
        strategy.nodes.Add(node);
        strategy.nodes.Add(consumer);
        var main = Port(typeof(IntFloatTestNode), "Value");

        StrategyPorts.Connect(main, node, Port(typeof(ConsumerTestNode), "Old"), consumer);

        Assert.AreEqual(5, consumer.Old.Value(null));
        Assert.AreSame(main, StrategyPorts.RestoreEdges(strategy).Single().Out);
    }

    [Test]
    public void DisconnectResetsInSlot()
    {
        var six = Create<ValueOutputsTests.SixOutputsTestNode>();
        var consumer = Create<ConsumerTestNode>();
        var count = Port(typeof(ConsumerTestNode), "Count");
        var output = Port(typeof(ValueOutputsTests.SixOutputsTestNode), "Value4");

        StrategyPorts.Connect(output, six, count, consumer);
        StrategyPorts.Disconnect(count, consumer, output, six);

        Assert.IsFalse(consumer.Count.IsConnected);
    }

    [Test]
    public void DisconnectingFlowWireClearsTheOutputFieldToo()
    {
        var from = Create<CheckGenericEntityAlive>();
        var to = Create<CheckGenericEntityAlive>();
        var positive = Port(typeof(CheckGenericEntityAlive), "Positive");
        var input = Port(typeof(CheckGenericEntityAlive), "Input");

        StrategyPorts.Connect(positive, from, input, to);
        Assert.AreSame(to, from.Positive);

        StrategyPorts.Disconnect(input, to, positive, from);

        Assert.IsNull(to.Input);
        Assert.IsNull(from.Positive);
    }

    private (ValueOutputsTests.SixOutputsTestNode six, ConsumerTestNode consumer, Strategy strategy) SlotFixture()
    {
        var six = Create<ValueOutputsTests.SixOutputsTestNode>();
        var consumer = Create<ConsumerTestNode>();
        var strategy = Create<Strategy>();
        strategy.nodes.Add(six);
        strategy.nodes.Add(consumer);
        return (six, consumer, strategy);
    }

    [Test]
    public void ReconcileRebindsSlotToOutputWithTheSameName()
    {
        var (six, consumer, strategy) = SlotFixture();
        consumer.Count = new In<int>(six, 4, "count");

        var messages = StrategyPorts.ReconcileOutputNames(strategy);

        Assert.AreEqual(2, consumer.Count.Output);
        Assert.AreEqual(2, consumer.Count.Value(null));
        StringAssert.Contains("slot rebound", messages.Single());
    }

    [Test]
    public void ReconcileKeepsSlotWhenNameIsGone()
    {
        var (six, consumer, strategy) = SlotFixture();
        consumer.Count = new In<int>(six, 2, "bullets");

        var messages = StrategyPorts.ReconcileOutputNames(strategy);

        Assert.AreEqual(2, consumer.Count.Output);
        StringAssert.Contains("not found", messages.Single());
    }

    [Test]
    public void ReconcileLeavesMatchingSlotAlone()
    {
        var (six, consumer, strategy) = SlotFixture();
        consumer.Count = new In<int>(six, 2, "count");

        CollectionAssert.IsEmpty(StrategyPorts.ReconcileOutputNames(strategy));
        Assert.AreEqual(2, consumer.Count.Output);
    }

    [Test]
    public void ReconcileIgnoresSameNameOfIncompatibleType()
    {
        var (six, consumer, strategy) = SlotFixture();
        consumer.Count = new In<int>(six, 2, "ratio");

        var messages = StrategyPorts.ReconcileOutputNames(strategy);

        Assert.AreEqual(2, consumer.Count.Output);
        StringAssert.Contains("not found", messages.Single());
    }

    public class ConsumerTestNode : BaseDecisionNode
    {
        public override string TitleOfNode { get; } = "ConsumerTestNode";

        [Connection(ConnectionPointType.In, "Count")]
        public In<int> Count;

        [Connection(ConnectionPointType.In, "<int> Old")]
        public GenericNode<int> Old;

        [Connection(ConnectionPointType.In, "Input")]
        public BaseDecisionNode Input;

        public override void Execute(Entity entity) { }
    }

    public class IntOutTestNode : GenericNode<int>
    {
        public override string TitleOfNode { get; } = "IntOutTestNode";

        [Connection(ConnectionPointType.Out, "<int> Out")]
        public BaseDecisionNode Out;

        public override void Execute(Entity entity) { }

        public override int Value(Entity entity) => 3;
    }

    public class IntFloatTestNode : GenericNode<int, float>
    {
        public override string TitleOfNode { get; } = "IntFloatTestNode";

        public override void Execute(Entity entity) { }

        [Output("count")] public override int Value(Entity entity) => 5;
        [Output("ratio")] public override float Value2(Entity entity) => 0.5f;
    }
}
