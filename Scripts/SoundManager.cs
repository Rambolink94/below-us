using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;

    public List<AudioClip> clips = new List<AudioClip>();

    [Header("TESTING")]
    public int maxClipsPlaying = 2;

    private int currentClipsPlaying;

    public List<List<AudioClip>> ambientCaveVolumes = new List<List<AudioClip>>();
    public List<List<AudioClip>> ambientCaveMusicVolumes = new List<List<AudioClip>>();

    // int = index of clip in list
    // float[0] = clip duration
    // float[1] = clip startTime
    private Dictionary<int, float[]> playingClips = new Dictionary<int, float[]>();
    private Dictionary<int, float[]> playingClips1 = new Dictionary<int, float[]>();

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        currentClipsPlaying = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            int randomIndex = Random.Range(0, clips.Count);
            audioSource.clip = clips[randomIndex];
            audioSource.Play();
        }

        //ShuffleVolumes(ambientCaveVolumes, playingClips);
        //ShuffleVolumes(ambientCaveMusicVolumes, playingClips1);
    }

    void ShuffleVolumes(List<List<AudioClip>> volume, Dictionary<int, float[]> playingClips)
    {
        foreach (KeyValuePair<int, float[]> timings in playingClips)
        {
            int clipIndex = timings.Key;
            float[] clipTimes = timings.Value;

            if (Time.time - clipTimes[0] < clipTimes[1])
            {
                int randomIndex = Random.Range(0, volume[clipIndex].Count);
                audioSource.clip = volume[clipIndex][randomIndex];
                audioSource.Play();

                clipTimes[0] = audioSource.clip.length;
                clipTimes[1] = Time.time;
            }
        }
    }
}
