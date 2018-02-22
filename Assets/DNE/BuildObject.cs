using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNE {
    [System.Serializable]
    public class BuildObject : ScriptableObject {
        [SerializeField] private List<BuildNode> nodes = new List<BuildNode>();
        [SerializeField] private int start_index;
        [SerializeField] private int current_index;

        //something something no constructos for instances
        public void Init(List<BuildNode> nodes, int start_index, int current_index) {
            this.nodes = nodes;
            this.start_index = start_index;
            this.current_index = current_index;
        }

        public BuildObject Get() {
            return (BuildObject)MemberwiseClone();
        }

        public BuildNode Next(string trigger) {
            current_index = nodes[current_index].next_node(trigger);
            if (current_index >= 0) {
                return nodes[current_index];
            } else {
                return null;
            }
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
        [SerializeField] private string title;
        [SerializeField] private string text;
        [SerializeField] private AudioClip clip;
        [SerializeField] private List<string> triggers;
        public List<int> next_index; //TODO figure out what your own code does so we can make this private k?

        public string Title { get { return title; } }
        public string Text { get { return text; } }
        public AudioClip Clip { get { return clip; } }
        public List<string> Triggers { get { return triggers; } }

        public int next_node(string trigger) {
            if (!trigger.Contains(trigger)) {
                Debug.LogWarning("Trigger does not exist in this node!");
            }
            return next_index[triggers.IndexOf(trigger)];
        }

        public BuildNode(string title, string text, AudioClip clip, List<string> triggers) {
            this.title = title;
            this.text = text;
            this.clip = clip;
            this.triggers = triggers;
            next_index = new List<int>();
            for (int i = 0; i < triggers.Count; i++) {
                next_index.Add(-1);
            }
        }
    }
}