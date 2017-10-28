using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using DNECore;

namespace DNECore {
    public class EndNode : Node {
        public ConnectionPoint endPoint;

        public EndNode(DialogNodeEditor editor, Vector2 position) : base(editor, position) {
            Init(position);
        }

        public EndNode(DialogNodeEditor editor, NodeInfo info) : base(editor, info) {
            Init(new Vector2(info.rect.x, info.rect.y));
        }

        public override void Init(Vector2 position) {
            width = 200;
            height = 100;
            rect = new Rect(position.x, position.y, width, height);
            endPoint = new ConnectionPoint(this, ConnectionPointType.In, editor.OnClickInPoint);
        }

        public override void Draw() {
            endPoint.Draw();

            GUI.Box(rect, "", style);
            GUI.Label(rect, "END", style);
        }

        public override bool ProcessEvents(Event e) {
            ProcessDefault(e);
            endPoint.ProcessEvents(e);
            return false;
        }

        public override void SetStyle() {
            style.normal.background = AssetDatabase.LoadAssetAtPath("Assets/Editor/DialogNodeEditor/Textures/redTex.png", typeof(Texture2D)) as Texture2D;

            style.normal.textColor = Color.white;
            style.fontSize = 32;
            style.alignment = TextAnchor.MiddleCenter;
        }

        public override List<ConnectionPoint> GetConnectionPoints() {
            return new List<ConnectionPoint> { endPoint };
        }

        public EndNode Clone() {
            return (EndNode)MemberwiseClone();
        }

        public override NodeInfo GetInfo() {
            return new NodeInfo(GetType().FullName, rect);
        }

        public override void Rebuild(List<ConnectionPoint> cp) {
            endPoint = cp[0];
            endPoint.Rebuild(this, ConnectionPointType.In, editor.OnClickInPoint);
        }
    }
}