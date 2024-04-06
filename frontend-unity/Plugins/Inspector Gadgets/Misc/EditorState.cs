// Inspector Gadgets // https://kybernetik.com.au/inspector-gadgets // Copyright 2021 Kybernetik //

using UnityEngine;

namespace InspectorGadgets
{
    /// <summary>Represents a Unity Editor state which can be used as a condition.</summary>
    public enum EditorState
    {
        /// <summary>All the time, regardless of the current state of the Unity Editor.</summary>
        Always,

        /// <summary>When the Unity Editor is in Play Mode or in a Runtime Build.</summary>
        Playing,

        /// <summary>When the Unity Editor is not in Play Mode.</summary>
        Editing,
    }

    public static partial class IGUtils
    {
        /************************************************************************************************************************/

        private const string DefaultEditorStatePrefKey = nameof(InspectorGadgets) + "." + nameof(DefaultEditorState);

        /// <summary>Determines when to show Inspectable attributes if not specified in their constructor.</summary>
        /// <remarks>This value is stored in <see cref="PlayerPrefs"/>.</remarks>
        public static EditorState DefaultEditorState
        {
            get => (EditorState)PlayerPrefs.GetInt(DefaultEditorStatePrefKey);
            set => PlayerPrefs.SetInt(DefaultEditorStatePrefKey, (int)value);
        }

        /// <summary>
        /// Returns the `state` as long as it isn't <c>null</c>.
        /// Otherwise returns the <see cref="DefaultEditorState"/>.
        /// </summary>
        public static EditorState ValueOrDefault(this EditorState? state)
            => state != null ? state.Value : DefaultEditorState;

        /// <summary>Returns true if the Unity Editor is currently in the specified `state`.</summary>
        public static bool IsNow(this EditorState state)
        {
            switch (state)
            {
#if UNITY_EDITOR
                case EditorState.Playing:
                    return UnityEditor.EditorApplication.isPlaying;
                case EditorState.Editing:
                    return !UnityEditor.EditorApplication.isPlaying;
#else
                case EditorState.Playing:
                    return true;
                case EditorState.Editing:
                    return false;
#endif
                case EditorState.Always:
                default:
                    return true;
            }
        }

        /************************************************************************************************************************/
    }
}

