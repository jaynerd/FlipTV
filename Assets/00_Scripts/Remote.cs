using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Remote : MonoBehaviour {
    public static Remote Instance;

    AudioSource aSource;
    public AudioClip buttonSFX;
    private const float MIN_VOLUME = 0.5f;
    private const float MAX_VOLUME = 1f;

    void Awake() {
        Instance = this;
        aSource = GetComponent<AudioSource>();
    }
    public void PlayButtonSFX()
    {
        float volume = Random.Range(MIN_VOLUME, MAX_VOLUME);
        aSource.PlayOneShot(buttonSFX, volume);
    }
}
