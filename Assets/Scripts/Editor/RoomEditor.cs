using System;
using System.Collections;
using System.Collections.Generic;
using Room;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomLayout))]
public class RoomEditor : Editor
{

    public override void OnInspectorGUI()
    {
        RoomLayout roomLayout = (RoomLayout)target;
        DrawDefaultInspector();
        SerializedObject serializedRoom = new SerializedObject(roomLayout);

        for (int i = 0; i < RoomLayout.RoomHeight; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < RoomLayout.RoomWidth; j++)
            {
                int currentIndex = (i * RoomLayout.RoomWidth + j);
                SerializedProperty tileProperty = serializedRoom.FindProperty("tiles");
                EditorGUILayout.PropertyField(tileProperty.GetArrayElementAtIndex(currentIndex), GUIContent.none);
            }
            EditorGUILayout.EndHorizontal();
        }

        serializedRoom.ApplyModifiedProperties();
    }
}
