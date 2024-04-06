// Inspector Gadgets // https://kybernetik.com.au/inspector-gadgets // Copyright 2021 Kybernetik //

#if UNITY_EDITOR
using InspectorGadgets.Attributes;
using System.Collections.Generic;
using UnityEditor;
#endif
using UnityEngine;

namespace InspectorGadgets.Attributes
{
    /// <summary>[Pro-Only]
    /// Causes the attributed <see cref="Quaternion"/> field to be drawn as Euler Angles.
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public sealed class EulerAttribute : PropertyAttribute { }
}

#if UNITY_EDITOR
namespace InspectorGadgets.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(EulerAttribute))]
    internal sealed class EulerDrawer : PropertyDrawer
    {
        /************************************************************************************************************************/

        private sealed class Data
        {
            public Vector3 eulerAngles, oldEulerAngles;
        }

        private static readonly Dictionary<string, Data>
            CachedData = new Dictionary<string, Data>();

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void OnGUI(Rect area, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Quaternion)
            {
                EditorGUI.PropertyField(area, property, label);
                var accessor = property.GetAccessor();
                Debug.LogError("Invalid [Euler] Attribute: " +
                    (accessor != null ? accessor.ToString() : property.propertyPath) +
                    " is not a Quaternion.");
                return;
            }

            if (!CachedData.TryGetValue(property.propertyPath, out var data))
            {
                data = new Data();
                data.eulerAngles = data.oldEulerAngles = property.quaternionValue.eulerAngles;

                CachedData.Add(property.propertyPath, data);
            }

            var wideMode = EditorGUIUtility.wideMode;
            var showMixedValue = EditorGUI.showMixedValue;

            EditorGUIUtility.wideMode = true;
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

            label = EditorGUI.BeginProperty(area, label, property);

            if (EditorGUI.showMixedValue)
            {
                data.eulerAngles = data.oldEulerAngles = default;
            }
            else
            {
                var euler = property.quaternionValue.eulerAngles;
                if (data.oldEulerAngles != euler)
                {
                    data.eulerAngles = euler;
                }
            }

            EditorGUI.BeginChangeCheck();
            data.eulerAngles = EditorGUI.Vector3Field(area, label, data.eulerAngles);
            if (EditorGUI.EndChangeCheck())
            {
                var quaternion = Quaternion.Euler(data.eulerAngles);
                property.quaternionValue = quaternion;
                data.oldEulerAngles = quaternion.eulerAngles;
            }

            EditorGUI.EndProperty();

            EditorGUI.showMixedValue = showMixedValue;
            EditorGUIUtility.wideMode = wideMode;
        }

        /************************************************************************************************************************/
    }
}
#endif
