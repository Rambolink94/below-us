using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class MapDisplay : MonoBehaviour
    {
        public bool disableDirectionalLight = true;

        public Renderer textureRenderer;
        public GameObject directionalLight;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;

        public void Start()
        {
            meshFilter.gameObject.SetActive(false);
            if (disableDirectionalLight)
            {
                directionalLight.SetActive(false);
            }
        }

        public void DrawNoiseMap(int[,] noiseMap)
        {
            int width = noiseMap.GetLength(0);
            int height = noiseMap.GetLength(1);

            Texture2D texture = new Texture2D(width, height);

            Color[] colorMap = new Color[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
                }
            }
            texture.SetPixels(colorMap);
            texture.Apply();

            textureRenderer.sharedMaterial.mainTexture = texture;
            textureRenderer.transform.localScale = new Vector3(width, 1, height);
        }

        public void DrawMesh(MeshData meshData)
        {
            meshFilter.sharedMesh = meshData.CreateMesh();
            MeshCollider collider = meshFilter.gameObject.GetComponent<MeshCollider>();
            if (!collider)
            {
                collider = meshFilter.gameObject.AddComponent<MeshCollider>();
            }
            collider.sharedMesh = meshFilter.sharedMesh;
        }
    }
}
