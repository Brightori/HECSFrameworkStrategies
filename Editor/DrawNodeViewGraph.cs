using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using HECSFramework.Core.Helpers;
using Strategies;
using HECSFramework.Unity;

public class DrawNodeViewGraph : Node
{
    public string GUID;
    public string Name;
    public BaseDecisionNode InnerNode;
    public Dictionary<Port, (MemberInfo member, Direction direction)> ConnectedPorts = new Dictionary<Port, (MemberInfo member, Direction direction)>();
}

public class StrategyGraphView : GraphView, IDisposable
{
    private readonly Strategy strategy;
    private readonly string path;
    private List<DrawNodeViewGraph> drawNodes = new List<DrawNodeViewGraph>(16);
    private NodeSearchWindow _searchWindow;

    public StrategyGraphView(Strategy strategy, string path)
    {
        styleSheets.Add(Resources.Load<StyleSheet>("StrategiesGraph"));

        SetupZoom(0.01f, 2f);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new FreehandSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
        this.strategy = strategy;
        this.path = path;

        AddElement(EntryPoint(strategy));

        foreach (var dn in strategy.nodes)
        {
            if (dn is StartDecision)
                continue;

            AddNodeToDecision(dn);
        }

        ConnectStrategyNodes();
        graphViewChanged += Changes;
        
        FrameAll();
    }

    public override void AddToSelection(ISelectable selectable)
    {
        base.AddToSelection(selectable);

        if (selectable is DrawNodeViewGraph drawNode)
        {
            EditorGUIUtility.PingObject(drawNode.InnerNode);
            Selection.activeObject = drawNode.InnerNode;
        }
    }

    private GraphViewChange Changes(GraphViewChange graphViewChange)
    {
        if (graphViewChange.movedElements != null)
        {
            foreach (var moved in graphViewChange.movedElements)
            {
                if (moved is DrawNodeViewGraph drawNode)
                    drawNode.InnerNode.coords = drawNode.GetPosition().position;
            }
        }

        if (graphViewChange.edgesToCreate != null)
        {
            foreach (var e in graphViewChange.edgesToCreate)
            {
                var input = NeededNode(e.input, Direction.Input);
                var output = NeededNode(e.output, Direction.Output);

                ((FieldInfo)output.ConnectedPorts[e.output].member).SetValue(output.InnerNode, input.InnerNode);
            }
        }

        if (graphViewChange.elementsToRemove != null)
        {
            foreach (var remove in graphViewChange.elementsToRemove)
            {
                if (remove is Edge edge)
                {
                    foreach (var dn in drawNodes)
                    {
                        foreach (var portInfo in dn.ConnectedPorts.ToArray())
                        {
                            if (portInfo.Key == edge.output && portInfo.Value.direction == Direction.Output)
                            {
                                ((FieldInfo)portInfo.Value.member)?.SetValue(dn.InnerNode, null);
                            }
                        }
                    }
                }

                if (remove is DrawNodeViewGraph node)
                {
                    strategy.nodes.AddOrRemoveElement(node.InnerNode, false);
                    ClearNodes();
                }
            }
        }

        return graphViewChange;
    }

    private DrawNodeViewGraph NeededNode(Port port, Direction direction)
    {
        foreach (var dn in drawNodes)
        {
            foreach (var portInfo in dn.ConnectedPorts)
            {
                if (portInfo.Key == port && portInfo.Value.direction == direction)
                    return dn;
            }
        }

        return null;
    }

    private void ConnectStrategyNodes()
    {
        foreach (var dn in drawNodes)
        {
            foreach (var portinfo in dn.ConnectedPorts)
            {
                if (portinfo.Value.direction == Direction.Input)
                    continue;

                if (portinfo.Value.member is FieldInfo field)
                {
                    var data =  field.GetValue(dn.InnerNode);

                    if (data != null)
                    {
                        foreach (var dnOverall in drawNodes)
                        {
                            if ((BaseDecisionNode)data == dnOverall.InnerNode)
                            {
                                var port = dnOverall.ConnectedPorts.FirstOrDefault(x => x.Value.direction == Direction.Input);
                                LinkNodesTogether(portinfo.Key, port.Key);
                            }
                        }
                    }
                }
            }
        }
    }

    private void LinkNodesTogether(Port outputSocket, Port inputSocket)
    {
        var tempEdge = new Edge()
        {
            output = outputSocket,
            input = inputSocket
        };

        tempEdge?.input.Connect(tempEdge);
        tempEdge?.output.Connect(tempEdge);

        tempEdge?.styleSheets.Add(Resources.Load<StyleSheet>("StrategiesGraph"));
        this.Add(tempEdge);
    }

