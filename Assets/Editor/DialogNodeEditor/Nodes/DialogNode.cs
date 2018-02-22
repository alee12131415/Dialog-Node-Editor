using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DNECore;

namespace DNECore {
    public class DialogNode : Node {
        public ConnectionPoint inPoint; //not really used, for editor structure
        public List<ConnectionPoint> outPoints = new List<ConnectionPoint>();
        public List<string> triggers = new List<string>();

        public string title;
        public string text;
        public AudioClip clip;

        private float offset;
        private float button_height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + 4f;
        private bool isAddClicked = false;
        private bool isRemoveClicked = false;
        private Vector2 scroll;

        public DialogNode(DialogNodeEditor editor, Vector2 position) : base(editor, position) {
            Init(position);
        }

        public DialogNode(DialogNodeEditor editor, NodeInfo info) : base(editor, info) {
            Init(new Vector2(info.rect.x, info.rect.y));
            SetTriggers(info.triggers);
            clip = info.clip;
            text = info.text;
            title = info.title;
        }

        public override void Init(Vector2 position) {
            width = 300;
            height = 200;
            rect = new Rect(position.x, position.y, width, height);
            inPoint = new ConnectionPoint(this, ConnectionPointType.In, editor.OnClickInPoint);
            outPoints.Add(new ConnectionPoint(this, ConnectionPointType.Out, editor.OnClickOutPoint));
            triggers.Add("default");
        }

        public void SetTriggers(List<string> triggers) {
            this.triggers = triggers;
        }

        public override void Draw() {
            //calc height needed
            rect.height = offset + ((3 + triggers.Count) * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing)) + 10 + button_height + (EditorGUIUtility.singleLineHeight * 5);


            inPoint.Draw();
            for (int i = triggers.Count - 1; i >= 0; i--) {
                outPoints[i].Draw(rect.y + offset + ((2 + i) * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing)) - (outPoints[i].rect.height * 0.5f) + (EditorGUIUtility.singleLineHeight * 0.5f) + button_height);
            }

            GUI.Box(rect, title, style);

            GUILayout.BeginArea(new Rect(rect.x, rect.y + offset, rect.width, rect.height - offset));
            GUILayout.BeginVertical();

            title = EditorGUILayout.TextField("Title", title);
            EditorGUILayout.PrefixLabel("Text");
            text = EditorGUILayout.TextArea(text, GUILayout.Height(EditorGUIUtility.singleLineHeight * 5));
            clip = (AudioClip)EditorGUILayout.ObjectField("Audio", clip, typeof(AudioClip), false);

            GUILayout.BeginHorizontal();
            isRemoveClicked = GUILayout.Button("-");
            isAddClicked = GUILayout.Button("+");
            GUILayout.EndHorizontal();

            for (int i = 0; i < triggers.Count; i++) {
                triggers[i] = EditorGUILayout.TextField("Option " + i, triggers[i]);
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        public override List<ConnectionPoint> GetConnectionPoints() {
            List<ConnectionPoint> result = new List<ConnectionPoint> { inPoint };
            result.AddRange(outPoints);
            return result;
        }

        public override bool ProcessEvents(Event e) {
            ProcessDefault(e);
            inPoint.ProcessEvents(e);
            for (int i = 0; i < outPoints.Count; i++) {
                outPoints[i].ProcessEvents(e);
            }
            if (isAddClicked) {
                outPoints.Add(new ConnectionPoint(this, ConnectionPointType.Out, editor.OnClickOutPoint));
                triggers.Add("");
            }
            else if (isRemoveClicked && outPoints.Count > 1) {
                outPoints.RemoveAt(outPoints.Count - 1);
                triggers.RemoveAt(triggers.Count - 1);
            }
            return false;
        }

        public override void SetStyle() {
            style.normal.background = AssetDatabase.LoadAssetAtPath("Assets/Editor/DialogNodeEditor/Textures/blueTex.png", typeof(Texture2D)) as Texture2D;

            style.normal.textColor = Color.white;
            style.fontSize = 24;
            style.alignment = TextAnchor.UpperCenter;

            GUIContent content = new GUIContent(title);
            offset = style.CalcSize(content).y;
        }

        public DialogNode Clone() {
            return (DialogNode)MemberwiseClone();
        }

        public override NodeInfo GetInfo() {
            return new NodeInfo(GetType().FullName, rect, title, text, clip, triggers);
        }

        public override void Rebuild(List<ConnectionPoint> cp) {
            inPoint = cp[0];
            outPoints = cp.GetRange(1, cp.Count - 1);

            inPoint.Rebuild(this, ConnectionPointType.In, editor.OnClickInPoint);
            for (int i = 0; i < outPoints.Count; i++) {
                outPoints[i].Rebuild(this, ConnectionPointType.Out, editor.OnClickOutPoint);
            }
        }
    }
}