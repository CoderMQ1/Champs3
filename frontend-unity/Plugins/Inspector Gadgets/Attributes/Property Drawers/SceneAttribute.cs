// Inspector Gadgets // https://kybernetik.com.au/inspector-gadgets // Copyright 2021 Kybernetik //

using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using InspectorGadgets.Attributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace InspectorGadgets.Attributes
{
    /// <summary>[Pro-Only]
    /// Causes the attributed int or string field to be drawn as a dropdown box for selecting scenes from the build settings.
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public sealed class SceneAttribute : PropertyAttribute { }
}

#if UNITY_EDITOR
namespace InspectorGadgets.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    internal sealed class SceneDrawer : PropertyDrawer
    {
        /************************************************************************************************************************/

        private static bool _IsListeningForSceneListChange;
        private static EditorBuildSettingsScene[] _AllScenes;
        private static readonly List<string> ActiveScenes = new List<string>();
        private static readonly GUIContent ButtonContent = new GUIContent();

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void OnGUI(Rect area, SerializedProperty property, GUIContent label)
        {
            GatherScenes();

            area = EditorGUI.IndentedRect(area);

            var width = area.width;

            var labelWidth = EditorGUIUtility.labelWidth + 14;

            area.xMax = labelWidth;
            GUI.Label(area, label);

            area.width = width;
            area.xMin = labelWidth;

            string buttonLabel;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:

                    var value = property.intValue;

                    if (value >= 0 && value < ActiveScenes.Count)
                        buttonLabel = ActiveScenes[value];
                    else
                        buttonLabel = "None";

                    ButtonContent.text = value + ": " + buttonLabel;
                    ButtonContent.tooltip = buttonLabel;

                    if (DrawButton(area))
                        ShowIndexPopup(buttonLabel, property);

                    break;

                case SerializedPropertyType.String:

                    buttonLabel = property.stringValue;
                    ButtonContent.text = buttonLabel;
                    ButtonContent.tooltip = null;

                    if (DrawButton(area))
                        ShowPathPopup(buttonLabel, property);

                    break;

                default:
                    GUI.Label(area, "Invalid [Scene] attribute");
                    break;
            }
        }

        /************************************************************************************************************************/

        private void GatherScenes()
        {
            if (_AllScenes != null)
                return;

            if (!_IsListeningForSceneListChange)
            {
                _IsListeningForSceneListChange = true;

                EditorBuildSettings.sceneListChanged += () =>
                {
                    _AllScenes = null;
                    ActiveScenes.Clear();
                };
            }

            _AllScenes = EditorBuildSettings.scenes;

            for (int i = 0; i < _AllScenes.Length; i++)
            {
                var scene = _AllScenes[i];
                if (scene.enabled)
                    ActiveScenes.Add(scene.path);
            }
        }

        /************************************************************************************************************************/

        private bool DrawButton(Rect area)
        {
            return GUI.Button(area, ButtonContent, EditorStyles.popup);
        }

        /************************************************************************************************************************/

        private void ShowIndexPopup(string selectedLabel, SerializedProperty property)
        {
            var menu = new GenericMenu();

            AddMenuItem(menu, "None", selectedLabel, () => property.intValue = -1, property);

            var sceneIndex = 0;
            for (int i = 0; i < _AllScenes.Length; i++)
            {
                var scene = _AllScenes[i];
                if (scene.enabled)
                {
                    var currentSceneIndex = sceneIndex;
                    AddMenuItem(menu, currentSceneIndex + ": " + scene.path.AllBackslashes(), selectedLabel,
                        () => property.intValue = currentSceneIndex, property);
                    sceneIndex++;
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(scene.path.AllBackslashes()));
                }
            }

            menu.ShowAsContext();
        }

        /************************************************************************************************************************/

        private void ShowPathPopup(string selectedLabel, SerializedProperty property)
        {
            var menu = new GenericMenu();

            AddMenuItem(menu, "None", selectedLabel, () => property.stringValue = "", property);

            var sceneIndex = 0;
            for (int i = 0; i < _AllScenes.Length; i++)
            {
                var scene = _AllScenes[i];
                var name = Path.GetFileNameWithoutExtension(scene.path);

                if (scene.enabled)
                {
                    AddMenuItem(menu, name, selectedLabel, () => property.stringValue = name, property);
                    sceneIndex++;
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(name));
                }
            }

            menu.ShowAsContext();
        }

        /************************************************************************************************************************/

        private static void AddMenuItem(GenericMenu menu, string label, string selectedLabel, Action method, SerializedProperty property)
        {
            menu.AddItem(new GUIContent(label), label == selectedLabel, () =>
            {
                method();
                property.serializedObject.ApplyModifiedProperties();
            });
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        /************************************************************************************************************************/
    }
}
#endif
