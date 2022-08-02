using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLightController : MonoBehaviour
{
    [SerializeField] LayerMask placeableLayers;
    [SerializeField] GameObject placeableLight;
    [SerializeField] GameObject playerLight;

    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    void ChangeLightColor(GameObject lightObject)
    {
        Color newColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        Light playerLightComponent;
        playerLightComponent = lightObject.GetComponentInChildren<Light>();
        playerLightComponent.color = newColor;

        Renderer rend = lightObject.GetComponent<Renderer>();
        if(rend == null)
        {
            rend = lightObject.GetComponentInChildren<Renderer>();
        }
        rend.material.SetColor("_EmissionColor", newColor);
    }

    public void PlaceTorch()
    {
        if (placeableLight != null) {
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, placeableLayers))
            {
                Quaternion rotation = Quaternion.LookRotation(hit.normal);
                GameObject newLight = Instantiate(placeableLight, hit.point, rotation, hit.collider.gameObject.transform);

                ChangeLightColor(newLight);
            }
        }
    }
}
