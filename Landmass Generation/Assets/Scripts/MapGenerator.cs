﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class MapGenerator : MonoBehaviour
{
	public enum DrawMode { NoiseMap, Mesh, FalloffMap };
	public DrawMode drawMode;
	[Range(0, 6)]
	public int editorPreviewLOD;
	public bool autoUpdate;
	public TerrainData terrainData;
	public NoiseData noiseData;
	public TextureData textureData;
	public Material terrainMaterial;
	float[,] falloffMap;
	Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
	Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    private void Awake()
    {

		textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);
	}

    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
			DrawMapInEditor();
        }
    }

	void OnTextureValuesUpdated()
	{
		textureData.ApplyToMaterial(terrainMaterial);
	}

	public int mapChunkSize
    {
        get
        {
			if(terrainData.useFlatShading) { return 95; } else { return 239; }
        }
    }
	void Update()
	{
        if (mapDataThreadInfoQueue.Count > 0)
        {
			for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
				MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
				threadInfo.callback(threadInfo.parameter);
            }
        }
		
		if (meshDataThreadInfoQueue.Count > 0)
        {
			for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
				MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
				threadInfo.callback(threadInfo.parameter);
            }
        }
	}

	public void DrawMapInEditor()
	{

		textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);
		MapData mapData = GenerateMapData(Vector2.zero);
		MapDisplay display = FindObjectOfType<MapDisplay>();
		if (drawMode == DrawMode.NoiseMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
		}
		else if (drawMode == DrawMode.Mesh)
		{
			display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, editorPreviewLOD, terrainData.useFlatShading));
		}
		else if (drawMode == DrawMode.FalloffMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(mapChunkSize)));
		}
	}

	public void RequestMapData(Vector2 center,Action<MapData> callback)
	{
		ThreadStart threadStart = delegate
		{
			MapDataThread(center, callback);
		};
		new Thread(threadStart).Start();
	}

	void MapDataThread(Vector2 center, Action<MapData> callback)
	{
		MapData mapData = GenerateMapData(center);
		lock (mapDataThreadInfoQueue)
		{
			mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
		}
	}


	public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
	{
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, lod, callback);
        };
        new Thread(threadStart).Start();
    }

	void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
	{
		MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, lod, terrainData.useFlatShading);
		lock (meshDataThreadInfoQueue)
		{
			meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
		}
	}



	MapData GenerateMapData(Vector2 center)
	{
		float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, center + noiseData.offset, noiseData.normalizedMode);
		if (terrainData.useFalloffMap)
		{
            if (falloffMap == null)
            {
				falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize+2);
			}
			for (int y = 0; y < mapChunkSize+2; y++)
			{
				for (int x = 0; x < mapChunkSize+2; x++)
				{
					if (terrainData.useFalloffMap)
					{
						noiseMap[x, y] = Mathf.Clamp(noiseMap[x, y] - falloffMap[x, y], 0, 1);
					}
				}
			}
		}
		return new MapData(noiseMap);
	}

	void OnValidate()
	{
        if (terrainData != null)
        {
			terrainData.OnValuesUpdated -= OnValuesUpdated;
			terrainData.OnValuesUpdated += OnValuesUpdated;
        }
		if (noiseData != null)
		{
			noiseData.OnValuesUpdated -= OnValuesUpdated;
			noiseData.OnValuesUpdated += OnValuesUpdated;
		}
		if (textureData != null)
		{
			textureData.OnValuesUpdated -= OnTextureValuesUpdated;
			textureData.OnValuesUpdated += OnTextureValuesUpdated;
		}
	}

	struct MapThreadInfo<T>
	{
		public readonly Action<T> callback;
		public readonly T parameter;

		public MapThreadInfo(Action<T> callback, T parameter) {
			this.callback = callback;
			this.parameter = parameter;
		}
	}

}


public struct MapData
{
	public readonly float[,] heightMap;

	public MapData(float[,] heightMap)
    {
		this.heightMap = heightMap;
    }
}
