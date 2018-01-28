using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using System;
using DNECore;
using DNE;

/// <summary>
/// todo:
/// - zoom
/// 
/// known bugs:
/// - Moving window fast offsets everything (does not change functionality seems like)
/// - canvas grid draw incomplete sometimes (bottom and right)
/// </summary>

public class DialogNodeEditor : EditorWindow {

    private List<Node> nodes;
    private List<Connection> connections;

    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;

    private Vector2 offset;
    private Vector2 drag;

    private Toolbar toolbar;

	[MenuItem("Dialog Node Editor/Canvas")]
    private static void OpenWindow() {
        DialogNodeEditor window = GetWindow<DialogNodeEditor>();
        window.titleContent = new GUIContent("Dialog Node Editor");
    }

    #region Core Loop
    private void OnEnable() {
        toolbar = new Toolbar(this);
        //StartNode t = (StartNode)nodes.Find(item => item.GetType() == typeof(StartNode));
    }

    private void OnGUI() {
        //draw then process?

        //////////Draw//////////
        DrawCanvas(20, 0.2f, Color.gray);
        DrawCanvas(100, 0.4f, Color.gray);
        DrawNodes();
        DrawConnections();
        DrawConnectionLine(Event.current);
        DrawToolbar();

        //////////Process//////////
        ProcessToolbar(Event.current);
        ProcessConnections(Event.current);
        ProcessNodes(Event.current);
        ProcessCanvas(Event.current);


        ////////?////////
        if (GUI.changed) {
            Repaint();
        }
    }
    #endregion

    #region Draw Functions
    private void DrawNodes() {
        if (nodes != null) {
            for (int i = 0; i < nodes.Count; i++) {
                nodes[i].Draw();
            }
        }
    }

    private void DrawConnections() {
        if (connections != null) {
            for (int i = 0; i < connections.Count; i++) {
                connections[i].Draw();
            }
        }
    }

