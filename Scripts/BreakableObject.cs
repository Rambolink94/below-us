using UnityEngine;

public class BreakableObject : MonoBehaviour, IDestructable
{
    [SerializeField] GameObject physicalMesh; 
    [SerializeField] AudioClip breakSound;

    private ItemSpawner spawner;
    private AudioSource listener;

    private void Start()
    {
        spawner = GetComponent<ItemSpawner>();
        listener = GetComponent<AudioSource>();

        listener.pitch = Random.Range(0.6f, 1f);
        listener.clip = breakSound;
    }

    public void TriggerDestruction()
    {
        spawner.GeneratePickup();
        listener.Play();
        physicalMesh.SetActive(false);

        Destroy(gameObject, listener.clip.length);
    }
}
