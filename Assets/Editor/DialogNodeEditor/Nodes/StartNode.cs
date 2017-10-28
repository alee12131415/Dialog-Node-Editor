using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using DNECore;

namespace DNECore {
    public class StartNode : Node {
        public ConnectionPoint startPoint;

        public StartNode(DialogNodeEditor editor, Vector2 position) : base(editor, position) {
            Init(position);
        }

        public StartNode(DialogNodeEditor editor, NodeInfo info) : base(editor, info) {
            Init(new Vector2(info.rect.x, info.rect.y));
        }

        public override void Init(Vector2 position) {
            width = 200;
            height = 100;
            rect = new Rect(position.x, position.y, width, height);
            startPoint = new ConnectionPoint(this, ConnectionPointType.Out, editor.OnClickOutPoint);
        }

        public override void Draw() {
            startPoint.Draw();

            GUI.Box(rect, "", style);
            GUI.Label(rect, "START", style);
        }

        public override bool ProcessEvents(Event e) {
            ProcessDefault(e);
            startPoint.ProcessEvents(e);
            return false;
        }

        public override void SetStyle() {
            style.normal.background = AssetDatabase.LoadAssetAtPath("Assets/Editor/DialogNodeEditor/Textures/greenTex.png", typeof(Texture2D)) as Texture2D;

            style.normal.textColor = Color.white;
            style.fontSize = 32;
            style.alignment = TextAnchor.MiddleCenter;
        }

        public override List<ConnectionPoint> GetConnectionPoints() {
            return new List<ConnectionPoint> { startPoint };
        }

        public StartNode Clone() {
            return (StartNode)MemberwiseClone();
        }

        public override NodeInfo GetInfo() {
            return new NodeInfo(GetType().FullName, rect);
        }

        public override void Rebuild(List<ConnectionPoint> cp) {
            startPoint = cp[0];
            startPoint.Rebuild(this, ConnectionPointType.Out, editor.OnClickOutPoint);
        }
    }
}