    private void DrawCanvas(float gridSpacing, float gridOpacity, Color gridColor) {
        int widthdivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthdivs; i++) {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int i = 0; i < heightDivs; i++) {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * i, 0) + newOffset, new Vector3(position.width, gridSpacing * i, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawConnectionLine(Event e) {
        if (selectedInPoint != null && selectedOutPoint == null) {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.magenta,
                null,
                2f
                );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null) {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.magenta,
                null,
                2f
                );

            GUI.changed = true;
        }
    }

    private void DrawToolbar() {
        toolbar.Draw();
    }
    #endregion

    #region Process Functions
    private void ProcessNodes(Event e) {
        if (nodes != null) {
            for (int i = nodes.Count - 1; i >= 0; i--) {
                bool guiChanged = nodes[i].ProcessEvents(e);

                if (guiChanged) {
                    GUI.changed = true;
                }
            }
        }
    }

    private void ProcessConnections(Event e) {
        if (connections != null) {
            for (int i = connections.Count - 1; i >= 0; i--) {
                connections[i].ProcessEvents(e);
            }
        }
    }

    private void ProcessCanvas(Event e) {
        drag = Vector2.zero;
        
        switch(e.type) {
            case EventType.MouseDown:
                if (e.button == 0) {
                    ClearConnectionSelection();
                }
                if (e.button == 1) {
                    ProcessContextMenu(e.mousePosition);
                }
                break;
            case EventType.MouseDrag:
                if (e.button == 0) {
                    OnDrag(e.delta);
                }
                break;
        }
    }

    private void ProcessToolbar(Event e) {
        toolbar.ProcessEvents(e);
    }
    #endregion

    #region Save/Load/Build
    public string SaveCanvas(string path) {
        //TODO better way than try catch block AND change to return bool?
        try {
            if (nodes == null) nodes = new List<Node>();
            if (connections == null) connections = new List<Connection>();
            EditorSaveObject save = BuildSaveObject();
            AssetDatabase.CreateAsset(save, path);
        } catch(Exception e) {
            return e.Message;
        }
        return null;
    }

    public string LoadCanvas(string path) {
        //SAME AS SAVE CANVAS
        try {
            //EditorSaveObject load = Resources.Load(path) as EditorSaveObject;
            EditorSaveObject load = AssetDatabase.LoadAssetAtPath(path, typeof(EditorSaveObject)) as EditorSaveObject;

            //build new CP / Index
            List<ConnectionPoint> CPIndex = new List<ConnectionPoint>();
            for (int i = 0; i < load.NumberOfCP; i++) {
                CPIndex.Add(new ConnectionPoint());
            }

            //build nodes
            int spent = 0; //tracks index of used CP
            nodes = new List<Node>();
            for (int i = 0; i < load.nodeinfos.Count; i++) {
                Type t = Type.GetType(load.nodeinfos[i].type);
                ConstructorInfo ctor = t.GetConstructor(new[] { GetType(), typeof(NodeInfo) });
                Node n = (Node)Convert.ChangeType(ctor.Invoke(new object[] { this, load.nodeinfos[i] }), t);
                n.Rebuild(CPIndex.GetRange(spent, load.NodeCPIndex[i]));
                spent += load.NodeCPIndex[i];
                AddNode(n);
            }

            //build connections
            connections = new List<Connection>();
            for (int i = 0; i < load.ConnectionIndexIn.Count; i++) {
                connections.Add(new Connection(CPIndex[load.ConnectionIndexIn[i]], CPIndex[load.ConnectionIndexOut[i]], RemoveConnection));
            }

            offset = new Vector2(load.offset.x, load.offset.y);
            drag = Vector2.zero;
        } catch (Exception e) {
            return e.Message;
        }
        return null;
    }

    private EditorSaveObject BuildSaveObject() {
        //Build CP array and Node CP index for reference
        List<ConnectionPoint> CPArray = new List<ConnectionPoint>();
        List<int> NodeCPIndex = new List<int>();
        for (int i = 0; i < nodes.Count; i++) {
            List<ConnectionPoint> t = nodes[i].GetConnectionPoints();
            CPArray.AddRange(t);
            NodeCPIndex.Add(t.Count);
        }

        //Build Connection Reference Index
        List<int> ConnectionIndexIn = new List<int>();
        List<int> ConnectionIndexOut = new List<int>();
        for (int i = 0; i < connections.Count; i++) {
            ConnectionIndexIn.Add(CPArray.IndexOf(connections[i].inPoint));
            ConnectionIndexOut.Add(CPArray.IndexOf(connections[i].outPoint));
        }

        //Build Node Info
        List<NodeInfo> nodeinfos = new List<NodeInfo>();
        for (int i = 0; i < nodes.Count; i++) {
            nodeinfos.Add(nodes[i].GetInfo());
        }

        //Return Save Object
        EditorSaveObject save = CreateInstance<EditorSaveObject>();
        save.init(nodeinfos, NodeCPIndex, ConnectionIndexIn, ConnectionIndexOut, CPArray.Count, offset);
        return save;
    }

    public void BuildCanvas(string path) {
        //creates friendly version for loading at runtime
        if (nodes == null) nodes = new List<Node>();
        if (connections == null) connections = new List<Connection>();
        List<BuildNode> buildNodes = new List<BuildNode>();

        //build node array
        List<Node> node_index_reference = new List<Node>();
        for (int i = 0; i < nodes.Count; i++) {
            if (nodes[i].GetType() == typeof(DialogNode)) {
                node_index_reference.Add(nodes[i]);
                NodeInfo temp = nodes[i].GetInfo();
                buildNodes.Add(new BuildNode(temp.title, temp.clip, temp.triggers));
            }
        }
        
        //build next indexes //indices?
        for (int i = 0; i < node_index_reference.Count; i++) {
            for (int j = 0; j < connections.Count; j++) {
                if (connections[j].outPoint.node == node_index_reference[i]) {
                    int index_of_next = node_index_reference.IndexOf(connections[j].inPoint.node);
                    int index_of_trigger = ((DialogNode)node_index_reference[i]).outPoints.IndexOf(connections[j].outPoint);
                    buildNodes[i].next_index[index_of_trigger] = index_of_next;
                }
            }
        }

        //get starting DialogNode
        Node start_n = null;
        for (int i = 0; i < nodes.Count; i++) {
            if (nodes[i].GetType() == typeof(StartNode)) {
                start_n = nodes[i];
            }
        }
        if (start_n == null) {
            Debug.LogError("No Start Node");
            return;
        }
        int starting_index = -1;
        for (int i = 0; i < connections.Count; i++) {
            if (connections[i].outPoint.node == start_n) {
                starting_index = node_index_reference.IndexOf(connections[i].inPoint.node);
            }
        }
        if (starting_index < 0) {
            Debug.LogError("Start Node not connected to anything");
            return;
        }

        BuildObject build = CreateInstance<BuildObject>();
        build.Init(buildNodes, starting_index, starting_index);

        //
        AssetDatabase.CreateAsset(build, path);
    }
    #endregion

    private void AddNode(Node node) {
        if (nodes == null) {
            nodes = new List<Node>();
        }

        //look for start node, reject if start node exists
        if (node.GetType() == typeof(StartNode)) {
            foreach (Node n in nodes) {
                if (n.GetType() == typeof(StartNode)) {
                    toolbar.setMessage("Cannot create more than 1 Start Node");
                    return;
                }
            }
        }

        nodes.Add(node);
    }

    private void ProcessContextMenu(Vector2 mousePosition) {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Start Node"), false, () => AddNode(new StartNode(this, mousePosition)));
        genericMenu.AddItem(new GUIContent("End Node"), false, () => AddNode(new EndNode(this, mousePosition)));
        genericMenu.AddItem(new GUIContent("Dialog Node"), false, () => AddNode(new DialogNode(this, mousePosition)));
        genericMenu.ShowAsContext();
    }

    public void OnClickInPoint(ConnectionPoint inPoint) {
        selectedInPoint = inPoint;

        
        if (selectedOutPoint != null) {
            if (selectedInPoint.node != selectedOutPoint.node) {
                CreateConnection();
                ClearConnectionSelection();
            } else {
                ClearConnectionSelection();
            }
        }
    }

    public void OnClickOutPoint(ConnectionPoint outPoint) {
        selectedOutPoint = outPoint;

        if (selectedInPoint != null) {
            if (selectedOutPoint.node != selectedInPoint.node) {
                CreateConnection();
                ClearConnectionSelection();
            } else {
                ClearConnectionSelection();
            }
        }
    }

    private void CreateConnection() {
        if (connections == null) {
            connections = new List<Connection>();
        }

        //prevents creating the same connection twice
        //bool connectionExists = connections.Any(item => item.inPoint == selectedInPoint && item.outPoint == selectedOutPoint);

        //allows only one connection for each outgoing connection point
        bool out_exists = false;
        for (int i = 0; i < connections.Count; i++) {
            if (connections[i].outPoint == selectedOutPoint) {
                out_exists = true;
            }
        }

        if (/*!connectionExists*/ !out_exists) {
            connections.Add(new Connection(selectedInPoint, selectedOutPoint, RemoveConnection));
        }
    }

    private void RemoveConnection(Connection connection) {
        connections.Remove(connection);
    }

    private void ClearConnectionSelection() {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

    private void OnDrag(Vector2 delta) {
        drag = Vector2.zero + delta;

        if (nodes != null) {
            for (int i = 0; i < nodes.Count; i++) {
                nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    public void OnClickRemoveNode(Node node) {
        if (connections != null) {
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = 0; i < connections.Count; i++) {
                if (connections[i].inPoint.node == node || connections[i].outPoint.node == node) {
                    connectionsToRemove.Add(connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++) {
                connections.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null; //needed?
        }

        nodes.Remove(node);
    }
}
