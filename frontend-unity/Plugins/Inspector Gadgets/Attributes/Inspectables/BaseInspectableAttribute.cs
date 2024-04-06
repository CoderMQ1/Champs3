// Inspector Gadgets // https://kybernetik.com.au/inspector-gadgets // Copyright 2021 Kybernetik //

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InspectorGadgets.Attributes
{
    /// <summary>[Pro-Only]
    /// <see cref="Editor.Editor{T}"/> uses these attributes to add extra elements to the inspector.
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public abstract class BaseInspectableAttribute : Attribute, IComparable<BaseInspectableAttribute>
    {
        /************************************************************************************************************************/

        /// <summary>The label to use as a prefix before the value. If not set, it will use the name of the attributed member.</summary>
        public string Label { get; set; }

        /// <summary>The tooltip to use as for the label. If not set, it will use the full name of the attributed member.</summary>
        public string Tooltip { get; set; }

        private GUIContent _LabelContent;

        /// <summary>
        /// The <see cref="GUIContent"/> used for this inspectable's label, creates from the <see cref="Label"/> and
        /// <see cref="Tooltip"/>.
        /// </summary>
        public GUIContent LabelContent
        {
            get
            {
                if (_LabelContent == null)
                    _LabelContent = new GUIContent(Label, Tooltip);
                return _LabelContent;
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// If set, this inspectable will be drawn at the specified index amongst the regular serialized fields instead
        /// of after them.
        /// </summary>
        public int DisplayIndex { get; set; } = int.MaxValue;

        /************************************************************************************************************************/

        private EditorState? _When;

        /// <summary>Determines when this attribute should be active.</summary>
        public EditorState When
        {
            get => _When.ValueOrDefault();
            set => _When = value;
        }

        /************************************************************************************************************************/

        /// <summary>Compares the <see cref="DisplayIndex"/> of this inspectable to the specified `other`.</summary>
        public int CompareTo(BaseInspectableAttribute other)
            => DisplayIndex.CompareTo(other.DisplayIndex);

        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        private static readonly Dictionary<Type, List<BaseInspectableAttribute>>
            AllInspectables = new Dictionary<Type, List<BaseInspectableAttribute>>();

        /************************************************************************************************************************/

        internal static List<BaseInspectableAttribute> Gather(Type type)
        {
            if (!AllInspectables.TryGetValue(type, out var inspectables))
            {
                inspectables = new List<BaseInspectableAttribute>();

                if (type.BaseType != null)
                {
                    inspectables.AddRange(Gather(type.BaseType));
                }

                Gather(type, inspectables, Editor.IGEditorUtils.AnyAccessBindings);

                AllInspectables.Add(type, inspectables);
            }

            return inspectables;
        }

        /************************************************************************************************************************/

        private static readonly List<FieldInfo> Fields = new List<FieldInfo>();
        private static readonly List<PropertyInfo> Properties = new List<PropertyInfo>();
        private static readonly List<MethodInfo> Methods = new List<MethodInfo>();

        private static void Gather(Type type, List<BaseInspectableAttribute> inspectables, BindingFlags bindings)
        {
            Fields.Clear();
            Properties.Clear();
            Methods.Clear();

            var first = inspectables.Count;
            var index = first;

            // Fields.
            IGUtils.GetAttributedFields(type, bindings, inspectables, Fields);
            for (int i = 0; i < Fields.Count; i++)
            {
                Initialize(inspectables, first, ref index, Fields[i]);
            }

            // Properties.
            IGUtils.GetAttributedProperties(type, bindings, inspectables, Properties);
            for (int i = 0; i < Properties.Count; i++)
            {
                Initialize(inspectables, first, ref index, Properties[i]);
            }

            // Methods.
            IGUtils.GetAttributedMethods(type, bindings, inspectables, Methods);
            for (int i = 0; i < Methods.Count; i++)
            {
                Initialize(inspectables, first, ref index, Methods[i]);
            }

            // Sort by DisplayIndex.
            IGUtils.StableInsertionSort(inspectables);
        }

        /************************************************************************************************************************/

        private static void Initialize(
            List<BaseInspectableAttribute> inspectables,
            int first,
            ref int index,
            MemberInfo member)
        {
            for (int i = 0; i < first; i++)
            {
                if (inspectables[i].Member.MetadataToken == member.MetadataToken)
                {
                    inspectables.RemoveAt(index);
                    return;
                }
            }

            var inspectable = inspectables[index];
            var error = inspectable.Initialize(member);
            if (error != null)
            {
                inspectables.RemoveAt(index);
                inspectable.LogInvalidMember(error);
                return;
            }

            index++;
        }

        /************************************************************************************************************************/

        /// <summary>The attributed member.</summary>
        public MemberInfo Member { get; private set; }

        private string Initialize(MemberInfo member)
        {
            Member = member;
            return Initialize();
        }

        /// <summary>Initializes this inspectable with a member.</summary>
        protected abstract string Initialize();

        /************************************************************************************************************************/

        /// <summary>Logs a warning that the specified `member` can't have this kind of attribute for the given `reason`.</summary>
        private void LogInvalidMember(string reason)
        {
            var text = new StringBuilder();
            text.Append("The member: ");
            text.Append(Member.DeclaringType.FullName);
            text.Append('.');
            text.Append(Member.Name);
            text.Append(" cannot have a [");
            text.Append(GetType().FullName);
            text.Append("] attribute because ");
            text.Append(reason);
            text.Append('.');
            Debug.LogWarning(text.ToString());
        }

        /************************************************************************************************************************/

        /// <summary>Draws this inspectable using <see cref="GUILayout"/>.</summary>
        public abstract void OnGUI(Object[] targets);

        /************************************************************************************************************************/

        /// <summary>
        /// If <see cref="Event.current"/> is a Context Click within the `area`, this method creates a menu, calls
        /// <see cref="PopulateContextMenu"/>, and shows it as a context menu.
        /// </summary>
        protected void CheckContextMenu(Rect area, Object[] targets)
        {
            var currentEvent = Event.current;
            if (currentEvent.type != EventType.ContextClick ||
                !area.Contains(currentEvent.mousePosition))
                return;

            var menu = new UnityEditor.GenericMenu();
            PopulateContextMenu(menu, targets);
            menu.ShowAsContext();
        }

        /// <summary>Adds various items to the `menu` relating to the `targets`.</summary>
        protected virtual void PopulateContextMenu(UnityEditor.GenericMenu menu, Object[] targets) { }

        /************************************************************************************************************************/

        /// <summary>
        /// Draws a label like <see cref="UnityEditor.EditorGUILayout.PrefixLabel(string)"/> but doesn't get greyed out
        /// if the GUI is disabled for the following control.
        /// </summary>
        public static Rect PrefixLabel(GUIContent label)
        {
            var area = GUILayoutUtility.GetRect(0, UnityEditor.EditorGUIUtility.singleLineHeight, LabelStyle);

            var labelWidth = UnityEditor.EditorGUIUtility.labelWidth + LabelStyle.padding.horizontal;
            labelWidth -= UnityEditor.EditorGUI.indentLevel * Editor.IGEditorUtils.IndentSize;
            var labelArea = Editor.IGEditorUtils.StealFromLeft(ref area, labelWidth);
            labelArea = UnityEditor.EditorGUI.IndentedRect(labelArea);

            GUI.Label(labelArea, label, LabelStyle);
            return area;
        }

        /************************************************************************************************************************/

        private static GUIStyle _LabelStyle;

        /// <summary>
        /// Returns a style based on the default label with the font set to italic.
        /// </summary>
        public static GUIStyle LabelStyle
        {
            get
            {
                if (_LabelStyle == null)
                {
                    _LabelStyle = new GUIStyle(UnityEditor.EditorStyles.label)
                    {
                        fontStyle = FontStyle.Italic
                    };
                }

                return _LabelStyle;
            }
        }

        /************************************************************************************************************************/
#endif
    }
}

