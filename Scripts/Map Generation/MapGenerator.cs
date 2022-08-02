using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using UnityEditor;

namespace BelowUs
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] bool showFloorMeshDebugInfo = true;
        [SerializeField] bool invertFloor;
        [SerializeField] int squareSize = 1;
        [SerializeField] int wallHeight = 2;

        public enum DrawMode { NoiseMap, Mesh };
        [SerializeField] DrawMode drawMode;

        public float maxViewDst = 100f;
        public int chunkSize = 50;

        public float noiseScale;
        public int seed;
        public Vector2 offset;

        public int octaves;
        [Range(0, 1)]
        public float persistance;
        public float lacunarity;

        public bool autoUpdate;

        MeshData meshData;

        Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
        Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

        public void RequestMapData(Vector2 center, Action<MapData> callback)
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
            lock (mapDataThreadInfoQueue) { mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData)); }
        }

        public void RequestMeshData(MapData mapData, Action<MeshData> callback)
        {
            ThreadStart threadStart = delegate
            {
                MeshDataThread(mapData, callback);
            };

            new Thread(threadStart).Start();
        }

        void MeshDataThread(MapData mapData, Action<MeshData> callback)
        {
            MeshGenerator meshGenerator = new MeshGenerator(squareSize, wallHeight);
            MeshData meshData = meshGenerator.GenerateMapMeshes(mapData.noiseMap);
            lock (meshDataThreadInfoQueue) { meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData)); }
        }

        public void RequestUpdatedMeshData(MeshData chunkMeshData, Vector3 hitPos, float radius, bool calledFromNeighbour, Action<MeshData> callback)
        {
            ThreadStart threadStart = delegate
            {
                UpdateMeshDataThread(chunkMeshData, hitPos, radius, calledFromNeighbour, callback);
            };

            new Thread(threadStart).Start();
        }

        void UpdateMeshDataThread(MeshData chunkMeshData, Vector3 hitPos, float radius, bool calledFromNeighbour, Action<MeshData> callback)
        {
            MeshGenerator meshGenerator = new MeshGenerator(chunkMeshData);
            MeshData newMeshData = meshGenerator.UpdateMeshData(hitPos, radius, calledFromNeighbour);
            lock (meshDataThreadInfoQueue) { meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, newMeshData)); }
        }

        private void Update()
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

        public MapData GenerateMapData(Vector2 center)
        {
            int[,] noiseMap = Noise.GenerateNoiseMap(chunkSize, seed, invertFloor, noiseScale, octaves, persistance, lacunarity, center + offset);

            return new MapData(noiseMap);
        }

        public void DrawMapInEditor()
        {
            MapData mapData = GenerateMapData(Vector2.zero);

            MapDisplay display = GetComponent<MapDisplay>();
            switch (drawMode)
            {
                case DrawMode.NoiseMap:
                    display.DrawNoiseMap(mapData.noiseMap);
                    break;
                case DrawMode.Mesh:
                    MeshGenerator meshGenerator = new MeshGenerator(squareSize, wallHeight);
                    meshData = meshGenerator.GenerateMapMeshes(mapData.noiseMap);
                    display.DrawMesh(meshData);
                    break;
                default:
                    break;
            }
        }

        #if UNITY_EDITOR
        // DEBUGGING
        public void OnDrawGizmos()
        {
            if (meshData != null && meshData.squareGrid != null && showFloorMeshDebugInfo)
            {
                for (int x = 0; x < meshData.squareGrid.squares.GetLength(0); x++)
                {
                    for (int y = 0; y < meshData.squareGrid.squares.GetLength(1); y++)
                    {
                        if(x + 1 == meshData.squareGrid.squares.GetLength(0))
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawSphere(meshData.squareGrid.squares[x, y].centerRight.position, 0.2f);
                        }

                        if (y + 1 == meshData.squareGrid.squares.GetLength(0))
                        {
                            Gizmos.color = Color.blue;
                            Gizmos.DrawSphere(meshData.squareGrid.squares[x, y].centerTop.position, 0.2f);
                        }

                        Handles.Label(meshData.squareGrid.squares[x, y].centerLeft.position + new Vector3(0.5f, 0, 0), meshData.squareGrid.squares[x, y].configuration.ToString());

                        Gizmos.color = meshData.squareGrid.squares[x, y].topLeft.active ? Color.black : Color.white;
                        Gizmos.DrawCube(meshData.squareGrid.squares[x, y].topLeft.position, Vector3.one * 0.4f);

                        Gizmos.color = meshData.squareGrid.squares[x, y].topRight.active ? Color.black : Color.white;
                        Gizmos.DrawCube(meshData.squareGrid.squares[x, y].topRight.position, Vector3.one * 0.4f);

                        Gizmos.color = meshData.squareGrid.squares[x, y].bottomLeft.active ? Color.black : Color.white;
                        Gizmos.DrawCube(meshData.squareGrid.squares[x, y].bottomLeft.position, Vector3.one * 0.4f);

                        Gizmos.color = meshData.squareGrid.squares[x, y].bottomRight.active ? Color.black : Color.white;
                        Gizmos.DrawCube(meshData.squareGrid.squares[x, y].bottomRight.position, Vector3.one * 0.4f);

                        Gizmos.color = Color.gray;
                        Gizmos.DrawCube(meshData.squareGrid.squares[x, y].centerLeft.position, Vector3.one * 0.15f);
                        Gizmos.DrawCube(meshData.squareGrid.squares[x, y].centerTop.position, Vector3.one * 0.15f);
                        Gizmos.DrawCube(meshData.squareGrid.squares[x, y].centerRight.position, Vector3.one * 0.15f);
                        Gizmos.DrawCube(meshData.squareGrid.squares[x, y].centerBottom.position, Vector3.one * 0.15f);
                    }
                }
            }
        }
        #endif

        private void OnValidate()
        {
            if (chunkSize < 1) chunkSize = 1;
            if (octaves <= 0) octaves = 1;
        }

        struct MapThreadInfo<T>
        {
            public readonly Action<T> callback;
            public readonly T parameter;

            public MapThreadInfo(Action<T> callback, T parameter)
            {
                this.callback = callback;
                this.parameter = parameter;
            }
        }
    }
    public struct MapData
    {
        public readonly int[,] noiseMap;

        public MapData(int[,] noiseMap)
        {
            this.noiseMap = noiseMap;
        }
    }
}

