using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DNE;

public class Toolbar {
    private DialogNodeEditor editor;
    private Rect rect;
    private bool clickedSave;
    private bool clickedLoad;
    private bool clickedBuild;

    public Toolbar(DialogNodeEditor editor) {
        this.editor = editor;
        rect = new Rect(0, 0, editor.position.width, 100);
    }

    public void Draw() {
        rect.width = editor.position.width;

        GUILayout.BeginArea(rect, EditorStyles.toolbar);

        GUILayout.BeginHorizontal();

        clickedSave = GUILayout.Button(new GUIContent("Save"), EditorStyles.toolbarButton, GUILayout.Width(50));
        clickedLoad = GUILayout.Button(new GUIContent("Load"), EditorStyles.toolbarButton, GUILayout.Width(50));
        clickedBuild = GUILayout.Button(new GUIContent("Build"), EditorStyles.toolbarButton, GUILayout.Width(50));

        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    public void ProcessEvents(Event e) {
        if (clickedSave) {
            string path = EditorUtility.SaveFilePanelInProject("Save Canvas", "Canvas.asset", "asset", "");
            if (path.Length > 0) {
                editor.SaveCanvas(path);
            }
        }
        
        if (clickedLoad) {
            string path = EditorUtility.OpenFilePanel("Load Canvas", "Assets", "asset");
            if (path.Length > "Assets".Length) {
                //path = path.Substring(Application.dataPath.Length + "/Resources/".Length, path.Length - Application.dataPath.Length - ".asset".Length - "/Resources/".Length);
                path = path.Substring(Application.dataPath.Length - "Assets".Length);
                editor.LoadCanvas(path);
            }
        }

        if (clickedBuild) {
            string path = EditorUtility.SaveFilePanelInProject("Save Build", "Build.asset", "asset", "");
            if (path.Length > 0) {
                editor.BuildCanvas(path);
            }
        }
    }
}
