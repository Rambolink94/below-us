using BelowUs;
using Cinemachine;
using StarterAssets;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] GameObject playerObject;

    public void Awake()
    {
        playerObject.SetActive(false);
    }

    public GameObject SpawnPlayer()
    {
        playerObject.SetActive(true);
        gameObject.transform.GetChild(0).gameObject.SetActive(false);

        return playerObject;
    }
}
