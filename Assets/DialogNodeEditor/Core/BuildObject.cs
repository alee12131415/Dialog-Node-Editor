using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNE {
    [System.Serializable]
    public class BuildObject : ScriptableObject {
        public List<BuildNode> nodes = new List<BuildNode>();
        public int start_index;
        public int current_index;

        public BuildObject Get() {
            return (BuildObject)MemberwiseClone();
        }

        public void Next(string trigger) {
            current_index = nodes[current_index].next_node(trigger);
        }

        public BuildNode GetCurrent() {
            return nodes[current_index];
        }

        public void Reset() {
            current_index = start_index;
        }
    }

    [System.Serializable]
    public class BuildNode {
        public string title;
        public AudioClip clip;
        public List<string> triggers;
        public List<int> next_index;

        public int next_node(string trigger) {
            if (!trigger.Contains(trigger)) {
                Debug.LogWarning("Trigger does not exist in this node!");
            }
            return next_index[triggers.IndexOf(trigger)];
        }

        public BuildNode(NodeInfo info) {
            title = info.title;
            clip = info.clip;
            triggers = info.triggers;
            next_index = new List<int>();
            for (int i = 0; i < triggers.Count; i++) {
                next_index.Add(-1);
            }
        }
    }
}