    private DrawNodeViewGraph EntryPoint(Strategy strategy)
    {
        var node = new DrawNodeViewGraph();
        var startNode = strategy.nodes.FirstOrDefault(x => x is StartDecision);

        if (startNode == null)
        {
            OnClickAddNode(Vector2.zero, typeof(StartDecision));
            startNode = strategy.nodes.FirstOrDefault(x => x is StartDecision);
        }

        node.title = startNode.TitleOfNode;
        node.GUID = Guid.NewGuid().ToString();
        node.InnerNode = startNode;

        node.SetPosition(new Rect(startNode.coords, new Vector2(150, 50)));
        GeneratePorts(node);
        drawNodes.Add(node);
        return node;
    }
   
    private DrawNodeViewGraph AddNodeToDecision(BaseDecisionNode node)
    {
        var drawNode = new DrawNodeViewGraph();

        drawNode.title = node.TitleOfNode;
        drawNode.GUID = Guid.NewGuid().ToString();
        drawNode.InnerNode = node;
        drawNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

        drawNode.SetPosition(new Rect(node.coords, new Vector2(150, 50)));
        GeneratePorts(drawNode);
        GenerateCustomField(drawNode);

        AddElement(drawNode);
        drawNodes.Add(drawNode);
        return drawNode;
    }

    private void GenerateCustomField(DrawNodeViewGraph drawNode)
    {
        var so = new SerializedObject(drawNode.InnerNode);

        if (drawNode.InnerNode != null)
        {
            foreach (var m in drawNode.InnerNode.GetType().GetMembers())
            {
                foreach (var a in m.GetCustomAttributes())
                {
                    if (a is SerializeField)
                    {
                        var field = m as FieldInfo;

                        if (field.FieldType == typeof(int))
                        {
                            var intField = new IntegerField(m.Name + ":");
                            intField.value = (int)(m as FieldInfo).GetValue(drawNode.InnerNode);

                            intField.RegisterValueChangedCallback((evt) => IntChangeReact(evt, m as FieldInfo, drawNode.InnerNode));
                            drawNode.contentContainer.Add(intField);
                        }
                        else if (field.FieldType.BaseType == typeof(Enum))
                        {
                            var enumField = new EnumField(m.Name+":");
                            var property = so.FindProperty(m.Name);
                            enumField.value = (Enum)Enum.ToObject(field.FieldType, field.GetValue(drawNode.InnerNode));
                            enumField.Init((Enum)Enum.ToObject(field.FieldType, field.GetValue(drawNode.InnerNode)));

                            enumField.RegisterValueChangedCallback((evt) => EnumChangeReact(evt, field, drawNode.InnerNode));
                            drawNode.contentContainer.Add(enumField);
                        }
                        else if (field.FieldType == typeof(float))
                        {
                            var floatField = new FloatField(m.Name + ":");
                            floatField.value = (float)field.GetValue(drawNode.InnerNode);
                            floatField.RegisterValueChangedCallback((evt) => FloatFieldReact(evt, field, drawNode.InnerNode));
                            drawNode.contentContainer.Add(floatField);
                        }   
                        else if (field.FieldType == typeof(string))
                        {
                            var floatField = new TextField(m.Name + ":");
                            floatField.value = (string)field.GetValue(drawNode.InnerNode);
                            floatField.RegisterValueChangedCallback((evt) => StringFieldReact(evt, field, drawNode.InnerNode));
                            drawNode.contentContainer.Add(floatField);
                        }
                        else if (field.FieldType == typeof(Vector3))
                        {
                            var floatField = new Vector3Field(m.Name + ":");
                            floatField.value = (Vector3)field.GetValue(drawNode.InnerNode);
                            floatField.RegisterValueChangedCallback((evt) => UpdateField(evt, field, drawNode.InnerNode));
                            drawNode.contentContainer.Add(floatField);
                        }
                        else if (field.FieldType == typeof(bool))
                        {
                            var boolField = new Toggle(m.Name + ":");
                            boolField.value = (bool)field.GetValue(drawNode.InnerNode);
                            boolField.RegisterValueChangedCallback((evt) => BoolFieldReact(evt, field, drawNode.InnerNode));
                            drawNode.contentContainer.Add(boolField);
                        }
                        else
                        {
                            var objectField = new ObjectField(m.Name + ":") { objectType = field.FieldType };
                            var value = (m as FieldInfo).GetValue(drawNode.InnerNode) as UnityEngine.Object;

                            objectField.value = value;
                            objectField.RegisterValueChangedCallback((evt) => SetEntityContainer(evt, m as FieldInfo, drawNode.InnerNode));
                            drawNode.contentContainer.Add(objectField);
                        }
                        
                        drawNode.RefreshPorts();
                        drawNode.RefreshExpandedState();
                    }
                }
            }
        }
    }

