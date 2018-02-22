using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DNECore {
    public class EditorSaveObject : ScriptableObject {
        public List<NodeInfo> nodeinfos;
        public List<int> NodeCPIndex;
        public List<int> ConnectionIndexIn;
        public List<int> ConnectionIndexOut;
        public int NumberOfCP;
        public Vector2 offset;

        public void init(List<NodeInfo> nodeinfos, List<int> NodeCPIndex, List<int> ConnectionIndexIn, List<int> ConnectionIndexOut, int NumberOfCP, Vector2 offset) {
            this.nodeinfos = nodeinfos;
            this.NodeCPIndex = NodeCPIndex;
            this.ConnectionIndexIn = ConnectionIndexIn;
            this.ConnectionIndexOut = ConnectionIndexOut;
            this.NumberOfCP = NumberOfCP;
            this.offset = offset;
        }
    }

    [Serializable]
    public class NodeInfo {
        public string type;
        public Rect rect;

        public string title;
        public string text;
        public AudioClip clip;
        public List<string> triggers;

        public NodeInfo(string type, Rect rect, string title = null, string text = null, AudioClip clip = null, List<string> triggers = null) {
            this.type = type;
            this.rect = rect;
            this.title = title;
            this.text = text;
            this.clip = clip;
            this.triggers = triggers;
        }
    }
}