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

    public static Conductor instance = null;
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

    public AudioSource source; // Source of the audio clip

    public static float spb;

    private Beatmap beatmap;
    private int currentNoteIndex = 0;

    private bool running = false;
    private float runTime = 0f;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
        }

        source = GetComponent<AudioSource>();
        running = true;

        nextBeatTime = offset;
    }

    // Takes information from the beatmap and loads the paramters into the conductor
    public void LoadBeatmap(Beatmap beatmap)
    {
        if (source == null) { source = GetComponent<AudioSource>(); }
        this.beatmap = beatmap;
        source.clip = beatmap.song;
        spb = 60f / beatmap.BPM; // Seconds per beat
        offset = beatmap.RawOffset / 1000f; // Offset is in milliseconds for easier readibility

        nextBeatTime = GetNextBeatTime(beatmap.Notes[currentNoteIndex]);
        SpawnTimeOffset = spb * NoteHelper.Whole; // Difference between the EndTime and the SpawnTime
        spawnTime = nextBeatTime - SpawnTimeOffset;

        Play(); // Play the audio
    }

    private float GetNextBeatTime(string hitNote)
    {
        float hitTime = float.Parse(hitNote.Split(',')[0]) / 1000f;
        // Debug.Log("Next Beat Time: " + hitTime);
        return hitTime;
    }

    void Update()
    {
        // Only run update if there is an Audio Clip
        if (source.clip == null)
        {
            Debug.Log(" No clip to play");
            return;
        }

        runTime += Time.deltaTime;
        songPosition = source.timeSamples / (float)source.clip.frequency;

        // Update the next beat time.
        while (songPosition > spawnTime && currentNoteIndex < beatmap.Notes.Count)
        {
            // Debug.Log("Spawn Time: " + spawnTime + ", Song Position " + songPosition + ", bIndex: " + currentNoteIndex + "/" + beatmap.Notes.Count);
            NoteSpawner.SpawnHitObject(beatmap.Notes[currentNoteIndex]);

            // Update Timings: 
            currentNoteIndex++;
            if (currentNoteIndex < beatmap.Notes.Count)
            {
                nextBeatTime = GetNextBeatTime(beatmap.Notes[currentNoteIndex]);
                spawnTime = nextBeatTime - SpawnTimeOffset;
            }
            // Debug.Log("Current Song Position: " + songPosition);
        }

        // Check to finish
        CheckIfFinished();

        // Debug.Log("Current Song Position: " + songPosition + ", Spawn Time: " + spawnTime);
    }

    public void Play()
    {
        if (source.clip != null) {
            source.Play();
        }

        else
        {
            Debug.LogWarning("No Audio Clip Present");
        }
    }

    private void CheckIfFinished()
    {
        if (currentNoteIndex < beatmap.Notes.Count) { return; } // Check to see if we are not on the alst note
        if (songPosition > spawnTime + (NoteHelper.Whole * 4f * spb))
        {
            Finish();
        }
    }

    // Song has been finished, display score screen.
    public void Finish()
    {
        // To do: Figure out a better place to put this Clear function. We need to clear all of the notepaths out of the static list.
        NotePath.NotePaths.Clear();
        LevelManager.instance.ResetPlayground();

        LevelManager.instance.ChangeLevel(LevelEnum.RESULTS);
    }
}