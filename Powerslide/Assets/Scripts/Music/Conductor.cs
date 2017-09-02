using UnityEngine;
using System.Collections;

// The code example shows how to implement a metronome that procedurally generates the click sounds via the OnAudioFilterRead callback.
// While the game is paused or the suspended, this time will not be updated and sounds playing will be paused. Therefore developers of music scheduling routines do not have to do any rescheduling after the app is unpaused 

// VARAIBLES
// BPM = Beats Per Minute
// Offset - The amount of time before the first beat hits.
[RequireComponent(typeof(AudioSource))]
public class Conductor : MonoBehaviour
{

    /**** For testing
     * Dango Daikazoku - bpm: 100, offset: 2.390
     * Celestial Stinger - bpm: 259, offset: 572
   :*/
    // public static Conductor conductor;
    public static float bpm = 100f;
    public static float offset = 2.390f; // was 2.655, changed for Dango Daikazoku
    public static float songPosition = 0f;
    public static float nextBeatTime = 0f; // time of the next beat.
    public static float spawnTime = 0f; // Time the note should spawn

    [SerializeField]
    private AudioSource source; // Source of the audio clip

    public static float spb;

    // Testing purposes
    private bool flip = true;

    private Beatmap beatmap;
    private int currentNoteIndex = 0;

    public float gain = 0.5F;
    public int signatureHi = 4;
    public int signatureLo = 4;
    private double nextTick = 0.0F;
    private float amp = 0.0F;
    private float phase = 0.0F;
    private double sampleRate = 0.0F;
    private int accent;
    private bool running = false;
    private float runTime = 0f;

    void Start()
    {
        spb = 60f / (float)bpm; // 60 seconds / beats per minute
        accent = signatureHi;
        double startTick = AudioSettings.dspTime;
        sampleRate = AudioSettings.outputSampleRate;
        source = GetComponent<AudioSource>();
        nextTick = startTick * sampleRate;
        running = true;

        nextBeatTime = offset;
        spawnTime = offset - spb * 8f;
    }

    public void LoadBeatmap(Beatmap beatmap)
    {
        if (source == null) { source = GetComponent<AudioSource>(); }
        this.beatmap = beatmap;
        source.clip = beatmap.song;
        spb = 60f / beatmap.BPM; // Seconds per beat
        offset = beatmap.Offset / 1000f; // Offset is in milliseconds for easier readibility
        nextBeatTime = GetNextBeatTime(beatmap.Notes[currentNoteIndex]);
        Debug.Log("Total Number of notes: " + beatmap.Notes.Count);
        Play();
    }

    private float GetNextBeatTime(string hitNote)
    {
        float offset = float.Parse(hitNote.Split(',')[0]) / 1000f;
        Debug.Log("Next Beat Time: " + offset);
        return offset;
    }

    void Update()
    {
        runTime += Time.deltaTime;
        songPosition = source.timeSamples / (float)source.clip.frequency;
        // Debug.Log("Song Position: " + songPosition + ", SpawnTime: " + spawnTime);

        /*
        if (songPosition < offset)
            return; // Don't spawn yet. */

        // Update the next beat time.
        while (songPosition > spawnTime && currentNoteIndex < beatmap.Notes.Count)
        {
            Debug.Log("Spawn Note");
            // NoteSpawner.SpawnNote(nextBeatTime, 0);
            // NoteSpawner.SpawnNote(1);
            // NoteSpawner.SpawnDrag(nextBeatTime);
            // NoteSpawner.SpawnHold(beatmap.Notes[currentNoteIndex].Split(','));
            //NoteSpawner.SpawnHold(2, "false", 2);
            // NoteSpawner.SpawnFlick(nextBeatTime, 1,0, "l");
            //NoteSpawner.SpawnFlick(3,2, "l");
            NoteSpawner.SpawnHitObject(beatmap.Notes[currentNoteIndex]);

            // Update Timings: 
            currentNoteIndex++;
            if (currentNoteIndex < beatmap.Notes.Count)
            {
                nextBeatTime = GetNextBeatTime(beatmap.Notes[currentNoteIndex]);
                spawnTime = nextBeatTime - spb * 8f;
            }
            // Debug.Log("Current Song Position: " + songPosition);
        }

        Debug.Log("Current Song Position: " + songPosition + ", Spawn Time: " + spawnTime);
    }

    public void Play()
    {
        if (source.clip != null) {
            source.Play();
        }
    }

    public void GetNextBeat()
    {
        
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        /*if (!running)
            return;

        if (runTime < offset)
            return;

        double samplesPerTick = sampleRate * 60.0F / bpm * 4.0F / signatureLo;
        double sample = AudioSettings.dspTime * sampleRate;
        int dataLen = data.Length / channels;
        int n = 0;
        while (n < dataLen)
        {
            float x = gain * amp * Mathf.Sin(phase);
            int i = 0;
            while (i < channels)
            {
                data[n * channels + i] += x;
                i++;
            }
            while (sample + n >= nextTick)
            {
                nextTick += samplesPerTick;
                amp = 1.0F;
                if (++accent > signatureHi)
                {
                    accent = 1;
                    amp *= 2.0F;
                }
                Debug.Log("Tick: " + accent + "/" + signatureHi);
            }
            phase += amp * 0.3F;
            amp *= 0.993F;
            n++;
        }*/
    }
}