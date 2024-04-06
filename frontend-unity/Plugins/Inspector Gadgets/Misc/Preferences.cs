// Inspector Gadgets // https://kybernetik.com.au/inspector-gadgets // Copyright 2021 Kybernetik //

#if UNITY_EDITOR

using InspectorGadgets.Editor.PropertyDrawers;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InspectorGadgets.Editor
{
    internal static class Preferences
    {
        /************************************************************************************************************************/

        private static readonly GUIContent
            // Transform Inspector.
            ShowCopyButton = new GUIContent("Show Copy Button",
                "Should the Transform Inspector show the [C] button to copy a transform property to an internal clipboard?"),
            ShowPasteButton = new GUIContent("Show Paste Button",
                "Should the Transform Inspector show the [P] button to paste a transform property from the internal clipboard?"),
            ShowSnapButton = new GUIContent("Show Snap Button",
                "Should the Transform Inspector show the [S] button to snap a transform property to the nearest snap increment specified in Edit/Snap Setings?"),
            ShowResetButton = new GUIContent("Show Reset Button",
                "Should the Transform Inspector show the [R] button to reset a transform property to its default value?"),
            DisableUselessButtons = new GUIContent("Disable Useless Buttons",
                "Should the above buttons be greyed out when they would do nothing?"),
            UseFieldColors = new GUIContent("Use Field Colors",
                "Should the X/Y/Z fields be colored Red/Green/Blue respectively?"),
            FieldPrimaryColor = new GUIContent("Field Primary Color",
                "The strength of the main color for each axis."),
            FieldSecondaryColor = new GUIContent("Field Secondary Color",
                "The strength of the other colors."),
            EmphasizeNonDefaultFields = new GUIContent("Emphasize Non-Default Fields",
                "Should Transform fields which aren't at their default value be given a thicker border?"),
            ItaliciseNonSnappedFields = new GUIContent("Italicise Non-Snapped Fields",
                "Should Transform fields which aren't a multiple of the snap increment specified in Edit/Snap Setings use italic text?"),
            ShrinkScientificFields = new GUIContent("Shrink Scientific Fields",
                "Should the font size be decreased for values that are displayed using scientific notation?"),
            DefaultToUniformScale = new GUIContent("Default to Uniform Scale",
                "Should Transform scale be shown as a single float field by default when the selected object has the same scale on all axes?"),
            SnapToGroundDistance = new GUIContent("Snap to Ground Distance",
                "The distance within which to check for the ground when using the Snap to Ground function in the Transform Position context menu."),
            SnapToGroundLayers = new GUIContent("Snap to Ground Layers",
                "This layer mask determines which physics layers are treated as ground for the Transform Position context menu."),

            // Scene Tools.
            OverrideTransformGizmos = new GUIContent("Override Transform Gizmos",
                "Should the default scene gizmos be overwritten in order to implement various features like \"Freeze child transforms\" and \"Draw gizmos for all selected objects\"?"),
            ShowMovementGuides = new GUIContent("Show Movement Guides",
                "Should the scene view movement tool show some extra lines while you are moving an object to indicate where you are moving it from?"),
            ShowMovementDistance = new GUIContent("Show Movement Distance",
                "Should moving an object display the distance from the old position?"),
            ShowMovementDistancePerAxis = new GUIContent("Show Movement Distance Per Axis",
                "Should the distance moved on each individual axis also be displayed?"),
            SceneLabelBackgroundColor = new GUIContent("Scene Label Background Color",
                "The color to use behind scene view labels to make them easier to read."),
            ShowPositionLabels = new GUIContent("Show Position Labels",
                "Should the scene view show the selected object's position around the Move tool?"),

            // Script Inspector.
            HideScriptProperty = new GUIContent("Hide Script Property",
                "Should the \"Script\" property at the top of each inspector be hidden to save space?"),
            AutoGatherRequiredComponents = new GUIContent("Auto Gather Required Components",
                "Should selecting an object in the editor automatically gather references for any of its component fields with a [RequireAssignment] attribute?" +
                "\n\nGathering is conducted using InspectorGadgetsUtils.GetComponentInHierarchy which finds the most appropriately named component in the selected object's children or parents."),
            AutoGatherSerializedComponents = new GUIContent("Auto Gather Serialized Components",
                "Should selecting an object in the editor automatically gather references for any of its component fields which are public or have [SerializeField] attribute?" +
                "\n\nGathering is conducted using InspectorGadgetsUtils.GetComponentInHierarchy which finds the most appropriately named component in the selected object's children or parents."),
            ItaliciseSelfReferences = new GUIContent("Italicise Self References",
                "Should Object reference fields be drawn in italics when referencing another component on the same GameObject?"),
            DefaultEditorState = new GUIContent("Default Editor State",
                "When to show Inspectables if not specified in their constructor"),
            ObjectEditorNestLimit = new GUIContent("Object Editor Nest Limit",
                "If higher than 0, Object fields will be drawn with a foldout arrow to draw the target object's inspector nested inside the current one.");

        /************************************************************************************************************************/

        private const string
            TransformInspector = "Transform Inspector",
            SceneTools = "Scene Tools",
            ScriptInspector = "Script Inspector";

        /************************************************************************************************************************/

        private static SettingsProvider _SettingsProvider;

        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            if (_SettingsProvider == null)
            {
                var keywords = new HashSet<string> { TransformInspector, SceneTools, ScriptInspector };
                GatherGUIContentText(typeof(Preferences), keywords);

                keywords.Add(DragAndDropSubAssets.Headding);
                GatherGUIContentText(typeof(DragAndDropSubAssets), keywords);

                _SettingsProvider = new SettingsProvider("Preferences/Inspector Gadgets", SettingsScope.User)
                {
                    label = "Inspector Gadgets Pro v" + Strings.InspectorGadgetsVersion,
                    guiHandler = (searchContext) => DrawPreferences(),
                    keywords = keywords,
                };
            }

            return _SettingsProvider;
        }

        /************************************************************************************************************************/

        private static void GatherGUIContentText(Type type, HashSet<string> set)
        {
            var fields = type.GetFields(IGEditorUtils.StaticBindings);
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.FieldType != typeof(GUIContent))
                    continue;

                var value = (GUIContent)field.GetValue(null);
                if (value != null)
                    set.Add(value.text);
            }
        }

        /************************************************************************************************************************/

        private static void DrawPreferences()
        {

            if (GUILayout.Button($"Open Documentation:  {Strings.DocumentationURL}"))
                IGEditorUtils.OpenDocumentation();

            var enabled = GUI.enabled;

            EditorGUILayout.Space();
            GUILayout.Label(TransformInspector, EditorStyles.boldLabel);

            TransformPropertyDrawer.ShowCopyButton.OnGUI(ShowCopyButton);
            TransformPropertyDrawer.ShowPasteButton.OnGUI(ShowPasteButton);
            TransformPropertyDrawer.ShowSnapButton.OnGUI(ShowSnapButton);
            TransformPropertyDrawer.ShowResetButton.OnGUI(ShowResetButton);
            TransformPropertyDrawer.DisableUselessButtons.OnGUI(DisableUselessButtons);

            TransformPropertyDrawer.UseFieldColors.OnGUI(UseFieldColors);
            if (!TransformPropertyDrawer.UseFieldColors)
                GUI.enabled = false;

            TransformPropertyDrawer.FieldPrimaryColor.OnGUI(FieldPrimaryColor, GUI.skin.horizontalSlider,
            (area, content, style) =>
            {
                return EditorGUI.Slider(area, content, TransformPropertyDrawer.FieldPrimaryColor, 0, 1);
            });

            TransformPropertyDrawer.FieldSecondaryColor.OnGUI(FieldSecondaryColor, GUI.skin.horizontalSlider,
            (area, content, style) =>
            {
                return EditorGUI.Slider(area, content, TransformPropertyDrawer.FieldSecondaryColor, 0, 1);
            });

            GUI.enabled = enabled;

            TransformPropertyDrawer.EmphasizeNonDefaultFields.OnGUI(EmphasizeNonDefaultFields);
            TransformPropertyDrawer.ItaliciseNonSnappedFields.OnGUI(ItaliciseNonSnappedFields);
            TransformPropertyDrawer.ShrinkScientificFields.OnGUI(ShrinkScientificFields);
            TransformEditor.DefaultToUniformScale.OnGUI(DefaultToUniformScale);

            if (PositionDrawer.SnapToGroundDistance.OnGUI(SnapToGroundDistance))
                PositionDrawer.SnapToGroundDistance.Value = Mathf.Max(PositionDrawer.SnapToGroundDistance, 0);
            PositionDrawer.SnapToGroundLayers.OnGUI(SnapToGroundLayers, (position, content, style) =>
            {
                return IGEditorUtils.DoLayerMaskField(position, content, PositionDrawer.SnapToGroundLayers);
            });

            EditorGUILayout.Space();
            GUILayout.Label(SceneTools, EditorStyles.boldLabel);
            TransformEditor.OverrideTransformGizmos.OnGUI(OverrideTransformGizmos);
            if (!TransformEditor.OverrideTransformGizmos)
            {
                EditorGUILayout.HelpBox(
                    "With this disabled, features like 'Freeze child transforms' and 'Draw gizmos for all selected objects' won't work.",
                    MessageType.Warning);
                Tools.hidden = false;
                GUI.enabled = false;
            }

            PositionDrawer.ShowMovementGuides.OnGUI(ShowMovementGuides);
            PositionDrawer.ShowMovementDistance.OnGUI(ShowMovementDistance);

            if (!PositionDrawer.ShowMovementDistance)
                GUI.enabled = false;
            PositionDrawer.ShowMovementDistancePerAxis.OnGUI(ShowMovementDistancePerAxis);
            GUI.enabled = enabled;

            InternalGUI.SceneLabelBackgroundColor.DoColorGUIField(SceneLabelBackgroundColor);

            PositionDrawer.ShowPositionLabels.OnGUI(ShowPositionLabels);

            GUI.enabled = true;
            AutoHideUI.DoPrefsGUI();

            EditorGUILayout.Space();
            GUILayout.Label(ScriptInspector, EditorStyles.boldLabel);

            ComponentEditor.HideScriptProperty.OnGUI(HideScriptProperty);
            ObjectDrawer.ItaliciseSelfReferences.OnGUI(ItaliciseSelfReferences);

            EditorGUI.BeginChangeCheck();
            var value = EditorGUILayout.EnumPopup(DefaultEditorState, IGUtils.DefaultEditorState);
            if (EditorGUI.EndChangeCheck())
                IGUtils.DefaultEditorState = (EditorState)value;

            ObjectDrawer.ObjectEditorNestLimit.OnGUI(ObjectEditorNestLimit,
                (position, content, style) => EditorGUI.IntSlider(position, content, ObjectDrawer.ObjectEditorNestLimit.Value, 0, 10));

            if (GUILayout.Button("Find and Fix Missing Scripts"))
                EditorWindow.GetWindow<MissingScriptWindow>();

            DragAndDropSubAssets.DoPreferencesGUI();

        }

        /************************************************************************************************************************/
    }
}

#endif
