// Inspector Gadgets // https://kybernetik.com.au/inspector-gadgets // Copyright 2021 Kybernetik //

using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace InspectorGadgets.Attributes
{
    /// <summary>[Pro-Only]
    /// <see cref="Editor.Editor{T}"/> uses this attribute to add a label at the bottom of the default inspector to
    /// display the value of the marked property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class LabelAttribute : BaseInspectableAttribute
    {
        /************************************************************************************************************************/

        /// <summary>If true, the label will be hidden when the value is null.</summary>
        public bool HideWhenNull { get; set; }

        /// <summary>If true, the inspector will be constantly repainted while this label is shown to keep it updated.</summary>
        public bool ConstantlyRepaint { get; set; }

        /// <summary>
        /// If true, the label or attributed member name will be drawn on one line with the actual value drawn below it
        /// and able to take as many lines as you want.
        /// </summary>
        public bool LargeMode { get; set; }

        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        private Color? _Color;
        private Func<object, object> _Getter;
        private bool _IsStatic;

        private object _Value;
        private string _LabelValue;
        private Exception _Exception;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override string Initialize()
        {
            var field = Member as FieldInfo;
            if (field != null)
            {
                Initialize(field.GetValue, field.IsStatic, field);
                return null;
            }

            var property = Member as PropertyInfo;
            if (property != null)
            {
                var getter = property.GetGetMethod(true);

                if (getter != null)
                    Initialize(obj => getter.Invoke(obj, null), getter.IsStatic, property);
                else
                    return "it has no getter";

                return null;
            }

            var method = Member as MethodInfo;
            if (method != null)
            {
                if (method.GetParameters().Length == 0)
                    Initialize(obj => method.Invoke(obj, null), method.IsStatic, method);
                else
                    return "it has parameters";

                return null;
            }

            return "it has an unsupported member type: " + Member.GetType().FullName;
        }

        /************************************************************************************************************************/

        private void Initialize(Func<object, object> getter, bool isStatic, MemberInfo member)
        {
            _Getter = getter;
            _IsStatic = isStatic;

            if (Label == null)
                Label = IGUtils.ConvertFieldNameToFriendly(member.Name, true);

            if (Tooltip == null)
                Tooltip = "[Label] " + member.GetNameCS();

            var attributes = member.GetCustomAttributes(typeof(ColorAttribute), true);
            if (attributes != null && attributes.Length > 0)
                _Color = (attributes[0] as ColorAttribute).Color;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void OnGUI(Object[] targets)
        {
            // Get the target object.
            if (Event.current.type == EventType.Layout)
            {
                try
                {
                    _Value = _Getter(_IsStatic ? null : targets[0]);

                    _LabelValue = _Value != null ? _Value.ToString() : "null";
                    _Exception = null;
                }
                catch (Exception exception)
                {
                    _LabelValue = exception.GetBaseException().GetType().Name;
                    _Exception = exception;
                }
            }

            if (HideWhenNull && _Value == null)
                return;

            // Draw the label.
            var oldColor = GUI.color;
            if (_Color != null) GUI.color = _Color.Value;

            Rect area;

            if (LargeMode)
            {
                area = EditorGUILayout.BeginVertical();
                {
                    GUILayout.Label(LabelContent, LabelStyle);
                    GUILayout.Label(_LabelValue, EditorStyles.wordWrappedLabel);
                }
                EditorGUILayout.EndVertical();
            }
            else
            {
                area = PrefixLabel(LabelContent);
                GUI.Label(area, _LabelValue, EditorStyles.wordWrappedLabel);
                area = GUILayoutUtility.GetLastRect();
            }

            GUI.color = oldColor;

            CheckContextMenu(area, targets);

            if (ConstantlyRepaint)
                HandleUtility.Repaint();
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override void PopulateContextMenu(GenericMenu menu, Object[] targets)
        {
            menu.AddItem(new GUIContent("Copy to Clipboard"), false, () =>
            {
                EditorGUIUtility.systemCopyBuffer = _LabelValue;
            });

            menu.AddItem(new GUIContent("Log Value"), false, () =>
            {
                if (_Exception == null)
                {
                    Debug.Log($"{Member.GetNameCS()}: {_LabelValue}", targets[0]);
                }
                else
                {
                    Debug.LogException(_Exception, targets[0]);
                }
            });
        }

        /************************************************************************************************************************/
#endif
    }
}

