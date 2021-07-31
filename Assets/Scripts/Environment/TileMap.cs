using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class TileMap : MonoBehaviour
{
    public GameObject tilePrefab;
    public float spawnHeight     = -0.5f;          // spawns the map at this Z-pos.
    public Vector2 prefabScale   = Vector2.one;    // Assumes prefab is a square, scaled at 1,1,1.
    public Vector2 mapDimensions = Vector2.one * 5;


    public void GenerateMap()
    {
        for (int X = 0; X < mapDimensions.x; X++)
        {
            for (int Y = 0; Y < mapDimensions.y; Y++)
            {
                var tile = Instantiate(tilePrefab, new Vector3(X * prefabScale.x, spawnHeight, Y * prefabScale.y), Quaternion.identity, transform);
                tile.transform.localScale = new Vector3(prefabScale.x, 1, prefabScale.y);
            }
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(TileMap))]
public class TileMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
        {
            (target as TileMap).GenerateMap();
        }

        if (GUILayout.Button("Clear"))
        {
            var transformTarget = (target as TileMap).transform;

            foreach(var child in transformTarget.GetComponentsInChildren<Transform>())
            {
                if (child == transformTarget)
                    continue;

                DestroyImmediate(child.gameObject);
            }
        }


        //if (GUILayout.Button("Dithering i guess lol"))
        //{
        //    var transformTarget = (target as TileMap).transform;

        //    for (int i = 0; i < transformTarget.childCount; i++)
        //    {
        //        DestroyImmediate(transformTarget.GetChild(i).gameObject);
        //    }
        //}
    }
}

#endif
