// Inspector Gadgets // https://kybernetik.com.au/inspector-gadgets // Copyright 2021 Kybernetik //

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InspectorGadgets.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(Object), true)]
    internal class ObjectDrawer : PropertyDrawer
    {
        /************************************************************************************************************************/

        public static readonly AutoPrefs.EditorInt
            ObjectEditorNestLimit = new AutoPrefs.EditorInt(Strings.PrefsKeyPrefix + nameof(ObjectEditorNestLimit), 3);
        public static readonly AutoPrefs.EditorBool
            ItaliciseSelfReferences = new AutoPrefs.EditorBool(Strings.PrefsKeyPrefix + nameof(ItaliciseSelfReferences), true);

        /************************************************************************************************************************/

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUI.GetPropertyHeight(property, label, property.isExpanded);

        /************************************************************************************************************************/

        public override void OnGUI(Rect area, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.PropertyField(area, property, label, property.isExpanded);
                return;
            }

            GetReference(property, out var reference, out var isSelf);

            DoGetButtonGUI(ref area, property, reference);
            HandleDragAndDrop(area, property, reference);

            // Property Field.

            var originalStyle = EditorStyles.objectField.fontStyle;
            var labelWidth = EditorGUIUtility.labelWidth;
            try
            {
                var fieldArea = area;
                if (isSelf)
                {
                    EditorStyles.objectField.fontStyle = FontStyle.Italic;
                }
                else
                {
                    isSelf = !NestingIsEnabled(property, reference);

                    if (!isSelf && !EditorGUIUtility.hierarchyMode)
                    {
                        var indentLevel = EditorGUI.indentLevel;
                        EditorGUI.indentLevel = 1;

                        var x = fieldArea.x;
                        fieldArea = EditorGUI.IndentedRect(fieldArea);

                        EditorGUIUtility.labelWidth -= fieldArea.x - x;

                        EditorGUI.indentLevel = indentLevel;
                    }
                }

                EditorGUI.PropertyField(fieldArea, property, label, property.isExpanded);
            }
            finally
            {
                EditorStyles.objectField.fontStyle = originalStyle;
                EditorGUIUtility.labelWidth = labelWidth;
            }

            // Nested Inspector.
            if (!isSelf)
                DoNestedInspectorGUI(area, property, reference);
        }

        /************************************************************************************************************************/

        private void GetReference(SerializedProperty property, out Object reference, out bool isSelf)
        {
            isSelf = false;

            if (!property.hasMultipleDifferentValues && property.propertyType == SerializedPropertyType.ObjectReference)
            {
                reference = property.objectReferenceValue;

                if (!ItaliciseSelfReferences ||
                    reference == null)
                    return;

                if (property.serializedObject.targetObject is Component targetComponent)
                {
                    if (reference is Component component)
                    {
                        if (component.gameObject == targetComponent.gameObject)
                        {
                            isSelf = true;
                        }
                    }
                    else
                    {
                        if (reference is GameObject gameObject)
                        {
                            if (gameObject == targetComponent.gameObject)
                            {
                                isSelf = true;
                            }
                        }
                    }
                }
                else if (reference == property.serializedObject.targetObject)
                {
                    isSelf = true;
                }
            }
            else
            {
                reference = null;
            }
        }

        /************************************************************************************************************************/

        private static void DoGetButtonGUI(ref Rect area, SerializedProperty property, Object reference)
        {
            if (reference != null ||
                !(property.serializedObject.targetObject is Component))
                return;

            var accessor = property.GetAccessor();
            var type = accessor.GetFieldElementType(property);
            if (!typeof(Component).IsAssignableFrom(type))
                return;

            var label = IGEditorUtils.TempContent("Get");

            var width = InternalGUI.SmallButtonStyle.CalculateWidth(label);

            var buttonArea = IGEditorUtils.StealFromRight(
                ref area, width, new RectOffset((int)EditorGUIUtility.standardVerticalSpacing, 0, 0, 0));

            if (GUI.Button(buttonArea, label, InternalGUI.SmallButtonStyle))
            {
                property.ForEachTarget((prop) =>
                {
                    if (prop.serializedObject.targetObject is Component component)
                    {
                        component = IGUtils.ProgressiveSearch(component.gameObject, type, prop.displayName);
                        if (component != null)
                        {
                            prop.objectReferenceValue = component;
                            EditorGUIUtility.PingObject(component);
                        }
                    }
                });
            }
        }

        /************************************************************************************************************************/

        private static readonly int DragHint = nameof(DragHint).GetHashCode();

        private static void HandleDragAndDrop(Rect area, SerializedProperty property, Object reference)
        {
            var id = GUIUtility.GetControlID(DragHint, FocusType.Passive, area);

            var currentEvent = Event.current;
            switch (currentEvent.type)
            {
                // Drag out of object field.
                case EventType.MouseDrag:
                    if (GUIUtility.keyboardControl == id + 1 &&
                        currentEvent.button == 0 &&
                        area.Contains(currentEvent.mousePosition) &&
                        reference != null)
                    {
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.objectReferences = Serialization.GetValues<Object>(property);
                        DragAndDrop.StartDrag("Objects");
                        currentEvent.Use();
                    }
                    break;

                // Drop into Component field, show menu if there are multiple valid components.
                case EventType.DragPerform:
                    if (area.Contains(currentEvent.mousePosition))
                    {
                        var accessor = property.GetAccessor();
                        var type = accessor.GetFieldElementType(property);
                        if (!typeof(Component).IsAssignableFrom(type))
                            return;

                        var droppingReferences = DragAndDrop.objectReferences;
                        if (droppingReferences.Length < 1)
                            return;

                        var components = new List<Component>();
                        for (int i = 0; i < droppingReferences.Length; i++)
                        {
                            var dropping = droppingReferences[i];
                            if (type.IsAssignableFrom(dropping.GetType()))
                            {
                                components.Add((Component)dropping);
                            }
                            else if (dropping is GameObject gameObject)
                            {
                                components.AddRange(gameObject.GetComponents(type));
                            }
                        }

                        if (components.Count > 1)
                        {
                            DragAndDrop.AcceptDrag();
                            currentEvent.Use();

                            property = property.Copy();
                            var menu = new GenericMenu();

                            GameObject previousGameObject = null;
                            var index = 0;

                            for (int i = 0; i < components.Count; i++)
                            {
                                var component = components[i];

                                if (previousGameObject != component.gameObject)
                                {
                                    previousGameObject = component.gameObject;
                                    index = 0;
                                }
                                else
                                {
                                    index++;
                                }

                                var label = $"{previousGameObject.name} [{index}] -> {component.GetType().GetNameCS()}";

                                menu.AddItem(new GUIContent(label), reference == component, () =>
                                {
                                    Undo.RecordObjects(property.serializedObject.targetObjects, "Inspector");
                                    property.objectReferenceValue = component;
                                    property.serializedObject.ApplyModifiedProperties();
                                });
                            }

                            menu.ShowAsContext();
                        }
                    }
                    break;
            }
        }

        /************************************************************************************************************************/
        #region Nesting
        /************************************************************************************************************************/

        private static readonly HashSet<Object>
            CurrentReferences = new HashSet<Object>();

        private static GUIStyle _NestAreaStyle;

        private bool _IsInitialized;
        private bool _AllowNestedEditors;
        private UnityEditor.Editor _TargetEditor;

        /************************************************************************************************************************/

        private bool NestingIsEnabled(SerializedProperty property, Object reference)
        {
            return
                reference != null &&
                AllowNesting(reference.GetType()) &&
                CurrentReferences.Count < ObjectEditorNestLimit &&
                !property.hasMultipleDifferentValues &&
                AllowNesting(property) &&
                !CurrentReferences.Contains(reference);
        }

        /************************************************************************************************************************/

        private void DoNestedInspectorGUI(Rect area, SerializedProperty property, Object reference)
        {
            // Disable the GUI if HideFlags.NotEditable is set.
            var enabled = GUI.enabled;

            try
            {
                GUI.enabled = (reference.hideFlags & HideFlags.NotEditable) != HideFlags.NotEditable;

                CurrentReferences.Add(reference);

                property.isExpanded = EditorGUI.Foldout(area, property.isExpanded, GUIContent.none, true);
                if (property.isExpanded)
                {
                    const float NegativePadding = 4;
                    EditorGUIUtility.labelWidth -= NegativePadding;

                    if (_NestAreaStyle == null)
                    {
                        _NestAreaStyle = new GUIStyle(GUI.skin.box);
                        var rect = _NestAreaStyle.margin;
                        rect.bottom = rect.top = 0;
                        _NestAreaStyle.margin = rect;
                    }

                    EditorGUI.indentLevel++;
                    GUILayout.BeginVertical(_NestAreaStyle);

                    try
                    {
                        // If the target has changed, destroy the old editor and make a new one.
                        if (_TargetEditor == null || _TargetEditor.target != reference)
                        {
                            Editors.Destroy(_TargetEditor);
                            _TargetEditor = Editors.Create(reference);
                        }

                        // Draw the target editor.
                        _TargetEditor.OnInspectorGUI();
                    }
                    catch (ExitGUIException)
                    {
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                    }

                    GUILayout.EndVertical();
                    EditorGUI.indentLevel--;

                    EditorGUIUtility.labelWidth += NegativePadding;
                }
            }
            finally
            {
                CurrentReferences.Remove(reference);
                GUI.enabled = enabled;
            }
        }

        /************************************************************************************************************************/

        private bool AllowNesting(SerializedProperty property)
        {
            if (!_IsInitialized)
            {
                _IsInitialized = true;
                _AllowNestedEditors =
                    Event.current.type != EventType.Repaint &&
                    AllowNesting(property.GetAccessor(), property);
            }

            return _AllowNestedEditors;
        }

        /************************************************************************************************************************/

        private static readonly Dictionary<Serialization.PropertyAccessor, bool>
            AllowNestingAccessorCache = new Dictionary<Serialization.PropertyAccessor, bool>();

        private static bool AllowNesting(Serialization.PropertyAccessor accessor, SerializedProperty property)
        {
            if (accessor == null)
                return true;

            if (!AllowNestingAccessorCache.TryGetValue(accessor, out var allow))
            {
                var type = accessor.GetFieldElementType(property);
                if (!AllowNesting(type))
                {
                    allow = false;
                }
                else
                {
                    allow = AllowNesting(accessor.Parent, property);
                }

                AllowNestingAccessorCache.Add(accessor, allow);
            }

            return allow;
        }

        /************************************************************************************************************************/

        private static readonly Dictionary<Type, bool>
            AllowNestingTypeCache = new Dictionary<Type, bool>()
            {
#if ! DISABLE_MODULE_AUDIO
                { typeof(AudioClip), false },
#endif
                { typeof(DefaultAsset), false },
                { typeof(GameObject), false },
                { typeof(Material), false },
#if ! DISABLE_MODULE_ANIMATION
                { typeof(RuntimeAnimatorController), false },
#endif
            };

        /************************************************************************************************************************/

        private static bool AllowNesting(Type type)
        {
            if (type == null)
                return true;

            if (!AllowNestingTypeCache.TryGetValue(type, out var allow))
            {
                var field = type.GetField("NestedObjectDrawers", IGEditorUtils.StaticBindings);
                if (field != null && field.IsLiteral && field.FieldType == typeof(bool))
                {
                    allow = (bool)field.GetValue(null);
                }
                else
                {
                    allow = AllowNesting(type.BaseType);
                }

                AllowNestingTypeCache.Add(type, allow);
            }

            return allow;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

#endif
