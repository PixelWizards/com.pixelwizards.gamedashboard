using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace PixelWizards.Utility.Editor
{
    public class GameDashboard : EditorWindow
    {
        static int MinWidth = 150;

        private static List<string> scenes = new();
        private static int buttonHeight = 30;
        Vector2 _scroll;
        
        private const string menuEntry = "Tools/Game Dashboard";
        
        [MenuItem(menuEntry, false, -100)]
        static void Init()
        {
            GameDashboard window = (GameDashboard)EditorWindow.GetWindow<GameDashboard>("Dashboard");
            window.minSize = new Vector2(MinWidth, 375);

            scenes.Clear();
            for (var i = 0; i < EditorSceneManager.sceneCountInBuildSettings; i++)
            {
                scenes.Add(SceneUtility.GetScenePathByBuildIndex(i));
            }
        }

        void OnGUI()
        {
            if(Application.isPlaying)
            {
                GUILayout.Label("Application playing...");
                return;
            }
            if(scenes.Count < 1)
            {
                Init();
            }
            GUI.backgroundColor = Color.grey;
            GUILayout.Label("Scenes in Build");
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            {
                GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                foreach (var scene in scenes)
                {
                    if (GUILayout.Button(scene, GUILayout.MinHeight(100), GUILayout.Height(buttonHeight)))
                    {
                        EditorSceneManager.OpenScene(scene, OpenSceneMode.Single);
                    }
                }
            }
            EditorGUILayout.EndScrollView();
            GUI.backgroundColor = Color.grey;
            if(GUILayout.Button("Refresh", GUILayout.MinHeight(100), GUILayout.Height(buttonHeight)))
            {
                Init();
            }
        }
    }
}