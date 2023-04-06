using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[Serializable]
public class PrefabPositioning
{
    public GameObject Prefab;
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale = Vector3.one;
    public bool DoUpdate;

    public void Apply(Transform t, Vector3 rotateOffset)
    {
        t.localPosition = Position;
        t.localRotation = Quaternion.Euler(
            Rotation.x + rotateOffset.x,
            Rotation.y + rotateOffset.y,
            Rotation.z + rotateOffset.z);
        t.localScale = Scale;
    }
    public void Apply(Transform t) => Apply(t, Vector3.zero);
}

[CustomPropertyDrawer(typeof(PrefabPositioning))]
public class PrefabPositioningDrawer : PropertyDrawer
{
    private static readonly float LINE_HEIGHT = EditorGUIUtility.singleLineHeight;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
        {
            return LINE_HEIGHT * 6;
        }
        return LINE_HEIGHT;
    }

    private Rect GetRect(Rect parent, int idx)
    {
        return new Rect(parent.x, parent.y + idx * LINE_HEIGHT, parent.width, LINE_HEIGHT);
    }

    private void Field(Rect rect, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(rect, label, property);
        EditorGUI.PropertyField(rect, property, label);
        EditorGUI.EndProperty();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(GetRect(position, 0), property.isExpanded, new GUIContent("Prefab"));
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            Field(GetRect(position, 1), property.FindPropertyRelative("Prefab"), GUIContent.none);
            EditorGUI.indentLevel++;
            Field(GetRect(position, 2), property.FindPropertyRelative("Position"), new GUIContent("Position"));
            Field(GetRect(position, 3), property.FindPropertyRelative("Rotation"), new GUIContent("Rotation"));
            Field(GetRect(position, 4), property.FindPropertyRelative("Scale"), new GUIContent("Scale"));
            EditorGUI.indentLevel--;
            Field(GetRect(position, 5), property.FindPropertyRelative("DoUpdate"), new GUIContent("Update?", "When false, changing these values in play mode will do nothing."));
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndFoldoutHeaderGroup();
    }
}
