using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace PixelWizards.Utility.Editor
{
    public class GameDashboard : EditorWindow
    {
        static int MinWidth = 150;

        private class SceneInfo
        {
            public string Path;
            public bool Enabled;
        }

        private static readonly List<SceneInfo> scenes = new();
        private static int buttonHeight = 30;
        private Vector2 _scroll;

        private const string menuEntry = "Tools/Game Dashboard";
        private const string ShowDisabledPrefKey = "PixelWizards.GameDashboard.ShowDisabledScenes";

        private bool _showDisabledScenes;

        [MenuItem(menuEntry, false, -100)]
        private static void Init()
        {
            var window = GetWindow<GameDashboard>("Dashboard");
            window.minSize = new Vector2(MinWidth, 375);
            window.RefreshScenes();
        }

        private void OnEnable()
        {
            _showDisabledScenes = EditorPrefs.GetBool(ShowDisabledPrefKey, false);
            RefreshScenes();
        }

        private void RefreshScenes()
        {
            scenes.Clear();

            // Use EditorBuildSettings so we can see enabled/disabled flags
            var buildScenes = EditorBuildSettings.scenes;
            foreach (var s in buildScenes)
            {
                scenes.Add(new SceneInfo
                {
                    Path = s.path,
                    Enabled = s.enabled
                });
            }
        }

        private void OnGUI()
        {
            if (Application.isPlaying)
            {
                GUILayout.Label("Application playing...");
                return;
            }

            if (scenes.Count < 1)
            {
                RefreshScenes();
            }

            DrawToolbar();

            GUI.backgroundColor = Color.grey;
            GUILayout.Label("Scenes in Build");

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            {
                GUI.skin.button.alignment = TextAnchor.MiddleLeft;

                IEnumerable<SceneInfo> visibleScenes = _showDisabledScenes
                    ? scenes
                    : scenes.Where(s => s.Enabled);

                foreach (var scene in visibleScenes)
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Ping", GUILayout.MinHeight(100), GUILayout.Height(buttonHeight), GUILayout.Width(45f)))
                        {
                            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.Path);
                            Selection.activeObject = sceneAsset;
                            EditorGUIUtility.PingObject(sceneAsset);
                        }

                        string label = scene.Enabled ? scene.Path : $"{scene.Path} (disabled)";

                        if (GUILayout.Button(label, GUILayout.MinHeight(100), GUILayout.Height(buttonHeight)))
                        {
                            EditorSceneManager.OpenScene(scene.Path, OpenSceneMode.Single);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();

            GUI.backgroundColor = Color.grey;
            if (GUILayout.Button("Refresh", GUILayout.MinHeight(100), GUILayout.Height(buttonHeight)))
            {
                RefreshScenes();
            }
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                bool newShowDisabled = GUILayout.Toggle(
                    _showDisabledScenes,
                    "Show Disabled Scenes",
                    EditorStyles.toolbarButton
                );

                if (newShowDisabled != _showDisabledScenes)
                {
                    _showDisabledScenes = newShowDisabled;
                    EditorPrefs.SetBool(ShowDisabledPrefKey, _showDisabledScenes);
                    // No need to rebuild the list, just affects filtering
                    Repaint();
                }

                GUILayout.FlexibleSpace();
            }
        }
    }
}
