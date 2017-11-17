using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EditorConductor : MonoBehaviour {

    public static EditorConductor instance = null;

    public float bpm = 0f;
    public float offset = 0f; // was 2.655, changed for Dango Daikazoku
    public float songPosition = -1f;
    public float nextBeatTime = 0f; // time of the next beat.
    public float spawnTime = 0f; // Time the note should spawn
    public float SpawnTimeOffset;

    public AudioSource source;

    public float spb;

    void Start()
    {
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

    public void Construct(EditorBeatmap beatmap)
    {
        if (beatmap.song != null)
        {
            source.clip = beatmap.song;
            // source.Play();
        }

        spb = 60f / beatmap.BPM;
        offset = beatmap.OsuOffset / 1000f;
        SpawnTimeOffset = spb * NoteHelper.Whole;


        Debug.Log("Initializing The Seperators");
        MeasureSeperatorManager.instance.InitializeSeperators();
    }

    private void Update()
    {
        if (source.isPlaying)
        {
            songPosition = source.timeSamples / (float) source.clip.frequency;
        }
    }

    public void Play()
    {
        if (source.clip != null)
        {
            source.Play();
        }
    }
}
