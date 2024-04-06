// Inspector Gadgets // https://kybernetik.com.au/inspector-gadgets // Copyright 2021 Kybernetik //

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace InspectorGadgets.Editor
{
    internal static class InternalGUI
    {
        /************************************************************************************************************************/

        public static readonly float
            NameLabelWidth;

        public static readonly GUIStyle
            FieldLabelStyle,
            FloatFieldStyle,
            MiniSquareButtonStyle,
            SmallButtonStyle,
            UniformScaleButtonStyle,
            ModeButtonStyle,
            ModeLabelStyle;

        /************************************************************************************************************************/

        static InternalGUI()
        {
            FieldLabelStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                margin = new RectOffset(0, 0, 2, 2),
            };

            NameLabelWidth = FieldLabelStyle.CalculateWidth("Rotation") + EditorGUIUtility.standardVerticalSpacing;

            FloatFieldStyle = new GUIStyle(EditorStyles.numberField);

            MiniSquareButtonStyle = new GUIStyle(EditorStyles.miniButton)
            {
                margin = new RectOffset(0, 0, 2, 0),
                padding = new RectOffset(2, 2, 2, 2),
                alignment = TextAnchor.MiddleCenter,
                fixedWidth = EditorGUIUtility.singleLineHeight - 1,
                fixedHeight = EditorGUIUtility.singleLineHeight,
            };

            SmallButtonStyle = new GUIStyle(MiniSquareButtonStyle)
            {
                padding = new RectOffset(3, 3, 2, 2),
                fixedWidth = 0,
            };

            UniformScaleButtonStyle = new GUIStyle(MiniSquareButtonStyle)
            {
                margin = new RectOffset(2, 0, 2, 0),
                padding = new RectOffset(1, 3, -2, 0),
                fontSize = (int)EditorGUIUtility.singleLineHeight - 1,
            };

            ModeLabelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
            };

            ModeButtonStyle = new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset(),
            };
        }

        /************************************************************************************************************************/

        public static readonly AutoPrefs.EditorVector4 SceneLabelBackgroundColor = new AutoPrefs.EditorVector4(
            Strings.PrefsKeyPrefix + nameof(SceneLabelBackgroundColor), new Vector4(0.15f, 0.15f, 0.15f, 0.5f),
            onValueChanged: (value) => _SceneLabelBackgroundColorChanged = true);

        private static bool _SceneLabelBackgroundColorChanged;

        private static Texture2D _SceneLabelBackground;
        public static Texture2D SceneLabelBackground
        {
            get
            {
                if (SceneLabelBackgroundColor.Value.w <= 0)
                    return null;

                if (_SceneLabelBackground == null)
                {
                    _SceneLabelBackground = new Texture2D(1, 1);
                    _SceneLabelBackgroundColorChanged = true;

                    AssemblyReloadEvents.beforeAssemblyReload +=
                        () => Object.DestroyImmediate(_SceneLabelBackground);
                }

                if (_SceneLabelBackgroundColorChanged)
                {
                    _SceneLabelBackgroundColorChanged = false;
                    _SceneLabelBackground.SetPixel(0, 0, SceneLabelBackgroundColor);
                    _SceneLabelBackground.Apply();
                }

                return _SceneLabelBackground;
            }
        }

        /************************************************************************************************************************/
    }
}

#endif
