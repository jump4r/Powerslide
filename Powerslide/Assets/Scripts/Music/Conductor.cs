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
    public static float SpawnTimeOffset;

    [SerializeField]
    private AudioSource source; // Source of the audio clip

    public static float spb;

    private Beatmap beatmap;
    public static int OffsetDifference;
    private int currentNoteIndex = 0;

    // Song
    private bool isNotesFinished = false;

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
        accent = signatureHi;
        double startTick = AudioSettings.dspTime;
        sampleRate = AudioSettings.outputSampleRate;
        source = GetComponent<AudioSource>();
        nextTick = startTick * sampleRate;
        running = true;

        nextBeatTime = offset;
        Debug.Log("Set Spawn Time: " + spawnTime);
    }

    // Takes information from the beatmap and loads the paramters into the conductor
    public void LoadBeatmap(Beatmap beatmap)
    {
        if (source == null) { source = GetComponent<AudioSource>(); }
        this.beatmap = beatmap;
        source.clip = beatmap.song;
        spb = 60f / beatmap.BPM; // Seconds per beat
        offset = beatmap.OsuOffset / 1000f; // Offset is in milliseconds for easier readibility
        OffsetDifference = beatmap.OffsetDifference; // Difference between the osu offset and the actual offset of the song
        nextBeatTime = GetNextBeatTime(CompileNoteForSpawning(beatmap.Notes[currentNoteIndex]));
        SpawnTimeOffset = spb * NoteHelper.Whole; // Difference between the EndTime and the SpawnTime
        spawnTime = nextBeatTime - SpawnTimeOffset;
        Play(); // Play the audio
    }

    private float GetNextBeatTime(string hitNote)
    {
        float hitTime = float.Parse(hitNote.Split(',')[0]) / 1000f;
        Debug.Log("Next Beat Time: " + hitTime);
        return hitTime;
    }

    void Update()
    {
        runTime += Time.deltaTime;
        songPosition = source.timeSamples / (float)source.clip.frequency;
        Debug.Log("Song Position: " + songPosition + ", SpawnTime: " + spawnTime);

        /*
        if (songPosition < offset)
            return; // Don't spawn yet. */

        // Update the next beat time.
        while (songPosition > spawnTime && currentNoteIndex < beatmap.Notes.Count)
        {
            Debug.Log("Spawn Time: " + spawnTime + ", Song Position " + songPosition);
            NoteSpawner.SpawnHitObject(CompileNoteForSpawning(beatmap.Notes[currentNoteIndex]));

            // Update Timings: 
            currentNoteIndex++;
            if (currentNoteIndex < beatmap.Notes.Count)
            {
                nextBeatTime = GetNextBeatTime(CompileNoteForSpawning(beatmap.Notes[currentNoteIndex]));
                spawnTime = nextBeatTime - SpawnTimeOffset;
            }
            // Debug.Log("Current Song Position: " + songPosition);
        }

        // Check to finish
        CheckIfFinished();

        // Debug.Log("Current Song Position: " + songPosition + ", Spawn Time: " + spawnTime);
    }

    // Given a raw note, change the EndTime (first element in the Note string array), by adding the OffsetDifference, and then return the new note.
    private string CompileNoteForSpawning(string note)
    {
        string[] split = note.Split(',');
        int newEndTime = int.Parse(split[0]) + OffsetDifference;
        split[0] = newEndTime.ToString();
        string rtn = string.Join(",", split);
        return rtn;
    }

    public void Play()
    {
        if (source.clip != null) {
            source.Play();
        }
    }

    private void CheckIfFinished()
    {
        if (currentNoteIndex < beatmap.Notes.Count) { return; } // Check to see if we are not on the alst note
        if (songPosition > spawnTime + (NoteHelper.Whole * 2f * spb))
        {
            Finish();
        }
    }

    // Song has been finished, display score screen.
    public void Finish()
    {
        // To do: Figure out a better place to put this Clear function. We need to clear all of the notepaths out of the static list.
        NotePath.NotePaths.Clear(); 
        GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().DestroyBeatmapBeforeLoad();
        GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().ChangeLevel(2);
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