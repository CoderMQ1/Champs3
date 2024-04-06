// Inspector Gadgets // https://kybernetik.com.au/inspector-gadgets // Copyright 2021 Kybernetik //

#if UNITY_EDITOR
using InspectorGadgets.Attributes;
using UnityEditor;
#endif
using UnityEngine;

namespace InspectorGadgets.Attributes
{
    /// <summary>[Pro-Only]
    /// Causes the attributed field to be greyed out and un-editable in the inspector.
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public sealed class ReadonlyAttribute : PropertyAttribute
    {
        /************************************************************************************************************************/

        private EditorState? _When;

        /// <summary>Indicates when the field should be greyed out.</summary>
        public EditorState When
        {
            get => _When.ValueOrDefault();
            set => _When = value;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Creates a new <see cref="ReadonlyAttribute"/> to apply its effects in the default <see cref="EditorState"/>
        /// (set in the <c>Edit/Preferences</c> menu.
        /// </summary>
        public ReadonlyAttribute() { }

        /// <summary>
        /// Creates a new <see cref="ReadonlyAttribute"/> to apply its effects in the specified <see cref="EditorState"/>.
        /// </summary>
        public ReadonlyAttribute(EditorState when = EditorState.Always)
        {
            _When = when;
        }

        /************************************************************************************************************************/
    }
}

#if UNITY_EDITOR
namespace InspectorGadgets.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ReadonlyAttribute))]
    internal sealed class ReadonlyDrawer : ObjectDrawer
    {
        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void OnGUI(Rect area, SerializedProperty property, GUIContent label)
        {
            var attribute = (ReadonlyAttribute)this.attribute;

            var enabled = GUI.enabled;
            GUI.enabled = !attribute.When.IsNow();
            base.OnGUI(area, property, label);

            var currentEvent = Event.current;
            if (currentEvent.rawType == EventType.ContextClick &&
                area.Contains(currentEvent.mousePosition))
                SerializedPropertyContextMenu.OpenMenu(property);

            GUI.enabled = enabled;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        /************************************************************************************************************************/
    }
}
#endif
