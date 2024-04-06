// Inspector Gadgets // https://kybernetik.com.au/inspector-gadgets // Copyright 2021 Kybernetik //

using InspectorGadgets.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace InspectorGadgets.Attributes
{
    /// <summary>[Pro-Only]
    /// Shows a warning for any elements of the attributed collection which aren't unique.
    /// </summary>
    public sealed class UniqueCollectionAttribute : PropertyAttribute { }
}

#if UNITY_EDITOR
namespace InspectorGadgets.Editor.PropertyDrawers
{
    [UnityEditor.CustomPropertyDrawer(typeof(UniqueCollectionAttribute))]
    internal sealed class UniqueCollectionDrawer : ObjectDrawer
    {
        /************************************************************************************************************************/

        private static readonly Color
            WarningColor = new Color(1, 0.65f, 0.65f);
        private const string
            NonUniqueMessage = "This element is not unique";

        private readonly List<object> Elements = new List<object>();
        private string _FirstProperty;
        private int _CurrentIndex;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
            => base.GetPropertyHeight(property, label);

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void OnGUI(Rect area, UnityEditor.SerializedProperty property, GUIContent label)
        {
            var color = GUI.color;
            var tooltip = label.tooltip;

            switch (Event.current.type)
            {
                case EventType.Layout:
                    if (_FirstProperty == null)
                    {
                        _FirstProperty = property.propertyPath;
                    }
                    else if (property.propertyPath == _FirstProperty)
                    {
                        Elements.Clear();
                        _CurrentIndex = 0;
                    }

                    Elements.Add(property.GetValue());
                    break;

                case EventType.Repaint:
                    var value = Elements[_CurrentIndex];
                    for (int i = 0; i < Elements.Count; i++)
                    {
                        if (i != _CurrentIndex && Equals(value, Elements[i]))
                        {
                            GUI.color = WarningColor;
                            if (!string.IsNullOrEmpty(label.tooltip))
                                label.tooltip = $"{NonUniqueMessage}\n{label.tooltip}";
                            else
                                label.tooltip = NonUniqueMessage;
                            break;
                        }
                    }

                    _CurrentIndex++;
                    break;

                default:
                    break;
            }

            base.OnGUI(area, property, label);

            label.tooltip = tooltip;
            GUI.color = color;
        }

        /************************************************************************************************************************/
    }
}
#endif
