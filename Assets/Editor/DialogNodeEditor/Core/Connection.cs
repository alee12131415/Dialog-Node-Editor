using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace DNECore {
    public class Connection {
        public ConnectionPoint inPoint;
        public ConnectionPoint outPoint;

        public Action<Connection> OnClickRemoveConnection;

        public bool isClicked = false;

        public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> OnClickRemoveConnection) {
            this.inPoint = inPoint;
            this.outPoint = outPoint;
            this.OnClickRemoveConnection = OnClickRemoveConnection;
        }

        public Connection(Connection c) : this(c.inPoint, c.outPoint, c.OnClickRemoveConnection) {
            //Copy Constructor
        }

        public void Draw() {
            Handles.DrawBezier(
                inPoint.rect.center,
                outPoint.rect.center,
                inPoint.rect.center + Vector2.left * 50f,
                outPoint.rect.center - Vector2.left * 50f,
                Color.white,
                null,
                2f
                );

            isClicked = Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap);
        }

        public void ProcessEvents(Event e) {
            if (isClicked) {
                if (OnClickRemoveConnection != null) {
                    OnClickRemoveConnection(this);
                }
            }
        }
    }
}