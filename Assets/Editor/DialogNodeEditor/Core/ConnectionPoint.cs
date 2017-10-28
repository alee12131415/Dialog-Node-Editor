using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace DNECore {
    public enum ConnectionPointType { In, Out }

    public class ConnectionPoint {
        public Rect rect;
        public Node node;
        public ConnectionPointType type;
        public GUIStyle style = new GUIStyle();

        public Action<ConnectionPoint> OnClickConnectionPoint;

        public bool isClicked = false;

        public ConnectionPoint(Node node, ConnectionPointType type, Action<ConnectionPoint> OnClickConnectionPoint) {
            this.node = node;
            this.type = type;
            this.OnClickConnectionPoint = OnClickConnectionPoint;
            SetStyle();
            rect = new Rect(0, 0, 10f, 10f);
        }

        public ConnectionPoint() {
            //for build purposes only
            SetStyle();
            rect = new Rect(0, 0, 10f, 10f);
        }

        public void ProcessEvents(Event e) {
            if (isClicked) {
                if (OnClickConnectionPoint != null) {
                    OnClickConnectionPoint(this);
                }
            }
        }

        public void Draw(float y) {
            rect.y = y;

            switch (type) {
                case ConnectionPointType.In:
                    rect.x = node.rect.x - rect.width + 0f;
                    break;
                case ConnectionPointType.Out:
                    rect.x = node.rect.x + node.rect.width - 0f;
                    break;
            }

            isClicked = GUI.Button(rect, "", style);
        }

        public void Draw() {
            Draw(node.rect.y + (node.rect.height * 0.5f) - (rect.height * 0.5f));
        }

        public void SetStyle() {
            style.normal.background = AssetDatabase.LoadAssetAtPath("Assets/Editor/DialogNodeEditor/Textures/grayTex.png", typeof(Texture2D)) as Texture2D;
            style.active.background = AssetDatabase.LoadAssetAtPath("Assets/Editor/DialogNodeEditor/Textures/grayDarkTex.png", typeof(Texture2D)) as Texture2D;
        }

        public void Rebuild(Node node, ConnectionPointType type, Action<ConnectionPoint> OnClickConnectionPoint) {
            this.node = node;
            this.type = type;
            this.OnClickConnectionPoint = OnClickConnectionPoint;
        }
    }
}