    private void UpdateField<T>(ChangeEvent<T> evt, FieldInfo field, BaseDecisionNode innerNode)
    {
        field.SetValue(innerNode, evt.newValue);
    }
 
    private void StringFieldReact(ChangeEvent<string> evt, FieldInfo field, BaseDecisionNode innerNode)
    {
        field.SetValue(innerNode, evt.newValue);
    }

    private void SetEntityContainer(ChangeEvent<UnityEngine.Object> evt, FieldInfo field, BaseDecisionNode baseDecisionNode)
    {
        field.SetValue(baseDecisionNode, evt.newValue);
    }

    private void BoolFieldReact(ChangeEvent<bool> evt, FieldInfo fieldInfo, BaseDecisionNode baseDecisionNode)
    {
        fieldInfo.SetValue(baseDecisionNode, evt.newValue);
    }

    private void IntChangeReact(ChangeEvent<int> evt, FieldInfo fieldInfo, BaseDecisionNode baseDecisionNode)
    {
        fieldInfo.SetValue(baseDecisionNode, evt.newValue);
    }

    private void FloatFieldReact(ChangeEvent<float> evt, FieldInfo fieldInfo, BaseDecisionNode baseDecisionNode)
    {
        fieldInfo.SetValue(baseDecisionNode, evt.newValue);
    }

    private void EnumChangeReact(ChangeEvent<Enum> evt, FieldInfo fieldInfo, BaseDecisionNode baseDecisionNode)
    {
        fieldInfo.SetValue(baseDecisionNode, evt.newValue);
    }

    private void ChangeStrategy(ChangeEvent<UnityEngine.Object> evt, FieldInfo field, BaseDecisionNode baseDecisionNode)
    {
        field.SetValue(baseDecisionNode, evt.newValue);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        var startPortView = startPort;

        ports.ForEach((port) =>
        {
            var portView = port;
            if (startPortView != portView && startPortView.node != portView.node)
                compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    private void GeneratePorts(DrawNodeViewGraph drawNode)
    {
        var members = drawNode.InnerNode.GetType().GetMembers();

        foreach (var m in members)
        {
            var atrs = m.GetCustomAttributes();

            foreach (var a in atrs)
            {
                if (a is ConnectionAttribute connection)
                {
                    switch (connection.ConnectionPointType)
                    {
                        case ConnectionPointType.In:
                            var port = GeneratePort(drawNode, Direction.Input, connection.NameOfField, Port.Capacity.Multi);
                            drawNode.ConnectedPorts.Add(port, (m,Direction.Input));
                            break;
                        case ConnectionPointType.Out:
                        case ConnectionPointType.Link:
                            var port2 = GeneratePort(drawNode, Direction.Output, connection.NameOfField);
                            drawNode.ConnectedPorts.Add(port2, (m, Direction.Output));
                            break;
                    }
                }
            }
        }
    }

    private Port GeneratePort(DrawNodeViewGraph node, Direction direction, string portName, Port.Capacity capacity = Port.Capacity.Single)
    {
        var port = node.InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(BaseDecisionNode));
        port.portName = portName;
        node.outputContainer.Add(port);

        node.RefreshExpandedState();
        node.RefreshPorts();
        return port;
    }

    public void AddSearchWindow(StrategyGraphViewWIndow editorWindow)
    {
        _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        _searchWindow.Configure(editorWindow, this);
        nodeCreationRequest = context =>
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
    }

    public void AddNodeToStrategyGraph(Vector2 position, Type type)
    {
        var newNode = OnClickAddNode(position, type);
        AddNodeToDecision(newNode);
    }

    private BaseDecisionNode OnClickAddNode(Vector2 mousePosition, Type type)
    {
        if (strategy.nodes == null)
            strategy.nodes = new List<BaseDecisionNode>(10);

        var asset = ScriptableObject.CreateInstance(type);
        var parent = AssetDatabase.LoadAssetAtPath<Strategy>(path);
        asset.name = type.ToString();
        AssetDatabase.AddObjectToAsset(asset, parent);
        strategy.nodes.Add(asset as BaseDecisionNode);
        AssetDatabase.SaveAssets();
        (asset as BaseDecisionNode).coords = mousePosition;

        return asset as BaseDecisionNode;
    }

    private void ClearNodes()
    {
        var path = AssetDatabase.GetAssetPath(strategy);
        var allSo = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);

        foreach (var go in allSo)
        {
            if (!strategy.nodes.Contains(go))
            {
                if (go == null)
                    continue;

                AssetDatabase.RemoveObjectFromAsset(go);
                MonoBehaviour.DestroyImmediate(go);
            }
        }

        AssetDatabase.SaveAssets();
    }


    public void Dispose()
    {
        graphViewChanged -= Changes;
    }
}
