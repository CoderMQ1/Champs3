// Inspector Gadgets // https://kybernetik.com.au/inspector-gadgets // Copyright 2021 Kybernetik //

using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InspectorGadgets.Attributes
{
    /// <summary>[Pro-Only]
    /// <see cref="Editor.Editor{T}"/> uses this attribute to add a button at the bottom of the default inspector to
    /// run the marked method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ButtonAttribute : BaseInspectableAttribute
    {
        /************************************************************************************************************************/

        /// <summary>
        /// If true, clicking the button will automatically call <see cref="UnityEditor.EditorUtility.SetDirty"/> after
        /// invoking the method.
        /// </summary>
        public bool SetDirty { get; set; }

        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        private MethodInfo _Method;

        /// <summary>Initialize this button with a method.</summary>
        protected override string Initialize()
        {
            _Method = Member as MethodInfo;
            if (_Method == null)
                return "it is not a method";

            if (Label == null)
                Label = IGUtils.ConvertCamelCaseToFriendly(_Method.Name);

            if (Tooltip == null)
                Tooltip = "Calls " + _Method.GetNameCS();

            return null;
        }

        /************************************************************************************************************************/

        /// <summary>Draw this button using <see cref="GUILayout"/>.</summary>
        public override void OnGUI(Object[] targets)
        {
            if (GUILayout.Button(Label, UnityEditor.EditorStyles.miniButton))
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (SetDirty)
                        UnityEditor.Undo.RecordObjects(targets, "Inspector");

                    if (_Method.IsStatic)// Static Method.
                    {
                        var result = _Method.Invoke(null, null);

                        if (_Method.ReturnType != typeof(void))
                            Debug.Log($"{Label}: {result}");
                    }
                    else// Instance Method.
                    {
                        foreach (var target in targets)
                        {
                            var result = _Method.Invoke(target, null);

                            if (_Method.ReturnType != typeof(void))
                                Debug.Log($"{Label}: {result}", target);
                        }
                    }
                };
            }
        }

        /************************************************************************************************************************/
#endif
    }
}

