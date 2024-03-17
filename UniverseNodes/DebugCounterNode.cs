using HECSFramework.Core;namespace Strategies{	[Documentation(Doc.Strategy, "DebugCounterNode")]	public sealed class DebugCounterNode : InterDecision	{		[Connection(ConnectionPointType.In, "<float> FloatIn")]		public GenericNode<float> FloatIn;		[Connection(ConnectionPointType.In, "<int> IntIn")]		public GenericNode<int> IntIn;		[ExposeField]		public string Message = "Counter";		public override string TitleOfNode { get; } = "DebugCounterNode";				protected override void Run(Entity entity)		{			if (FloatIn != null)
				HECSDebug.LogWarning($"{Message} {"Float"} {FloatIn.Value(entity)}");

            if (IntIn != null)
                HECSDebug.LogWarning($"{Message} {"Int"} {IntIn.Value(entity)}");

            Next.Execute(entity);		}	}}