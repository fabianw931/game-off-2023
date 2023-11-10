using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor
{

    public override void OnInspectorGUI()
    {
        Room room = (Room)target;
        DrawDefaultInspector();
        SerializedObject serializedRoom = new SerializedObject(room);

        for (int i = 0; i < Room.ROOM_HEIGHT; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < Room.ROOM_WIDTH; j++)
            {
                int currentIndex = (i * Room.ROOM_WIDTH + j);
                SerializedProperty tileProperty = serializedRoom.FindProperty("tiles");
                EditorGUILayout.PropertyField(tileProperty.GetArrayElementAtIndex(currentIndex), GUIContent.none);
            }
            EditorGUILayout.EndHorizontal();
        }

        serializedRoom.ApplyModifiedProperties();
    }
}
