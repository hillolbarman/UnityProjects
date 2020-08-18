﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {

        MapGenerator map = target as MapGenerator;
        if (DrawDefaultInspector())
        {
            map.GenerateMap();
        }

        if(UnityEngine.GUILayout.Button("Generate Map"))
        {
            map.GenerateMap();
        }
    }
}
