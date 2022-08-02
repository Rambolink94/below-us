using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOutline : MonoBehaviour
{
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private float outlineScaleFactor;
    [SerializeField] private Color outlineColor;

    private Renderer outlineRenderer;

    private void Start()
    {
        outlineRenderer = GenerateOutline(outlineMaterial, outlineScaleFactor, outlineColor);
    }

    Renderer GenerateOutline(Material outlineMat, float scaleFactor, Color color)
    {
        GameObject outlineObject = Instantiate(gameObject, transform.position, transform.rotation, transform);
        Renderer renderer = outlineObject.GetComponent<Renderer>();

        renderer.material = outlineMat;
        renderer.material.SetColor("_OutlineColor", color);
        renderer.material.SetFloat("_ScaleFactor", scaleFactor);
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        outlineObject.GetComponent<CreateOutline>().enabled = false;
        outlineObject.GetComponent<Collider>().enabled = false;

        return renderer;
    }
}
