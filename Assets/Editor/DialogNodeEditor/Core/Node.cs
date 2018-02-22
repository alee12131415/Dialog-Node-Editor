using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// not abstract because unity reccommends it
/// </summary>

namespace DNECore {
    public abstract class Node {
        public Rect rect;
        //public string title;
        public GUIStyle style = new GUIStyle();
        public DialogNodeEditor editor;
        public float width;
        public float height;
        public bool isDragged;

        public Node(DialogNodeEditor editor, Vector2 position) {
            SetStyle();
            this.editor = editor;
        }

        public Node(DialogNodeEditor editor, NodeInfo info) {
            SetStyle();
            this.editor = editor;
        }

        public virtual void Drag(Vector2 delta) {
            rect.position += delta;
        }

        public abstract void Init(Vector2 position);

        public abstract void Draw();

        public abstract bool ProcessEvents(Event e);

        public abstract void SetStyle();

        public abstract List<ConnectionPoint> GetConnectionPoints();

        public abstract NodeInfo GetInfo();

        public abstract void Rebuild(List<ConnectionPoint> cp);

        public virtual bool ProcessDefault(Event e) {
            //adds clickdrag
            switch (e.type) {
                case EventType.MouseDown:
                    if (e.button == 0) {
                        if (rect.Contains(e.mousePosition)) {
                            isDragged = true;
                        }
                    }
                    else if (e.button == 1 && rect.Contains(e.mousePosition)) {
                        //delete node
                        GenericMenu genericMenu = new GenericMenu();
                        genericMenu.AddItem(new GUIContent("Remove"), false, () => editor.OnClickRemoveNode(this));
                        genericMenu.ShowAsContext();
                        e.Use();
                    }
                    break;
                case EventType.MouseUp:
                    isDragged = false;
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0 && isDragged) {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
            }

            return false;
        }
    }
}