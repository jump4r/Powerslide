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
    public static readonly float bpm = 100f;
    public static readonly float offset = 2.390f; // was 2.655, changed for Dango Daikazoku
    public static float songPosition = 0f;
    public static float nextBeatTime = 0f; // time of the next beat.
    private static AudioSource source; // Source of the audio clip
    public static float spb;

    // Testing purposes
    private bool flip = true;

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

    // Debug
    private bool spawnOneNote = false;

    void Start()
    {
        nextBeatTime = offset + spb * 4;
        spb = 60f / (float)bpm; // 60 seconds / beats per minute
        accent = signatureHi;
        double startTick = AudioSettings.dspTime;
        sampleRate = AudioSettings.outputSampleRate;
        source = GetComponent<AudioSource>();
        nextTick = startTick * sampleRate;
        running = true;
    }

    void Update()
    {
        runTime += Time.deltaTime;
        songPosition = source.timeSamples / (float)source.clip.frequency;

        if (songPosition < offset)
            return; // Don't spawn yet.

        // Update the next beat time.
        if (songPosition > nextBeatTime /*&& !spawnOneNote */)
        {
            nextBeatTime += spb * 2;
            spawnOneNote = true;
            NoteSpawner.SpawnNote();
            // NoteSpawner.SpawnDrag();
            // NoteSpawner.SpawnHold(1, "true", 2);
            // NoteSpawner.SpawnFlick(2, 1, "l");
            /*
            if (!flip)
            {
                NoteSpawner.SpawnHold(1, "true", 2);
                NoteSpawner.SpawnFlick(2, 1, "l");
                flip = !flip;
            }

            else
            {
                NoteSpawner.SpawnHold(2, "true", 2);
                NoteSpawner.SpawnFlick(1, 2, "r");
                flip = !flip;
            }*/
        }

        // Debug.Log("Current Song Position: " + songPosition);
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