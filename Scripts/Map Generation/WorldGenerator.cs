using BelowUs.Utility;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

namespace BelowUs
{
    public class WorldGenerator : MonoBehaviour
    {
        public PlayerSpawner spawner;
        public Material mapMaterial;
        
        [Tooltip("The size of the map. Each unit is a chunk. ex - 10 == 10 x 10.")]
        public int mapSize = 10;
        static int chunksVisibleInViewDst;
        int chunkCount = 1;

        public static int chunkSize;
        public static Vector2 viewerPosition;

        static Dictionary<Vector2, MapChunk> mapChunkDictionary = new Dictionary<Vector2, MapChunk>();
        static List<MapChunk> mapChunksVisibleLastUpdate = new List<MapChunk>();

        private Transform viewer;
        private MapGenerator mapGen;

        private void Start()
        {
            mapGen = gameObject.GetComponent<MapGenerator>();
            chunkSize = mapGen.chunkSize;
            chunksVisibleInViewDst = Mathf.RoundToInt(mapGen.maxViewDst / chunkSize);

            GenerateMapChunks();
        }

        private void Update()
        {
            if (viewer != null)
            {
                viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

                UpdateVisibleChunks();
            }
        }

        public void UpdateCurrentMapChunk(Vector3 hitPos, float radius = -1f)
        {
            int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
            int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

            Vector2 currentChunkCoord = new Vector2(currentChunkCoordX, currentChunkCoordY);

            if (mapChunkDictionary.ContainsKey(currentChunkCoord))
            {
                MapChunk currentMapChunk = mapChunkDictionary[currentChunkCoord];
                //Debug.Log("CHUNK POS: " + currentMapChunk.GetChunkPosition());

                // Adjust / Round hitPos to line up with square grid units and add offset of chunk coord.
                hitPos = HelperFunctions.RoundVector3NearestHalfOrWhole(hitPos);
                hitPos = OffsetHitPosition(currentMapChunk, hitPos);

                //Debug.Log("Hit Pos: " + hitPos);
                currentMapChunk.UpdateMeshData(hitPos, radius, false);
            }
        }

        public static MapChunk[,] GetNeighbouringChunks()
        {
            MapChunk[,] neighbouringChunks = new MapChunk[3, 3];

            int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
            int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

            for (int offsetX = -1; offsetX <= 1; offsetX++)
            {
                for (int offsetY = -1; offsetY <= 1; offsetY++)
                {
                    // If not current chunk coord
                    if (offsetX != 0 || offsetY != 0)
                    {
                        Vector2 currentNeighbourChunk = new Vector2(currentChunkCoordX + offsetX, currentChunkCoordY + offsetY);
                        if (mapChunkDictionary.ContainsKey(currentNeighbourChunk))
                        {
                            neighbouringChunks[offsetX + 1, offsetY + 1] = mapChunkDictionary[currentNeighbourChunk];
                            Debug.Log("CHUNK: " + mapChunkDictionary[currentNeighbourChunk] + " [" + (offsetX + 1) + ", " + (offsetY + 1) + "] Pos: " + currentNeighbourChunk);
                        }
                    }
                }
            }

            return neighbouringChunks;
        }

        // Probably should not be here, but I can worry about that later.
        // Could maybe be used to speed up chunk modification. Saved for later.
        public static List<Square> GetMissingNeighbors(List<Square> neighbors, ControlNode controlNode)
        {
            /*
            MapChunk[,] allNeighboringChunks = GetNeighbouringChunks();
            MapChunk[,] applicableChunks = new MapChunk[3, 3];

            // Determine what neighbors are missing
            switch (controlNode.controlNodeType)
            {
                case ControlNodeType.CentralNode:
                    // No neighbors are missing. Just get out of here.
                    return neighbors;
                case ControlNodeType.LeftEdge:
                    // Get left neighbor chunk
                    break;
                case ControlNodeType.TopEdge:
                    break;
                case ControlNodeType.RightEdge:
                    break;
                case ControlNodeType.BottomEdge:
                    break;
                case ControlNodeType.TopLeftCorner:
                    break;
                case ControlNodeType.TopRightCorner:
                    break;
                case ControlNodeType.BottomLeftCorner:
                    break;
                case ControlNodeType.BottomRightCorner:
                    break;
                default:
                    break;
            }

            foreach (Square neighbor in neighbors)
            {
                if ()
            }


            int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
            int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

            int halfChunkSize = chunkSize / 2;

            for (int hitPosOffsetX = -halfChunkSize; hitPosOffsetX <= halfChunkSize; hitPosOffsetX += halfChunkSize)
            {
                for (int hitPosOffsetY = -halfChunkSize; hitPosOffsetY <= halfChunkSize; hitPosOffsetY += halfChunkSize)
                {
                    int neighbourChunkCoordX = Mathf.RoundToInt((hitPosV2.x + hitPosOffsetX) / chunkSize);
                    int neighbourChunkCoordY = Mathf.RoundToInt((hitPosV2.y + hitPosOffsetY) / chunkSize);

                    if (currentChunkCoordX == neighbourChunkCoordX && currentChunkCoordY == neighbourChunkCoordY) continue;

                    Vector2 currentNeighbourChunk = new Vector2(currentChunkCoordX + neighbourChunkCoordX, currentChunkCoordY + neighbourChunkCoordY);
                    if (mapChunkDictionary.ContainsKey(currentNeighbourChunk))
                    {
                        neighbouringChunks[neighbourChunkCoordX + 1, neighbourChunkCoordY + 1] = mapChunkDictionary[currentNeighbourChunk];
                        Debug.Log("CHUNK: " + mapChunkDictionary[currentNeighbourChunk] + " [" + (neighbourChunkCoordX + 1) + ", " + (neighbourChunkCoordY + 1) + "] Pos: " + currentNeighbourChunk);
                    }
                }
            }

            
            */
            return neighbors;
        }

