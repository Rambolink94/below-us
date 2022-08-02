using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class ChunkManager : MonoBehaviour
    {
        public WorldGenerator.MapChunk mapChunkData;

        public void InitializeChunk(Vector2 coord, int size, int chunkCount, Transform parent, Material material, MapGenerator mapGen)
        {
            mapChunkData = new WorldGenerator.MapChunk(coord, size, chunkCount, parent, material, mapGen);
        }
    }
}
