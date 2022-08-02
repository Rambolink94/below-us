using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public static class Noise
    {
        public static int[,] GenerateNoiseMap(int chunkSize, int seed, bool invertFloor, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
        {
            // Add 1 to the chunkSize to account for later offsets with Squares and control nodes
            chunkSize = chunkSize + 1;
            int[,] noiseMap = new int[chunkSize, chunkSize];

            System.Random pseudoRandom = new System.Random(seed);
            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = pseudoRandom.Next(-100000, 100000) + offset.x;
                float offsetY = pseudoRandom.Next(-100000, 100000) + offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            if (scale <= 0)
            {
                scale = 0.0001f;
            }

            float halfChunkSize = chunkSize / 2f;

            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    float amplitude = 1f;
                    float frequency = 1f;
                    float noiseHeight = 0;

                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x - halfChunkSize + octaveOffsets[i].x) / scale * frequency;
                        float sampleY = (y - halfChunkSize + octaveOffsets[i].y) / scale * frequency;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }
                    if (invertFloor)
                    {
                        if (noiseHeight > 0)
                        {
                            noiseMap[x, y] = 1;
                        }
                        else if (noiseHeight <= 0)
                        {
                            noiseMap[x, y] = 0;
                        }
                    }
                    else
                    {
                        if (noiseHeight > 0)
                        {
                            noiseMap[x, y] = 0;
                        }
                        else if (noiseHeight <= 0)
                        {
                            noiseMap[x, y] = 1;
                        }
                    }
                    
                }
            }

            return noiseMap;
        }
    }
}