        public static void TriggerNeighborUpdate(Vector3 hitPos)
        {
            MapChunk[,] neighboringChunks = GetNeighbouringChunks();
            foreach (MapChunk neighbor in neighboringChunks)
            {
                if (neighbor != null)
                {
                    hitPos = OffsetHitPosition(neighbor, hitPos);
                    neighbor.UpdateMeshData(hitPos, ThirdPersonController.mineEffectRange, true);
                }
            }
        }

        public static Vector3 OffsetHitPosition(MapChunk mapChunk, Vector3 hitPos)
        {
            //Debug.Log("----- CHUNK: " + mapChunk + " POS: " + hitPos);
            hitPos.x -= mapChunk.GetChunkPosition().x;
            hitPos.z -= mapChunk.GetChunkPosition().y;

            return hitPos;
        }

        void GenerateMapChunks()
        {
            int halfMapSize = mapSize / 2;

            for (int xOffset = -halfMapSize; xOffset < halfMapSize; xOffset++)
            {
                for (int yOffset = -halfMapSize; yOffset < halfMapSize; yOffset++)
                {
                    Vector2 currentChunkCoord = new Vector2(xOffset, yOffset);

                    mapChunkDictionary.Add(currentChunkCoord, new MapChunk(currentChunkCoord, chunkSize, chunkCount, transform, mapMaterial, mapGen));
                    chunkCount++;
                }
            }

            viewer = spawner.SpawnPlayer().transform;
        }

        void UpdateVisibleChunks()
        {
            for (int i = 0; i < mapChunksVisibleLastUpdate.Count; i++)
            {
                mapChunksVisibleLastUpdate[i].SetVisible(false);
            }
            mapChunksVisibleLastUpdate.Clear();

            int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
            int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
                {
                    Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                    if (mapChunkDictionary.ContainsKey(viewedChunkCoord))
                    {
                        mapChunkDictionary[viewedChunkCoord].UpdateMapChunkVisibility();
                        if(mapChunkDictionary[viewedChunkCoord].IsVisible())
                        {
                            mapChunksVisibleLastUpdate.Add(mapChunkDictionary[viewedChunkCoord]);
                        }
                    }
                }
            }
        }
        public class MapChunk
        {
            GameObject meshObject;
            Vector2 position;
            Bounds bounds;

            MeshRenderer meshRenderer;
            MeshFilter meshFilter;
            MeshCollider meshCollider;
            NavMeshSurface navMeshSurface;
            MapGenerator mapGen;

            MeshData chunkMeshData;

            public MapChunk(Vector2 coord, int size, int chunkCount, Transform parent, Material material, MapGenerator mapGen)
            {
                position = coord * size;
                bounds = new Bounds(position, Vector2.one * size);
                Vector3 positionV3 = new Vector3(position.x, 0, position.y);

                meshObject = new GameObject("Map Chunk " + chunkCount + ":\t" + coord);
                meshObject.layer = 10;

                meshRenderer = meshObject.AddComponent<MeshRenderer>();
                meshFilter = meshObject.AddComponent<MeshFilter>();
                meshCollider = meshObject.AddComponent<MeshCollider>();
                navMeshSurface = meshObject.AddComponent<NavMeshSurface>();
                meshRenderer.material = material;

                this.mapGen = mapGen;

                meshObject.transform.position = positionV3;
                meshObject.transform.parent = parent;
                SetVisible(false);

                mapGen.RequestMapData(position, OnMapDataReceived);
            }

            public Vector2 GetChunkPosition()
            {
                return position;
            }

            void OnMapDataReceived(MapData mapData)
            {
                mapGen.RequestMeshData(mapData, OnMeshDataReceived);
            }

            void OnMeshDataReceived(MeshData meshData)
            {
                //Debug.Log("--- NEW MESH ---");
                //MeshGenerator.PrintGridInfo(meshData.squareGrid.squares);

                chunkMeshData = meshData;
                meshFilter.mesh = meshData.CreateMesh();
                meshCollider.sharedMesh = meshFilter.mesh;

                // This is bad code, but i'll keep it for now.
                navMeshSurface.layerMask = LayerMask.GetMask("Ground");
                navMeshSurface.BuildNavMesh();
            }

            public void UpdateMeshData(Vector3 hitPos, float radius, bool calledFromNeighbour)
            {
                chunkMeshData.ClearBasicMeshData();
                mapGen.RequestUpdatedMeshData(chunkMeshData, hitPos, radius, calledFromNeighbour, OnMeshDataReceived);
            }

            public void UpdateMapChunkVisibility()
            {
                float viewerDstFromNeareastEdge = bounds.SqrDistance(viewerPosition);
                bool visible = viewerDstFromNeareastEdge <= mapGen.maxViewDst;
                SetVisible(visible);
            }

            public void SetVisible(bool visible)
            {
                meshObject.SetActive(visible);
            }

            public bool IsVisible()
            {
                return meshObject.activeSelf;
            }
        }
    }
}
