using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]

// 
public class Song : MonoBehaviour {

    public string Desc; // Description string of the song

    public static Song playSong; // the Song to be played
    public GameObject metronome;
    public int offset; // Offset of the song to be started in miliseconds.
    public int time; // Current time of the song
    public int BPM;
    public float MPB;

	// Use this for initialization
	void Start () {
        // Calculate the Milliseconds per beat
        // Equation MPB = ( 1 / ( BPM / 60 sec) ) * 1000 ms/s
        // Offset predetermined to trigger when the first beat starts.
        MPB = (1 / (BPM / 60f)) * 1000f; // Set the MPB.
        GetComponent<AudioSource>().Play(); // Play the song of the level
        InvokeRepeating("TickMetronome", (offset / 1000f), MPB / 1000f); // Play metronome tick on every beat
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    private int GetTime()
    {
        return (int)GetComponent<AudioSource>().time; // This cast might be a little buggy but we'll come back to it.
    }

    private void TickMetronome()
    {
        metronome.GetComponent<AudioSource>().Play();       
    }
}
