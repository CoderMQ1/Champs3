// Inspector Gadgets // https://kybernetik.com.au/inspector-gadgets // Copyright 2021 Kybernetik //

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace InspectorGadgets.Attributes
{
    /// <summary>Displays the attributed <see cref="int"/> field as a layer.</summary>
    public sealed class LayerAttribute : PropertyAttribute
    {
        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        /// <summary>[Editor-Only] Draws <see cref="LayerAttribute"/> fields.</summary>
        [CustomPropertyDrawer(typeof(LayerAttribute))]
        internal sealed class Drawer : PropertyDrawer
        {
            /************************************************************************************************************************/

            /// <inheritdoc/>
            public override void OnGUI(Rect area, SerializedProperty property, GUIContent label)
            {
                property.intValue = EditorGUI.LayerField(area, label, property.intValue);
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
#endif
        /************************************************************************************************************************/
    }
}

