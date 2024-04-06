// Inspector Gadgets // https://kybernetik.com.au/inspector-gadgets // Copyright 2021 Kybernetik //

using InspectorGadgets.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using InspectorGadgets.Editor;
#endif

namespace InspectorGadgets.Attributes
{
    /// <summary>[Pro-Only]
    /// Provides labels for the elements of a collection field to use instead of just calling them "Element X".
    /// </summary>
    public sealed class LabelledCollectionAttribute : PropertyAttribute
    {
        /************************************************************************************************************************/

        private Func<int, string> _GetLabel;

        /// <summary>Get the label to use for the element at the specified `index` in the collection.</summary>
        public string GetLabel(int index)
        {
            if (_GetLabel != null)
                return _GetLabel(index);
            else
                return null;
        }

        /************************************************************************************************************************/

        /// <summary>Uses the specified `labels` for the collection elements.</summary>
        public LabelledCollectionAttribute(params string[] labels)
        {
            _GetLabel = (int index) => labels[index % labels.Length];
        }

        /************************************************************************************************************************/

        /// <summary>Uses the value names of the specified `enumType` for the collection elements.</summary>
        public LabelledCollectionAttribute(Type enumType)
        {
            var names = Enum.GetNames(enumType);
            _GetLabel = (int index) =>
            {
                if (index >= 0 && index < names.Length)
                    return names[index];
                else
                    return index.ToString();
            };
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Uses the member with the specified name to determine the labels of the collection elements.
        /// <para></para>
        /// If the member is a collection field, the values in that collection will be used as the element labels.
        /// <para></para>
        /// If the member is a method with a single int parameter and a non-void return type, it will be called with
        /// each element index to determine the label.
        /// </summary>
        public LabelledCollectionAttribute(string memberName)
        {
#if UNITY_EDITOR
            _MemberName = memberName;
#endif
        }

        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        private static readonly Type[] MethodParameterTypes = { typeof(int) };

        private string _MemberName;

        internal void Initialize(FieldInfo attributedField, UnityEditor.SerializedProperty property)
        {
            if (_MemberName == null)
                return;

            var memberName = _MemberName;
            _MemberName = null;

            var accessor = property.GetAccessor();
            if (accessor != null)
            {
                var type = accessor.GetField(property).FieldType;
                if (type.IsArray)// Array Field.
                {
                    _GetLabel = (int index) =>
                    {
                        var array = accessor.GetValue(property.serializedObject.targetObject) as Array;
                        if (array != null && index >= 0 && index < array.Length)
                        {
                            var value = array.GetValue(index);
                            return value?.ToString();
                        }
                        else
                        {
                            return index.ToString();
                        }
                    };
                }
                else if (typeof(IList).IsAssignableFrom(type))// List Field.
                {
                    _GetLabel = (int index) =>
                    {
                        var list = accessor.GetValue(property.serializedObject.targetObject) as IList;
                        if (list != null && index >= 0 && index < list.Count)
                        {
                            var value = list[index];
                            return value?.ToString();
                        }
                        else
                        {
                            return index.ToString();
                        }
                    };
                }

                return;
            }

            var method = attributedField.DeclaringType.GetMethod(memberName, IGEditorUtils.AnyAccessBindings, null, MethodParameterTypes, null);
            if (method != null)// Named Method returns labels.
            {
                if (method.ReturnType != typeof(void))
                {
                    accessor = accessor.Parent;
                    var parameters = new object[1];

                    _GetLabel = (int index) =>
                    {
                        parameters[0] = index;
                        var name = method.Invoke(accessor.GetValue(property.serializedObject.targetObject), parameters);
                        return name?.ToString();
                    };
                }

                return;
            }
        }

        /************************************************************************************************************************/
#endif
    }
}

#if UNITY_EDITOR
namespace InspectorGadgets.Editor.PropertyDrawers
{
    [UnityEditor.CustomPropertyDrawer(typeof(LabelledCollectionAttribute))]
    internal sealed class LabelledCollectionDrawer : ObjectDrawer
    {
        /************************************************************************************************************************/

        private readonly Dictionary<string, int>
            PropertyIndices = new Dictionary<string, int>();

        /************************************************************************************************************************/

        public override void OnGUI(Rect area, UnityEditor.SerializedProperty property, GUIContent label)
        {
            if (!string.IsNullOrEmpty(label.text) || !string.IsNullOrEmpty(label.tooltip))
            {
                var path = property.propertyPath;

                if (!PropertyIndices.TryGetValue(path, out var index))
                {
                    index = path.LastIndexOf('[');
                    if (index >= 0)
                    {
                        index++;
                        var close = path.IndexOf(']', index);
                        if (index >= 0)
                        {
                            var textIndex = path.Substring(index, close - index);
                            if (!int.TryParse(textIndex, out index))
                                index = -1;
                        }
                    }

                    PropertyIndices.Add(path, index);
                }

                if (index >= 0)
                {
                    var attribute = (LabelledCollectionAttribute)this.attribute;
                    attribute.Initialize(fieldInfo, property);

                    var name = attribute.GetLabel(index);

                    if (!string.IsNullOrEmpty(name))
                        label = new GUIContent(name, label.text + ": " + name);
                }
            }

            base.OnGUI(area, property, label);
        }

        /************************************************************************************************************************/
    }
}
#endif
