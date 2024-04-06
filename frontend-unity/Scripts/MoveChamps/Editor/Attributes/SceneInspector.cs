using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SceneInspector : PropertyAttribute
{

}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneInspector))]
public class SceneStringInspectorDrawer : PropertyDrawer
{
    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {	
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        SceneAsset setScene = null;
        if (!string.IsNullOrEmpty( property.stringValue ))
        {
            setScene = AssetDatabase.LoadAssetAtPath<SceneAsset>( property.stringValue );
        }

        SceneAsset newTargetScene = (SceneAsset)EditorGUI.ObjectField(position, setScene, typeof(SceneAsset), true);
        if (newTargetScene != null)
        {
            property.stringValue = AssetDatabase.GetAssetPath( newTargetScene );
        }
        else
        {
            property.stringValue = "";
        }

        EditorGUI.EndProperty();
    }
}
#endif