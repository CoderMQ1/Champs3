// Inspector Gadgets // https://kybernetik.com.au/inspector-gadgets // Copyright 2021 Kybernetik //

using UnityEngine;
using InspectorGadgets.Attributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace InspectorGadgets.Attributes
{
    /// <summary>[Pro-Only]
    /// Causes the attributed <see cref="Object"/> reference field to draw a preview of the target object.
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public sealed class ShowPreviewAttribute : PropertyAttribute
    {
        /// <summary>The maximum height (in pixels) at which to draw the preview.</summary>
        /// <remarks>Width will be determined using the aspect ratio of the preview.</remarks>
        public readonly int MaxHeight;

        /// <summary>Creates a new <see cref="ShowPreviewAttribute"/> with the specified size.</summary>
        public ShowPreviewAttribute(int maxHeight = 64)
        {
            MaxHeight = maxHeight;
        }
    }
}

#if UNITY_EDITOR
namespace InspectorGadgets.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ShowPreviewAttribute))]
    internal sealed class ShowPreviewDrawer : ObjectDrawer
    {
        /************************************************************************************************************************/

        private const float Padding = 1;

        private float _BaseHeight;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = _BaseHeight = EditorGUI.GetPropertyHeight(property, label);

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                if (property.objectReferenceValue != null)
                {
                    var previewTexture = AssetPreview.GetAssetPreview(property.objectReferenceValue);
                    if (previewTexture != null)
                    {
                        var attribute = (ShowPreviewAttribute)this.attribute;

                        height += Mathf.Min(attribute.MaxHeight, previewTexture.height) + Padding;
                    }
                }
            }

            return height;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void OnGUI(Rect area, SerializedProperty property, GUIContent label)
        {
            area.height = _BaseHeight;
            base.OnGUI(area, property, label);

            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                DrawWarningBox($"{property.name} is not an object reference type", property);
                return;
            }

            if (property.objectReferenceValue == null)
                return;

            var preview = AssetPreview.GetAssetPreview(property.objectReferenceValue);
            if (preview == null)
            {
                DrawWarningBox($"{property.name} doesn't have an asset preview", property);
                return;
            }

            var attribute = (ShowPreviewAttribute)this.attribute;

            var xMax = area.xMax;

            area.y += area.height + Padding;

            area.height = Mathf.Min(attribute.MaxHeight, preview.height);
            area.width = area.height * preview.height / preview.width;
            area.x = xMax - area.width;

            if (area.width <= 0 || area.height <= 0)
                return;

            GUI.DrawTexture(area, preview);
        }

        /************************************************************************************************************************/

        private void DrawWarningBox(string warningText, SerializedProperty property)
        {
            EditorGUILayout.HelpBox(warningText, MessageType.Warning);
            //Debug.LogWarning(warningText, property.serializedObject.targetObject);
        }

        /************************************************************************************************************************/
    }
}
#endif
