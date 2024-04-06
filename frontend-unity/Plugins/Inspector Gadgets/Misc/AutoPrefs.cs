// Inspector Gadgets // https://kybernetik.com.au/inspector-gadgets // Copyright 2021 Kybernetik //

// The missing methods these warnings complain about are implemented by the child types, so they aren't actually missing.
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)

using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace InspectorGadgets
{
    /// <summary>
    /// A collection of wrappers for PlayerPrefs and EditorPrefs which simplify the way you can store and retrieve values.
    /// </summary>
    public static class AutoPrefs
    {
        /************************************************************************************************************************/

        /// <summary>An object which encapsulates a pref value stored with a specific key.</summary>
        public interface IAutoPref
        {
            /// <summary>The key used to identify this pref.</summary>
            string Key { get; }

            /// <summary>The current value of this pref.</summary>
            object Value { get; }
        }

        /************************************************************************************************************************/

        /// <summary>An object which encapsulates a pref value stored with a specific key.</summary>
        public abstract class AutoPref<T> : IAutoPref
        {
            /************************************************************************************************************************/
            #region Fields and Properties
            /************************************************************************************************************************/

            /// <summary>The key used to identify this pref.</summary>
            public readonly string Key;

            /// <summary>The default value to use if this pref has no existing value.</summary>
            public readonly T DefaultValue;

            /// <summary>Called when the <see cref="Value"/> is changed.</summary>
            public readonly Action<T> OnValueChanged;

            /************************************************************************************************************************/

            private bool _IsLoaded;
            private T _Value;

            /// <summary>The current value of this pref.</summary>
            public T Value
            {
                get
                {
                    if (!_IsLoaded)
                        Reload();

                    return _Value;
                }
                set
                {
                    if (!_IsLoaded)
                    {
                        if (!IsSaved())
                        {
                            // If there is no saved value, set the value and make sure it is saved.
                            _Value = value;
                            _IsLoaded = true;
                            Save();

                            OnValueChanged?.Invoke(value);

#if UNITY_EDITOR
                            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
#endif
                            return;
                        }
                        else Reload();
                    }

                    // Otherwise store and save the new value if it is different.
                    if (!Equals(_Value, value))
                    {
                        _Value = value;
                        Save();

                        OnValueChanged?.Invoke(value);

#if UNITY_EDITOR
                        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
#endif
                    }
                }
            }

            /************************************************************************************************************************/

            string IAutoPref.Key => Key;
            object IAutoPref.Value => Value;

            /************************************************************************************************************************/
            #endregion
            /************************************************************************************************************************/
            #region Methods
            /************************************************************************************************************************/

            /// <summary>Constructs an <see cref="AutoPref{T}"/> with the specified `key` and `defaultValue`.</summary>
            protected AutoPref(string key, T defaultValue, Action<T> onValueChanged)
            {
                Key = key;
                DefaultValue = defaultValue;
                OnValueChanged = onValueChanged;
            }

            /// <summary>Loads the value of this pref from the system.</summary>
            protected abstract T Load();

            /// <summary>Saves the value of this pref to the system.</summary>
            protected abstract void Save();

            /************************************************************************************************************************/

            /// <summary>Returns the current value of this pref.</summary>
            public static implicit operator T(AutoPref<T> pref) => pref.Value;

            /************************************************************************************************************************/

            /// <summary>Checks if the value of this pref is equal to the specified `value`.</summary>
            public static bool operator ==(AutoPref<T> pref, T value) => Equals(pref.Value, value);

            /// <summary>Checks if the value of this pref is not equal to the specified `value`.</summary>
            public static bool operator !=(AutoPref<T> pref, T value) => !(pref == value);

            /************************************************************************************************************************/

            /// <summary>Reloads the value of this pref from the system.</summary>
            public void Reload()
            {
                _Value = Load();
                _IsLoaded = true;
            }

            /************************************************************************************************************************/
            #endregion
            /************************************************************************************************************************/
            #region Utils
            /************************************************************************************************************************/

            /// <summary>Returns a hash code for the current pref value.</summary>
            public override int GetHashCode() => base.GetHashCode();

            /************************************************************************************************************************/

            /// <summary>Returns true if the preferences currently contains a saved value for this pref.</summary>
            public virtual bool IsSaved() => PlayerPrefs.HasKey(Key);

            /************************************************************************************************************************/

            /// <summary>Deletes the value of this pref from the preferences and reverts to the default value.</summary>
            public virtual void DeletePref()
            {
                PlayerPrefs.DeleteKey(Key);
                RevertToDefaultValue();
            }

            /// <summary>Sets the <see cref="Value"/> = <see cref="DefaultValue"/>.</summary>
            protected void RevertToDefaultValue()
            {
                if (!Equals(_Value, DefaultValue))
                {
                    _Value = DefaultValue;

                    OnValueChanged?.Invoke(DefaultValue);

#if UNITY_EDITOR
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
#endif
                }
            }

            /************************************************************************************************************************/

            /// <summary>Returns <c>Value?.ToString()</c>.</summary>
            public override string ToString() => Value?.ToString();

            /************************************************************************************************************************/
#if UNITY_EDITOR
            /************************************************************************************************************************/

            /// <summary>[Editor-Only] A delegate used to draw a GUI field and return its value.</summary>
            public delegate T GUIFieldMethod(Rect area, GUIContent content, GUIStyle style);

            /************************************************************************************************************************/

            /// <summary>[Editor-Only] Draws GUI controls for this pref and returns true if the value was changed.</summary>
            public virtual bool OnGUI(Rect area, GUIContent content, GUIStyle style, GUIFieldMethod doGUIField)
            {
                var isDefault = Equals(Value, DefaultValue);
                if (!isDefault)
                    area.width -= Editor.InternalGUI.MiniSquareButtonStyle.fixedWidth;

                EditorGUI.BeginChangeCheck();
                var value = doGUIField(area, content, style);
                if (EditorGUI.EndChangeCheck())
                {
                    Value = value;
                    return true;
                }

                if (!isDefault)
                {
                    var resetStyle = Editor.InternalGUI.MiniSquareButtonStyle;
                    var resetPosition = new Rect(area.xMax, area.y, resetStyle.fixedWidth, resetStyle.fixedHeight);

                    if (GUI.Button(resetPosition, Strings.GUI.Reset, resetStyle))
                    {
                        Value = DefaultValue;
                        return true;
                    }
                }

                return false;
            }

            /// <summary>[Editor-Only] Draws GUI controls for this pref and returns true if the value was changed.</summary>
            public virtual bool OnGUI(Rect area, GUIContent content, GUIStyle style)
                => OnGUI(area, content, style, DoGUIField);

            /// <summary>[Editor-Only]
            /// Draws the default GUI style used by this pref if none is specified when calling
            /// <see cref="OnGUI(Rect, GUIContent, GUIStyle)"/>.
            /// </summary>
            public virtual GUIStyle DefaultStyle => EditorStyles.label;

            /// <summary>[Editor-Only] Draws a GUI field for this pref and returns the value it is set to.</summary>
            public virtual T DoGUIField(Rect area, GUIContent content, GUIStyle style)
            {
                EditorGUI.LabelField(area, content, new GUIContent(Value.ToString()), style);
                return Value;
            }

            /************************************************************************************************************************/

            /// <summary>
            /// Uses <see cref="EditorGUILayout.GetControlRect(bool, float, GUIStyle, GUILayoutOption[])"/>
            /// to allocate a <see cref="GUILayout"/> <see cref="Rect"/> for a control.
            /// </summary>
            public static Rect GetControlRect(GUIStyle style, params GUILayoutOption[] options)
                => EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, style, options);

            /// <summary>
            /// Uses <see cref="EditorGUILayout.GetControlRect(bool, float, GUIStyle, GUILayoutOption[])"/>
            /// to allocate a <see cref="GUILayout"/> <see cref="Rect"/> for a control.
            /// </summary>
            public Rect GetControlRect(params GUILayoutOption[] options)
                => GetControlRect(DefaultStyle, options);

            /************************************************************************************************************************/

            /// <summary>[Editor-Only] Draws GUI controls for this pref and returns true if the value was changed.</summary>
            public bool OnGUI(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
                => OnGUI(GetControlRect(style, options), content, style);

            /// <summary>[Editor-Only] Draws GUI controls for this pref and returns true if the value was changed.</summary>
            public bool OnGUI(GUIContent content, GUIStyle style, GUIFieldMethod doGUIField, params GUILayoutOption[] options)
                => OnGUI(GetControlRect(style, options), content, style, doGUIField);

            /// <summary>[Editor-Only] Draws GUI controls for this pref and returns true if the value was changed.</summary>
            public bool OnGUI(string text, GUIStyle style, params GUILayoutOption[] options)
                => OnGUI(new GUIContent(text), style, options);

            /// <summary>[Editor-Only] Draws GUI controls for this pref and returns true if the value was changed.</summary>
            public bool OnGUI(GUIContent content, params GUILayoutOption[] options)
                => OnGUI(content, DefaultStyle, options);

            /// <summary>[Editor-Only] Draws GUI controls for this pref and returns true if the value was changed.</summary>
            public bool OnGUI(GUIContent content, GUIFieldMethod doGUIField, params GUILayoutOption[] options)
                => OnGUI(content, DefaultStyle, doGUIField, options);

            /// <summary>[Editor-Only] Draws GUI controls for this pref and returns true if the value was changed.</summary>
            public bool OnGUI(string text, params GUILayoutOption[] options)
                => OnGUI(new GUIContent(text), options);

            /************************************************************************************************************************/
#endif
            /************************************************************************************************************************/
            #endregion
            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        /// <summary>
        /// An <see cref="AutoPref{T}"/> which encapsulates a <see cref="bool"/> value stored in
        /// <see cref="PlayerPrefs"/>.
        /// </summary>
        public class Bool : AutoPref<bool>
        {
            /************************************************************************************************************************/
            #region Inherited Methods
            /************************************************************************************************************************/

            /// <summary>Constructs a <see cref="Bool"/> pref with the specified `key` and `defaultValue`.</summary>
            public Bool(string key, bool defaultValue = default, Action<bool> onValueChanged = null)
                : base(key, defaultValue, onValueChanged)
            { }

            /// <summary>Loads the value of this pref from <see cref="PlayerPrefs"/>.</summary>
            protected override bool Load() => PlayerPrefs.GetInt(Key, DefaultValue ? 1 : 0) > 0;

            /// <summary>Saves the value of this pref to <see cref="PlayerPrefs"/>.</summary>
            protected override void Save() => PlayerPrefs.SetInt(Key, Value ? 1 : 0);

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="Bool"/> pref using the specified string as the key.</summary>
            public static implicit operator Bool(string key) => new Bool(key);

            /************************************************************************************************************************/
            #endregion
            /************************************************************************************************************************/
            #region Utils
            /************************************************************************************************************************/

            /// <summary>Toggles the value of this pref from false to true or vice versa.</summary>
            public void Invert() => Value = !Value;

            /************************************************************************************************************************/
#if UNITY_EDITOR
            /************************************************************************************************************************/

            /// <summary>[Editor-Only] Draws a GUI field for this pref and returns the value it is set to.</summary>
            public override bool DoGUIField(Rect area, GUIContent content, GUIStyle style)
                => EditorGUI.Toggle(area, content, Value, style);

            /// <summary>[Editor-Only]
            /// Draws the default GUI style used by this pref if none is specified when calling
            /// <see cref="AutoPref{T}.OnGUI(Rect, GUIContent, GUIStyle)"/>.
            /// </summary>
            public override GUIStyle DefaultStyle => EditorStyles.toggle;

            /************************************************************************************************************************/
#endif
            /************************************************************************************************************************/
            #endregion
            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        /// <summary>
        /// An <see cref="AutoPref{T}"/> which encapsulates a <see cref="float"/> value stored in
        /// <see cref="PlayerPrefs"/>.</summary>
        public class Float : AutoPref<float>
        {
            /************************************************************************************************************************/

            /// <summary>Constructs a <see cref="Float"/> pref with the specified `key` and `defaultValue`.</summary>
            public Float(string key, float defaultValue = default, Action<float> onValueChanged = null)
                : base(key, defaultValue, onValueChanged)
            { }

            /// <summary>Loads the value of this pref from <see cref="PlayerPrefs"/>.</summary>
            protected override float Load() => PlayerPrefs.GetFloat(Key, DefaultValue);

            /// <summary>Saves the value of this pref to <see cref="PlayerPrefs"/>.</summary>
            protected override void Save() => PlayerPrefs.SetFloat(Key, Value);

            /************************************************************************************************************************/
            #region Operators
            /************************************************************************************************************************/

            /// <summary>Checks if the value of this pref is greater then the specified `value`.</summary>
            public static bool operator >(Float pref, float value) => pref.Value > value;

            /// <summary>Checks if the value of this pref is less then the specified `value`.</summary>
            public static bool operator <(Float pref, float value) => pref.Value < value;

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="Float"/> pref using the specified string as the key.</summary>
            public static implicit operator Float(string key) => new Float(key);

            /************************************************************************************************************************/
            #endregion
            /************************************************************************************************************************/
#if UNITY_EDITOR
            /************************************************************************************************************************/

            /// <summary>[Editor-Only] Draws a GUI field for this pref and returns the value it is set to.</summary>
            public override float DoGUIField(Rect area, GUIContent content, GUIStyle style)
                => EditorGUI.FloatField(area, content, Value, style);

            /// <summary>[Editor-Only]
            /// Draws the default GUI style used by this pref if none is specified when calling
            /// <see cref="AutoPref{T}.OnGUI(Rect, GUIContent, GUIStyle)"/>.
            /// </summary>
            public override GUIStyle DefaultStyle => EditorStyles.numberField;

            /************************************************************************************************************************/
#endif
            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        /// <summary>
        /// An <see cref="AutoPref{T}"/> which encapsulates a <see cref="int"/> value stored in
        /// <see cref="PlayerPrefs"/>.
        /// </summary>
        public class Int : AutoPref<int>
        {
            /************************************************************************************************************************/

            /// <summary>Constructs an <see cref="Int"/> pref with the specified `key` and `defaultValue`.</summary>
            public Int(string key, int defaultValue = default, Action<int> onValueChanged = null)
                : base(key, defaultValue, onValueChanged)
            { }

            /// <summary>Loads the value of this pref from <see cref="PlayerPrefs"/>.</summary>
            protected override int Load() => PlayerPrefs.GetInt(Key, DefaultValue);

            /// <summary>Saves the value of this pref to <see cref="PlayerPrefs"/>.</summary>
            protected override void Save() => PlayerPrefs.SetInt(Key, Value);

            /************************************************************************************************************************/
            #region Operators
            /************************************************************************************************************************/

            /// <summary>Checks if the value of this pref is greater then the specified `value`.</summary>
            public static bool operator >(Int pref, int value) => pref.Value > value;

            /// <summary>Checks if the value of this pref is less then the specified `value`.</summary>
            public static bool operator <(Int pref, int value) => pref.Value < value;

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="Int"/> pref using the specified string as the key.</summary>
            public static implicit operator Int(string key) => new Int(key);

            /************************************************************************************************************************/
            #endregion
            /************************************************************************************************************************/
#if UNITY_EDITOR
            /************************************************************************************************************************/

            /// <summary>[Editor-Only] Draws a GUI field for this pref and returns the value it is set to.</summary>
            public override int DoGUIField(Rect area, GUIContent content, GUIStyle style)
                => EditorGUI.IntField(area, content, Value, style);

            /// <summary>[Editor-Only]
            /// Draws the default GUI style used by this pref if none is specified when calling
            /// <see cref="AutoPref{T}.OnGUI(Rect, GUIContent, GUIStyle)"/>.
            /// </summary>
            public override GUIStyle DefaultStyle => EditorStyles.numberField;

            /************************************************************************************************************************/
#endif
            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        /// <summary>
        /// An <see cref="AutoPref{T}"/> which encapsulates a <see cref="string"/> value stored in
        /// <see cref="PlayerPrefs"/>.
        /// </summary>
        public class String : AutoPref<string>
        {
            /************************************************************************************************************************/

            /// <summary>Constructs a <see cref="String"/> pref with the specified `key` and `defaultValue`.</summary>
            public String(string key, string defaultValue = default, Action<string> onValueChanged = null)
                : base(key, defaultValue, onValueChanged)
            { }

            /// <summary>Loads the value of this pref from <see cref="PlayerPrefs"/>.</summary>
            protected override string Load() => PlayerPrefs.GetString(Key, DefaultValue);

            /// <summary>Saves the value of this pref to <see cref="PlayerPrefs"/>.</summary>
            protected override void Save() => PlayerPrefs.SetString(Key, Value);

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="String"/> pref using the specified string as the key.</summary>
            public static implicit operator String(string key) => new String(key);

            /************************************************************************************************************************/
#if UNITY_EDITOR
            /************************************************************************************************************************/

            /// <summary>[Editor-Only] Draws a GUI field for this pref and returns the value it is set to.</summary>
            public override string DoGUIField(Rect area, GUIContent content, GUIStyle style)
                => EditorGUI.TextField(area, content, Value, style);

            /// <summary>[Editor-Only]
            /// Draws the default GUI style used by this pref if none is specified when calling
            /// <see cref="AutoPref{T}.OnGUI(Rect, GUIContent, GUIStyle)"/>.
            /// </summary>
            public override GUIStyle DefaultStyle => EditorStyles.textField;

            /************************************************************************************************************************/
#endif
            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        /// <summary>
        /// An <see cref="AutoPref{T}"/> which encapsulates a <see cref="UnityEngine.Vector2"/> value stored in
        /// <see cref="PlayerPrefs"/>.
        /// </summary>
        public class Vector2 : AutoPref<UnityEngine.Vector2>
        {
            /************************************************************************************************************************/

            /// <summary>The key used to identify the x value of this pref.</summary>
            public string KeyX => Key;

            /// <summary>The key used to identify the y value of this pref.</summary>
            public readonly string KeyY;

            /************************************************************************************************************************/

            /// <summary>Constructs a <see cref="Vector2"/> pref with the specified `key` and `defaultValue`.</summary>
            public Vector2(string key,
                UnityEngine.Vector2 defaultValue = default,
                Action<UnityEngine.Vector2> onValueChanged = null)
                : base(key + "X", defaultValue, onValueChanged)
            {
                KeyY = key + "Y";
            }

            /// <summary>Loads the value of this pref from <see cref="PlayerPrefs"/>.</summary>
            protected override UnityEngine.Vector2 Load() => new UnityEngine.Vector2(
                PlayerPrefs.GetFloat(Key, DefaultValue.x),
                PlayerPrefs.GetFloat(KeyY, DefaultValue.y));

            /// <summary>Saves the value of this pref to <see cref="PlayerPrefs"/>.</summary>
            protected override void Save()
            {
                PlayerPrefs.SetFloat(Key, Value.x);
                PlayerPrefs.SetFloat(KeyY, Value.y);
            }

            /************************************************************************************************************************/

            /// <summary>Returns true if <see cref="PlayerPrefs"/> currently contains a value for this pref.</summary>
            public override bool IsSaved() =>
                PlayerPrefs.HasKey(Key) &&
                PlayerPrefs.HasKey(KeyY);

            /// <summary>Deletes the value of this pref from <see cref="PlayerPrefs"/> and reverts to the default value.</summary>
            public override void DeletePref()
            {
                PlayerPrefs.DeleteKey(Key);
                PlayerPrefs.DeleteKey(KeyY);
                RevertToDefaultValue();
            }

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="Vector2"/> pref using the specified string as the key.</summary>
            public static implicit operator Vector2(string key) => new Vector2(key);

            /************************************************************************************************************************/
#if UNITY_EDITOR
            /************************************************************************************************************************/

            /// <summary>[Editor-Only] Draws a GUI field for this pref and returns the value it is set to.</summary>
            public override UnityEngine.Vector2 DoGUIField(Rect area, GUIContent content, GUIStyle style)
                => EditorGUI.Vector2Field(area, content, Value);

            /// <summary>[Editor-Only]
            /// Draws the default GUI style used by this pref if none is specified when calling
            /// <see cref="AutoPref{T}.OnGUI(Rect, GUIContent, GUIStyle)"/>.
            /// </summary>
            public override GUIStyle DefaultStyle => null;

            /************************************************************************************************************************/
#endif
            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        /// <summary>
        /// An <see cref="AutoPref{T}"/> which encapsulates a <see cref="UnityEngine.Vector3"/> value stored in
        /// <see cref="PlayerPrefs"/>.
        /// </summary>
        public class Vector3 : AutoPref<UnityEngine.Vector3>
        {
            /************************************************************************************************************************/

            /// <summary>The key used to identify the x value of this pref.</summary>
            public string KeyX => Key;

            /// <summary>The key used to identify the y value of this pref.</summary>
            public readonly string KeyY;

            /// <summary>The key used to identify the z value of this pref.</summary>
            public readonly string KeyZ;

            /************************************************************************************************************************/

            /// <summary>Constructs a <see cref="Vector3"/> pref with the specified `key` and `defaultValue`.</summary>
            public Vector3(string key,
                UnityEngine.Vector3 defaultValue = default,
                Action<UnityEngine.Vector3> onValueChanged = null)
                : base(key + "X", defaultValue, onValueChanged)
            {
                KeyY = key + "Y";
                KeyZ = key + "Z";
            }

            /// <summary>Loads the value of this pref from <see cref="PlayerPrefs"/>.</summary>
            protected override UnityEngine.Vector3 Load() => new UnityEngine.Vector3(
                PlayerPrefs.GetFloat(Key, DefaultValue.x),
                PlayerPrefs.GetFloat(KeyY, DefaultValue.y),
                PlayerPrefs.GetFloat(KeyZ, DefaultValue.z));

            /// <summary>Saves the value of this pref to <see cref="PlayerPrefs"/>.</summary>
            protected override void Save()
            {
                PlayerPrefs.SetFloat(Key, Value.x);
                PlayerPrefs.SetFloat(KeyY, Value.y);
                PlayerPrefs.SetFloat(KeyZ, Value.z);
            }

            /************************************************************************************************************************/

            /// <summary>Returns true if <see cref="PlayerPrefs"/> currently contains a value for this pref.</summary>
            public override bool IsSaved() =>
                PlayerPrefs.HasKey(Key) &&
                PlayerPrefs.HasKey(KeyY) &&
                PlayerPrefs.HasKey(KeyZ);

            /// <summary>Deletes the value of this pref from <see cref="PlayerPrefs"/> and reverts to the default value.</summary>
            public override void DeletePref()
            {
                PlayerPrefs.DeleteKey(Key);
                PlayerPrefs.DeleteKey(KeyY);
                PlayerPrefs.DeleteKey(KeyZ);
                RevertToDefaultValue();
            }

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="Vector3"/> pref using the specified string as the key.</summary>
            public static implicit operator Vector3(string key) => new Vector3(key);

            /// <summary>Returns a <see cref="Color"/> using the (x, y, z) of the pref as (r, g, b, a = 1).</summary>
            public static implicit operator Color(Vector3 pref)
            {
                var value = pref.Value;
                return new Color(value.x, value.y, value.z, 1);
            }

            /************************************************************************************************************************/
#if UNITY_EDITOR
            /************************************************************************************************************************/

            /// <summary>[Editor-Only] Draws a GUI field for this pref and returns the value it is set to.</summary>
            public override UnityEngine.Vector3 DoGUIField(Rect area, GUIContent content, GUIStyle style)
                => EditorGUI.Vector3Field(area, content, Value);

            /// <summary>[Editor-Only]
            /// Draws the default GUI style used by this pref if none is specified when calling
            /// <see cref="AutoPref{T}.OnGUI(Rect, GUIContent, GUIStyle)"/>.
            /// </summary>
            public override GUIStyle DefaultStyle => null;

            /************************************************************************************************************************/

            /// <summary>[Editor-Only]
            /// Draws a <see cref="Color"/> field for this pref and returns true if the value was changed.
            /// </summary>
            public bool DoColorGUIField(Rect area, GUIContent content)
            {
                return OnGUI(area, content, EditorStyles.colorField, (area2, content2, style2) =>
                {
                    var color = (Color)this;
                    color = EditorGUI.ColorField(area2, content2, color);
                    return new UnityEngine.Vector3(color.r, color.g, color.b);
                });
            }

            /// <summary>[Editor-Only]
            /// Draws a <see cref="Color"/> field for this pref and returns true if the value was changed.
            /// </summary>
            public bool DoColorGUIField(GUIContent content)
            {
                return OnGUI(content, EditorStyles.colorField, (area2, content2, style2) =>
                {
                    var color = (Color)this;
                    color = EditorGUI.ColorField(area2, content2, color);
                    return new UnityEngine.Vector3(color.r, color.g, color.b);
                });
            }

            /************************************************************************************************************************/
#endif
            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        /// <summary>
        /// An <see cref="AutoPref{T}"/> which encapsulates a <see cref="UnityEngine.Vector4"/> value stored in
        /// <see cref="PlayerPrefs"/>.
        /// </summary>
        public class Vector4 : AutoPref<UnityEngine.Vector4>
        {
            /************************************************************************************************************************/

            /// <summary>The key used to identify the x value of this pref.</summary>
            public string KeyX => Key;

            /// <summary>The key used to identify the y value of this pref.</summary>
            public readonly string KeyY;

            /// <summary>The key used to identify the z value of this pref.</summary>
            public readonly string KeyZ;

            /// <summary>The key used to identify the w value of this pref.</summary>
            public readonly string KeyW;

            /************************************************************************************************************************/

            /// <summary>Constructs a <see cref="Vector4"/> pref with the specified `key` and `defaultValue`.</summary>
            public Vector4(string key,
                UnityEngine.Vector4 defaultValue = default,
                Action<UnityEngine.Vector4> onValueChanged = null)
                : base(key + "X", defaultValue, onValueChanged)
            {
                KeyY = key + "Y";
                KeyZ = key + "Z";
                KeyW = key + "W";
            }

            /// <summary>Loads the value of this pref from <see cref="PlayerPrefs"/>.</summary>
            protected override UnityEngine.Vector4 Load() => new UnityEngine.Vector4(
                PlayerPrefs.GetFloat(Key, DefaultValue.x),
                PlayerPrefs.GetFloat(KeyY, DefaultValue.y),
                PlayerPrefs.GetFloat(KeyZ, DefaultValue.z),
                PlayerPrefs.GetFloat(KeyW, DefaultValue.w));

            /// <summary>Saves the value of this pref to <see cref="PlayerPrefs"/>.</summary>
            protected override void Save()
            {
                PlayerPrefs.SetFloat(Key, Value.x);
                PlayerPrefs.SetFloat(KeyY, Value.y);
                PlayerPrefs.SetFloat(KeyZ, Value.z);
                PlayerPrefs.SetFloat(KeyW, Value.w);
            }

            /************************************************************************************************************************/

            /// <summary>Returns true if <see cref="PlayerPrefs"/> currently contains a value for this pref.</summary>
            public override bool IsSaved() =>
                PlayerPrefs.HasKey(Key) &&
                PlayerPrefs.HasKey(KeyY) &&
                PlayerPrefs.HasKey(KeyZ) &&
                PlayerPrefs.HasKey(KeyW);

            /// <summary>Deletes the value of this pref from <see cref="PlayerPrefs"/> and reverts to the default value.</summary>
            public override void DeletePref()
            {
                PlayerPrefs.DeleteKey(Key);
                PlayerPrefs.DeleteKey(KeyY);
                PlayerPrefs.DeleteKey(KeyZ);
                PlayerPrefs.DeleteKey(KeyW);
                RevertToDefaultValue();
            }

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="Vector4"/> pref using the specified string as the key.</summary>
            public static implicit operator Vector4(string key) => new Vector4(key);

            /// <summary>Returns a <see cref="Color"/> using the (x, y, z, w) of the pref as (r, g, b, a).</summary>
            public static implicit operator Color(Vector4 pref)
            {
                var value = pref.Value;
                return new Color(value.x, value.y, value.z, value.w);
            }

            /************************************************************************************************************************/
#if UNITY_EDITOR
            /************************************************************************************************************************/

            /// <summary>[Editor-Only] Draws a GUI field for this pref and returns the value it is set to.</summary>
            public override UnityEngine.Vector4 DoGUIField(Rect area, GUIContent content, GUIStyle style)
                => EditorGUI.Vector4Field(area, content, Value);

            /// <summary>[Editor-Only]
            /// Draws the default GUI style used by this pref if none is specified when calling
            /// <see cref="AutoPref{T}.OnGUI(Rect, GUIContent, GUIStyle)"/>.
            /// </summary>
            public override GUIStyle DefaultStyle => null;

            /************************************************************************************************************************/

            /// <summary>[Editor-Only]
            /// Draws a <see cref="Color"/> field for this pref and returns true if the value was changed.
            /// </summary>
            public bool DoColorGUIField(Rect area, GUIContent content)
            {
                return OnGUI(area, content, EditorStyles.colorField, (area2, content2, style2) =>
                {
                    var color = (Color)this;
                    color = EditorGUI.ColorField(area2, content2, color);
                    return color;
                });
            }

            /// <summary>[Editor-Only]
            /// Draws a <see cref="Color"/> field for this pref and returns true if the value was changed.
            /// </summary>
            public bool DoColorGUIField(GUIContent content)
            {
                return OnGUI(content, EditorStyles.colorField, (area, content2, style) =>
                {
                    var color = (Color)this;
                    color = EditorGUI.ColorField(area, content2, color);
                    return color;
                });
            }

            /************************************************************************************************************************/
#endif
            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        /// <summary>
        /// An <see cref="AutoPref{T}"/> which <see cref="UnityEngine.Quaternion"/> value stored in
        /// <see cref="PlayerPrefs"/>.
        /// </summary>
        public class Quaternion : AutoPref<UnityEngine.Quaternion>
        {
            /************************************************************************************************************************/

            /// <summary>The key used to identify the x value of this pref.</summary>
            public string KeyX => Key;

            /// <summary>The key used to identify the y value of this pref.</summary>
            public readonly string KeyY;

            /// <summary>The key used to identify the z value of this pref.</summary>
            public readonly string KeyZ;

            /// <summary>The key used to identify the w value of this pref.</summary>
            public readonly string KeyW;

            /************************************************************************************************************************/

            /// <summary>Constructs a <see cref="Quaternion"/> pref with the specified `key` and `defaultValue`.</summary>
            public Quaternion(string key,
                UnityEngine.Quaternion defaultValue = default,
                Action<UnityEngine.Quaternion> onValueChanged = null)
                : base(key + "X", defaultValue, onValueChanged)
            {
                KeyY = key + "Y";
                KeyZ = key + "Z";
                KeyW = key + "W";
            }

            /// <summary>Loads the value of this pref from <see cref="PlayerPrefs"/>.</summary>
            protected override UnityEngine.Quaternion Load() => new UnityEngine.Quaternion(
                PlayerPrefs.GetFloat(Key, DefaultValue.x),
                PlayerPrefs.GetFloat(KeyY, DefaultValue.y),
                PlayerPrefs.GetFloat(KeyZ, DefaultValue.z),
                PlayerPrefs.GetFloat(KeyW, DefaultValue.w));

            /// <summary>Saves the value of this pref to <see cref="PlayerPrefs"/>.</summary>
            protected override void Save()
            {
                PlayerPrefs.SetFloat(Key, Value.x);
                PlayerPrefs.SetFloat(KeyY, Value.y);
                PlayerPrefs.SetFloat(KeyZ, Value.z);
                PlayerPrefs.SetFloat(KeyW, Value.w);
            }

            /************************************************************************************************************************/

            /// <summary>Returns true if <see cref="PlayerPrefs"/> currently contains a value for this pref.</summary>
            public override bool IsSaved() =>
                PlayerPrefs.HasKey(Key) &&
                PlayerPrefs.HasKey(KeyY) &&
                PlayerPrefs.HasKey(KeyZ) &&
                PlayerPrefs.HasKey(KeyW);

            /// <summary>Deletes the value of this pref from <see cref="PlayerPrefs"/> and reverts to the default value.</summary>
            public override void DeletePref()
            {
                PlayerPrefs.DeleteKey(Key);
                PlayerPrefs.DeleteKey(KeyY);
                PlayerPrefs.DeleteKey(KeyZ);
                PlayerPrefs.DeleteKey(KeyW);
                RevertToDefaultValue();
            }

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="Quaternion"/> pref using the specified string as the key.</summary>
            public static implicit operator Quaternion(string key) => new Quaternion(key);

            /************************************************************************************************************************/
#if UNITY_EDITOR
            /************************************************************************************************************************/

            /// <summary>[Editor-Only] Draws a GUI field for this pref and returns the value it is set to.</summary>
            public override UnityEngine.Quaternion DoGUIField(Rect area, GUIContent content, GUIStyle style)
                => UnityEngine.Quaternion.Euler(EditorGUI.Vector3Field(area, content, Value.eulerAngles));

            /// <summary>[Editor-Only]
            /// Draws the default GUI style used by this pref if none is specified when calling
            /// <see cref="AutoPref{T}.OnGUI(Rect, GUIContent, GUIStyle)"/>.
            /// </summary>
            public override GUIStyle DefaultStyle => null;

            /************************************************************************************************************************/
#endif
            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
        #region Editor Prefs
#if UNITY_EDITOR
        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// An <see cref="AutoPref{T}"/> which encapsulates a <see cref="bool"/> value stored in
        /// <see cref="EditorPrefs"/>.
        /// </summary>
        public sealed class EditorBool : Bool
        {
            /************************************************************************************************************************/

            /// <summary>Constructs an <see cref="EditorBool"/> pref with the specified `key` and `defaultValue`.</summary>
            public EditorBool(string key, bool defaultValue = default, Action<bool> onValueChanged = null)
                : base(key, defaultValue, onValueChanged)
            { }

            /// <summary>Loads the value of this pref from <see cref="EditorPrefs"/>.</summary>
            protected override bool Load() => EditorPrefs.GetBool(Key, DefaultValue);

            /// <summary>Saves the value of this pref to <see cref="EditorPrefs"/>.</summary>
            protected override void Save() => EditorPrefs.SetBool(Key, Value);

            /************************************************************************************************************************/

            /// <summary>Returns true if <see cref="EditorPrefs"/> currently contains a value for this pref.</summary>
            public override bool IsSaved() => EditorPrefs.HasKey(Key);

            /// <summary>Deletes the value of this pref from <see cref="EditorPrefs"/> and reverts to the default value.</summary>
            public override void DeletePref()
            {
                EditorPrefs.DeleteKey(Key);
                RevertToDefaultValue();
            }

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="EditorBool"/> pref using the specified string as the key.</summary>
            public static implicit operator EditorBool(string key) => new EditorBool(key);

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// An <see cref="AutoPref{T}"/> which encapsulates a <see cref="float"/> value stored in
        /// <see cref="EditorPrefs"/>.
        /// </summary>
        public sealed class EditorFloat : Float
        {
            /************************************************************************************************************************/

            /// <summary>Constructs an <see cref="EditorFloat"/> pref with the specified `key` and `defaultValue`.</summary>
            public EditorFloat(string key, float defaultValue = default, Action<float> onValueChanged = null)
                : base(key, defaultValue, onValueChanged)
            { }

            /// <summary>Loads the value of this pref from <see cref="EditorPrefs"/>.</summary>
            protected override float Load() => EditorPrefs.GetFloat(Key, DefaultValue);

            /// <summary>Saves the value of this pref to <see cref="EditorPrefs"/>.</summary>
            protected override void Save() => EditorPrefs.SetFloat(Key, Value);

            /************************************************************************************************************************/

            /// <summary>Returns true if <see cref="EditorPrefs"/> currently contains a value for this pref.</summary>
            public override bool IsSaved() => EditorPrefs.HasKey(Key);

            /// <summary>Deletes the value of this pref from <see cref="EditorPrefs"/> and reverts to the default value.</summary>
            public override void DeletePref()
            {
                EditorPrefs.DeleteKey(Key);
                RevertToDefaultValue();
            }

            /************************************************************************************************************************/
            #region Operators
            /************************************************************************************************************************/

            /// <summary>Checks if the value of this pref is greater then the specified `value`.</summary>
            public static bool operator >(EditorFloat pref, float value) => pref.Value > value;

            /// <summary>Checks if the value of this pref is less then the specified `value`.</summary>
            public static bool operator <(EditorFloat pref, float value) => pref.Value < value;

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="EditorFloat"/> pref using the specified string as the key.</summary>
            public static implicit operator EditorFloat(string key) => new EditorFloat(key);

            /************************************************************************************************************************/
            #endregion
            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// An <see cref="AutoPref{T}"/> which encapsulates a <see cref="int"/> value stored in
        /// <see cref="EditorPrefs"/>.
        /// </summary>
        public sealed class EditorInt : Int
        {
            /************************************************************************************************************************/

            /// <summary>Constructs an <see cref="EditorInt"/> pref with the specified `key` and `defaultValue`.</summary>
            public EditorInt(string key, int defaultValue = default, Action<int> onValueChanged = null)
                : base(key, defaultValue, onValueChanged)
            { }

            /// <summary>Loads the value of this pref from <see cref="EditorPrefs"/>.</summary>
            protected override int Load() => EditorPrefs.GetInt(Key, DefaultValue);

            /// <summary>Saves the value of this pref to <see cref="EditorPrefs"/>.</summary>
            protected override void Save() => EditorPrefs.SetInt(Key, Value);

            /************************************************************************************************************************/

            /// <summary>Returns true if <see cref="EditorPrefs"/> currently contains a value for this pref.</summary>
            public override bool IsSaved() => EditorPrefs.HasKey(Key);

            /// <summary>Deletes the value of this pref from <see cref="EditorPrefs"/> and reverts to the default value.</summary>
            public override void DeletePref()
            {
                EditorPrefs.DeleteKey(Key);
                RevertToDefaultValue();
            }

            /************************************************************************************************************************/
            #region Operators
            /************************************************************************************************************************/

            /// <summary>Checks if the value of this pref is greater then the specified `value`.</summary>
            public static bool operator >(EditorInt pref, int value) => pref.Value > value;

            /// <summary>Checks if the value of this pref is less then the specified `value`.</summary>
            public static bool operator <(EditorInt pref, int value) => pref.Value < value;

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="EditorInt"/> pref using the specified string as the key.</summary>
            public static implicit operator EditorInt(string key) => new EditorInt(key);

            /************************************************************************************************************************/
            #endregion
            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// An <see cref="AutoPref{T}"/> which encapsulates a <see cref="string"/> value stored in
        /// <see cref="EditorPrefs"/>.
        /// </summary>
        public sealed class EditorString : String
        {
            /************************************************************************************************************************/

            /// <summary>Constructs an <see cref="EditorString"/> pref with the specified `key` and `defaultValue`.</summary>
            public EditorString(string key, string defaultValue = default, Action<string> onValueChanged = null)
                : base(key, defaultValue, onValueChanged)
            { }

            /// <summary>Loads the value of this pref from <see cref="EditorPrefs"/>.</summary>
            protected override string Load() => EditorPrefs.GetString(Key, DefaultValue);

            /// <summary>Saves the value of this pref to <see cref="EditorPrefs"/>.</summary>
            protected override void Save() => EditorPrefs.SetString(Key, Value);

            /************************************************************************************************************************/

            /// <summary>Returns true if <see cref="EditorPrefs"/> currently contains a value for this pref.</summary>
            public override bool IsSaved() => EditorPrefs.HasKey(Key);

            /// <summary>Deletes the value of this pref from <see cref="EditorPrefs"/> and reverts to the default value.</summary>
            public override void DeletePref()
            {
                EditorPrefs.DeleteKey(Key);
                RevertToDefaultValue();
            }

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="EditorString"/> pref using the specified string as the key.</summary>
            public static implicit operator EditorString(string key) => new EditorString(key);

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// An <see cref="AutoPref{T}"/> which encapsulates a <see cref="UnityEngine.Vector2"/> value stored in
        /// <see cref="EditorPrefs"/>.
        /// </summary>
        public sealed class EditorVector2 : Vector2
        {
            /************************************************************************************************************************/

            /// <summary>Constructs an <see cref="EditorString"/> pref with the specified `key` and `defaultValue`.</summary>
            public EditorVector2(string key,
                UnityEngine.Vector2 defaultValue = default,
                Action<UnityEngine.Vector2> onValueChanged = null)
                : base(key, defaultValue, onValueChanged)
            { }

            /// <summary>Loads the value of this pref from <see cref="EditorPrefs"/>.</summary>
            protected override UnityEngine.Vector2 Load() => new UnityEngine.Vector2(
                EditorPrefs.GetFloat(Key, DefaultValue.x),
                EditorPrefs.GetFloat(KeyY, DefaultValue.y));

            /// <summary>Saves the value of this pref to <see cref="EditorPrefs"/>.</summary>
            protected override void Save()
            {
                EditorPrefs.SetFloat(Key, Value.x);
                EditorPrefs.SetFloat(KeyY, Value.y);
            }

            /************************************************************************************************************************/

            /// <summary>Returns true if <see cref="EditorPrefs"/> currently contains a value for this pref.</summary>
            public override bool IsSaved() =>
                EditorPrefs.HasKey(Key) &&
                EditorPrefs.HasKey(KeyY);

            /// <summary>Deletes the value of this pref from <see cref="EditorPrefs"/> and reverts to the default value.</summary>
            public override void DeletePref()
            {
                EditorPrefs.DeleteKey(Key);
                EditorPrefs.DeleteKey(KeyY);
                RevertToDefaultValue();
            }

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="EditorVector2"/> pref using the specified string as the key.</summary>
            public static implicit operator EditorVector2(string key) => new EditorVector2(key);

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// An <see cref="AutoPref{T}"/> which encapsulates a <see cref="UnityEngine.Vector3"/> value stored in
        /// <see cref="EditorPrefs"/>.
        /// </summary>
        public class EditorVector3 : Vector3
        {
            /************************************************************************************************************************/

            /// <summary>Constructs an <see cref="EditorVector3"/> pref.</summary>
            public EditorVector3(string key,
                UnityEngine.Vector3 defaultValue = default,
                Action<UnityEngine.Vector3> onValueChanged = null)
                : base(key, defaultValue, onValueChanged)
            { }

            /// <summary>Loads the value of this pref from <see cref="EditorPrefs"/>.</summary>
            protected override UnityEngine.Vector3 Load() => new UnityEngine.Vector3(
                EditorPrefs.GetFloat(Key, DefaultValue.x),
                EditorPrefs.GetFloat(KeyY, DefaultValue.y),
                EditorPrefs.GetFloat(KeyZ, DefaultValue.z));

            /// <summary>Saves the value of this pref to <see cref="EditorPrefs"/>.</summary>
            protected override void Save()
            {
                EditorPrefs.SetFloat(Key, Value.x);
                EditorPrefs.SetFloat(KeyY, Value.y);
                EditorPrefs.SetFloat(KeyZ, Value.z);
            }

            /************************************************************************************************************************/

            /// <summary>Returns true if <see cref="EditorPrefs"/> currently contains a value for this pref.</summary>
            public override bool IsSaved() =>
                EditorPrefs.HasKey(Key) &&
                EditorPrefs.HasKey(KeyY) &&
                EditorPrefs.HasKey(KeyZ);

            /// <summary>Deletes the value of this pref from <see cref="EditorPrefs"/> and reverts to the default value.</summary>
            public override void DeletePref()
            {
                EditorPrefs.DeleteKey(Key);
                EditorPrefs.DeleteKey(KeyY);
                EditorPrefs.DeleteKey(KeyZ);
                RevertToDefaultValue();
            }

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="EditorVector3"/> pref using the specified string as the key.</summary>
            public static implicit operator EditorVector3(string key) => new EditorVector3(key);

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// An <see cref="AutoPref{T}"/> which encapsulates a <see cref="Vector4"/> value stored in
        /// <see cref="EditorPrefs"/>.
        /// </summary>
        public class EditorVector4 : Vector4
        {
            /************************************************************************************************************************/

            /// <summary>Constructs an <see cref="EditorVector4"/> pref.</summary>
            public EditorVector4(string key,
                UnityEngine.Vector4 defaultValue = default,
                Action<UnityEngine.Vector4> onValueChanged = null)
                : base(key, defaultValue, onValueChanged)
            { }

            /// <summary>Loads the value of this pref from <see cref="EditorPrefs"/>.</summary>
            protected override UnityEngine.Vector4 Load() => new UnityEngine.Vector4(
                EditorPrefs.GetFloat(Key, DefaultValue.x),
                EditorPrefs.GetFloat(KeyY, DefaultValue.y),
                EditorPrefs.GetFloat(KeyZ, DefaultValue.z),
                EditorPrefs.GetFloat(KeyW, DefaultValue.w));

            /// <summary>Saves the value of this pref to <see cref="EditorPrefs"/>.</summary>
            protected override void Save()
            {
                EditorPrefs.SetFloat(Key, Value.x);
                EditorPrefs.SetFloat(KeyY, Value.y);
                EditorPrefs.SetFloat(KeyZ, Value.z);
                EditorPrefs.SetFloat(KeyW, Value.w);
            }

            /************************************************************************************************************************/

            /// <summary>Returns true if <see cref="EditorPrefs"/> currently contains a value for this pref.</summary>
            public override bool IsSaved() =>
                EditorPrefs.HasKey(Key) &&
                EditorPrefs.HasKey(KeyY) &&
                EditorPrefs.HasKey(KeyZ) &&
                EditorPrefs.HasKey(KeyW);

            /// <summary>Deletes the value of this pref from <see cref="EditorPrefs"/> and reverts to the default value.</summary>
            public override void DeletePref()
            {
                EditorPrefs.DeleteKey(Key);
                EditorPrefs.DeleteKey(KeyY);
                EditorPrefs.DeleteKey(KeyZ);
                EditorPrefs.DeleteKey(KeyW);
                RevertToDefaultValue();
            }

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="EditorVector4"/> pref using the specified string as the key.</summary>
            public static implicit operator EditorVector4(string key) => new EditorVector4(key);

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// An <see cref="AutoPref{T}"/> which encapsulates a <see cref="UnityEngine.Quaternion"/> value stored in
        /// <see cref="EditorPrefs"/>.
        /// </summary>
        public class EditorQuaternion : Quaternion
        {
            /************************************************************************************************************************/

            /// <summary>Constructs a <see cref="Quaternion"/> pref with the specified `key` and `defaultValue`.</summary>
            public EditorQuaternion(string key,
                UnityEngine.Quaternion defaultValue = default,
                Action<UnityEngine.Quaternion> onValueChanged = null)
                : base(key, defaultValue, onValueChanged)
            { }

            /// <summary>Constructs a <see cref="Quaternion"/> pref with the specified `key` and <see cref="UnityEngine.Quaternion.identity"/> as the default value.</summary>
            public EditorQuaternion(string key)
                : base(key, UnityEngine.Quaternion.identity)
            { }

            /// <summary>Loads the value of this pref from <see cref="EditorPrefs"/>.</summary>
            protected override UnityEngine.Quaternion Load() => new UnityEngine.Quaternion(
                EditorPrefs.GetFloat(Key, DefaultValue.x),
                EditorPrefs.GetFloat(KeyY, DefaultValue.y),
                EditorPrefs.GetFloat(KeyZ, DefaultValue.z),
                EditorPrefs.GetFloat(KeyW, DefaultValue.w));

            /// <summary>Saves the value of this pref to <see cref="EditorPrefs"/>.</summary>
            protected override void Save()
            {
                EditorPrefs.SetFloat(Key, Value.x);
                EditorPrefs.SetFloat(KeyY, Value.y);
                EditorPrefs.SetFloat(KeyZ, Value.z);
                EditorPrefs.SetFloat(KeyW, Value.w);
            }

            /************************************************************************************************************************/

            /// <summary>Returns true if <see cref="EditorPrefs"/> currently contains a value for this pref.</summary>
            public override bool IsSaved() =>
                EditorPrefs.HasKey(Key) &&
                EditorPrefs.HasKey(KeyY) &&
                EditorPrefs.HasKey(KeyZ) &&
                EditorPrefs.HasKey(KeyW);

            /// <summary>Deletes the value of this pref from <see cref="EditorPrefs"/> and reverts to the default value.</summary>
            public override void DeletePref()
            {
                EditorPrefs.DeleteKey(Key);
                EditorPrefs.DeleteKey(KeyY);
                EditorPrefs.DeleteKey(KeyZ);
                EditorPrefs.DeleteKey(KeyW);
                RevertToDefaultValue();
            }

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="EditorQuaternion"/> pref using the specified string as the key.</summary>
            public static implicit operator EditorQuaternion(string key) => new EditorQuaternion(key);

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
#endif
        #endregion
        /************************************************************************************************************************/
    }
}

