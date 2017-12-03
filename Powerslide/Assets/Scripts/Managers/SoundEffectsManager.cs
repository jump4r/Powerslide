using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectsManager : MonoBehaviour {

    public static SoundEffectsManager instance = null;

    [SerializeField]
    private AudioClip hitSound;

    private AudioSource source;

	// Use this for initialization
	void Start () {
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(this.gameObject);
        }

        source = GetComponent<AudioSource>();
	}
	
    public void PlayOneShotHitSound(float volume)
    {
        source.clip = hitSound;
        source.volume = volume;
        source.Play();
    }




}
