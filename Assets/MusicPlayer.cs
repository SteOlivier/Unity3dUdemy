using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;
   // [SerializeField] int repeatTimes = 2;
    [SerializeField] List<AudioClip> audioClips = new List<AudioClip>();
    int currentlyPlaying = 0;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //repeatTimes--;
    }
    void Update()
    {
        //if (audioSource.isPlaying && repeatTimes <= 0)
        //{
        //    audioSource.Stop();
        //}
        if (!audioSource.isPlaying && audioClips.Count > currentlyPlaying)
        {
            audioSource.PlayOneShot(audioClips[currentlyPlaying++]);
        }
    }
}
