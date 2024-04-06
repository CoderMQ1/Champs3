// Inspector Gadgets // https://kybernetik.com.au/inspector-gadgets // Copyright 2021 Kybernetik //

using UnityEngine;

namespace InspectorGadgets
{
    /// <summary>Common interface for objects that are displayed by the <see cref="Editor.CommentEditor"/>.</summary>
    public interface IComment
    {
        /************************************************************************************************************************/

        /// <summary>The text of this comment.</summary>
        string Text { get; set; }

#if UNITY_EDITOR
        /// <summary>[Editor-Only] The name of the serialized backing field of the <see cref="Text"/> property.</summary>
        string TextFieldName { get; }
#endif

        /// <summary>False if this script is set to <see cref="HideFlags.DontSaveInBuild"/>.</summary>
        bool IncludeInBuild { get; set; }

        /************************************************************************************************************************/
    }
}

namespace InspectorGadgets.Editor
{
    /************************************************************************************************************************/
#if UNITY_EDITOR
    /************************************************************************************************************************/

    /// <summary>[Editor-Only] Custom Inspector for <see cref="IComment"/>s.</summary>
    [UnityEditor.CanEditMultipleObjects]
    public class CommentEditor : UnityEditor.Editor
    {
        /************************************************************************************************************************/

        private UnityEditor.SerializedProperty _Text;

        /************************************************************************************************************************/

        private void OnEnable()
        {
            _Text = serializedObject.FindProperty(((IComment)target).TextFieldName);
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var style = IGEditorUtils.GetCachedStyle(() => new GUIStyle(UnityEditor.EditorStyles.textArea)
            {
                wordWrap = true
            });

            GUILayout.BeginHorizontal();
            GUILayout.Space(-10);

            UnityEditor.EditorGUI.BeginChangeCheck();
            var text = UnityEditor.EditorGUILayout.TextArea(_Text.stringValue, style, GUILayout.ExpandHeight(true));
            if (UnityEditor.EditorGUI.EndChangeCheck())
                _Text.stringValue = IGUtils.TruncateForLabel(text);

            GUILayout.EndHorizontal();

            // Include in Build.
            var includeSome = false;
            var excludeSome = false;
            foreach (var target in targets)
            {
                if ((target as IComment).IncludeInBuild)
                    includeSome = true;
                else
                    excludeSome = true;
            }

            if (includeSome)
            {
                if (excludeSome)
                    UnityEditor.EditorGUI.showMixedValue = true;

                UnityEditor.EditorGUI.BeginChangeCheck();
                includeSome = UnityEditor.EditorGUILayout.Toggle("Include in Build?", true);
                if (UnityEditor.EditorGUI.EndChangeCheck())
                    foreach (var target in targets)
                        (target as IComment).IncludeInBuild = includeSome;

                UnityEditor.EditorGUI.showMixedValue = false;
            }

            serializedObject.ApplyModifiedProperties();
        }

        /************************************************************************************************************************/

        [UnityEditor.MenuItem(Strings.CommentAssetIncludeInBuild)]
        [UnityEditor.MenuItem(Strings.CommentComponentIncludeInBuild)]
        private static void IncludeInBuild(UnityEditor.MenuCommand command)
        {
            (command.context as IComment).IncludeInBuild = true;
        }

        /************************************************************************************************************************/
    }

    /************************************************************************************************************************/
#endif
    /************************************************************************************************************************/
}

