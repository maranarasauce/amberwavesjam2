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

    [HideInInspector] public bool mapGenerated = false;


    public void GenerateMap()
    {
        for (int X = 0; X < mapDimensions.x; X++)
        {
            for (int Y = 0; Y < mapDimensions.y; Y++)
            {
                int randomRot = UnityEngine.Random.Range(0, 360);
                randomRot /= 90;
                randomRot *= 90;
                Vector3 euler = new Vector3(0, randomRot, 0);
                var tile = Instantiate(tilePrefab, new Vector3(X * prefabScale.x, spawnHeight, Y * prefabScale.y), Quaternion.Euler(euler), transform);
                tile.transform.localScale = new Vector3(prefabScale.x, 1, prefabScale.y);
            }
        }

        mapGenerated = true;
    }

    public void ClearMap()
    {
        foreach (var child in GetComponentsInChildren<Transform>())
        {
            if (child == transform || child == null)
                continue;

            DestroyImmediate(child.gameObject);
        }

        mapGenerated = false;
    }

}

#if UNITY_EDITOR

[CustomEditor(typeof(TileMap))]
public class TileMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var tileTarget = target as TileMap;

        if (GUILayout.Button("Generate"))
        {
            if (tileTarget.mapGenerated)
                tileTarget.ClearMap();

            tileTarget.GenerateMap();
        }

        if (GUILayout.Button("Clear"))
        {
            tileTarget.ClearMap();
        }


        if (GUILayout.Button("Dithering Iteration"))
        {
            var transformTarget = (target as TileMap).transform;

            for (int i = 0; i < transformTarget.childCount; i++)
            {
                DestroyImmediate(transformTarget.GetChild(i).gameObject);
            }
        }
    }
}

#endif
