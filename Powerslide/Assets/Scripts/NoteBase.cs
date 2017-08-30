using UnityEngine;
using System.Collections;

public enum NoteType
{
    Regular = 0,
    Flick = 1,
    Drag = 2,
    Hold = 3,
    Transition = 4,
    NULL = 5
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class NoteBase : MonoBehaviour {

    public string Name;
    public string definition;
    public bool active = true;
    public bool isReadyToHit = false;

    // Use this for initialization
    public Material Score100;
    public Material Score50;

    // Time note is to arrive at the hit bar
    public int time;

    // Speed based things
    public int bpm = 100; // Beats per minute
    public float velocity; // = playerSpeedMult * (1 / (bpm / 60s)) or 1 / spb, we are going to travel 1 meter per beat. Increase the playerSpeedMult for increase
    public float spb; // Seconds per beat, how many seconds pass for one beat to occur (ie. 1spb = 60bpm, lower = higher bpm).
    public float playerSpeedMult; // Player speed mulitplier
    private float rTP = 0f; // Ratio of how long has passed between the note spawn and the hit marker. Basically : StartTime / PlannedEndTime

    // Start and End position, rotation, and timings
    public Vector3 startPosition;
    public Vector3 endPosition;
    public static readonly float xRotation = 55f * Mathf.PI / 180f;
    [HideInInspector]
    public float EndTime; // When the note hits the hitbar
    [HideInInspector]
    public float StartTime; // When the note spawns

    // Path note is traveling down
    public int notePathID = 4;
    public int startPath;
    public int endPath; // Used in flick notes, although this should really not be here
    public NoteType type;
    public int noteValue = 0;

    // For testing purposes
    private AudioClip hitSound;

    // Use this for initialization
    void Start () {
        playerSpeedMult = 2f;
        startPosition = (this.gameObject.transform.position);
        endPosition = new Vector3(startPosition.x, startPosition.y - (8f * playerSpeedMult * Mathf.Sin(xRotation)), startPosition.z - (8f * playerSpeedMult * Mathf.Cos(xRotation)));
        // Debug.Log("Distance from Start to finish of note: " + Vector3.Distance(startPosition, endPosition));
        StartTime = Conductor.songPosition;
        EndTime = StartTime + (8f * Conductor.spb); // 8 beats after the spawn time, that's the end time

        // Set materials for 50/100 scores
        Score100 = Resources.Load("Materials/100Score", typeof(Material)) as Material;
        Score50 = Resources.Load("Materials/50Score", typeof(Material)) as Material;

        // Load a personal hitsound for each object as a test.
        hitSound = Resources.Load("Sound FX/hitsound") as AudioClip;
    }

    // Update is called once per frame
    void Update()
    {
        // I'm sure that we'll have a base note class that we'll add later
        // transform.Translate(Vector3.down * Time.deltaTime * velocity);
        rTP = (Conductor.songPosition - StartTime) / (8f * Conductor.spb);
        // Debug.Log("Current ratio: " + (Conductor.songPosition - startSongPosition) + " / " + 8f * Conductor.spb);
        // Debug.Log("rTP set to: " + rTP);
        // rTP = 1f - rTP;
        transform.position = new Vector3(startPosition.x, startPosition.y - (8f * playerSpeedMult * Mathf.Sin(xRotation) * rTP), startPosition.z - (8f * playerSpeedMult * Mathf.Cos(xRotation) * rTP));

        /****************************
         * TEST *
         * Play a hitsound when the note hits the perfect position, then destory it.
         ***************************/
        /* 
         if (Conductor.songPosition > EndTime)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().PlayHitSound();
            Debug.Log("Projected End Time: " + EndTime + ", Actual End Time: " + Conductor.songPosition);
            Destroy(this.gameObject);
        } */
    }

    // Virtual Functions
    public virtual void ChangeMaterial(Material mat) { /* GetComponent<MeshRenderer>().material = mat; */ }
    public virtual void Construct(int NotePathID, string NoteName) { } // Construct a Regular note
    public virtual void Construct(int NotePathID, string NoteName, string direction) { } // Construction  a Flick note.
    public virtual void ParseDefinition(string def) { } // Parse the definition of the note

    // Hold Note Virutal Functions
    public virtual void IsBeingHeld() { }
    public virtual void CalculateHoldStartError() { }
    public virtual void CalculateHoldEndError() { }

    // Difference between Player hit and perfect timing.
    public void CalculateError()
    {
        float delta = Mathf.Abs(Conductor.songPosition - EndTime); // Error calculation
        Debug.Log("Error Calculation results in: " + delta);
        if (delta < Conductor.spb / 4f)
            ChangeMaterial(Score100);

        else
            ChangeMaterial(Score50);

        //Invoke("DestroyNote", .2f);
    }

    // Trigger events for notes, add them to the collective note pile
    private void OnTriggerEnter(Collider col)
    {
        if (col.name != "Hitbar") return; // Wrong collision
        if (isReadyToHit) return; // We have already entered the collision, and do not want to add more notes to the ActiveNotes list.


        // Prepare to activiate a note for the first time
        if (/* noteValue == 0 && */ type != NoteType.Drag)
        {
            NotePath.NotePaths[notePathID].AddActiveNote(this);
        }

        isReadyToHit = true;
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.name != "Hitbar") return; // Wrong collision
        if (this.gameObject.GetComponent<NoteDrag>() != null) return; // We are not removing the object for drag notes >:3
        if (!isReadyToHit) return; // The object has already be deactiviated

        // MISSED NOTE IF 0, Deactivate Note
        // We dont' want to destroy notes that have a "length" field.
        if (type != NoteType.Drag && type != NoteType.Hold && type != NoteType.Transition)
        {
            NotePath.NotePaths[notePathID].RemoveActiveNote(this);
            isReadyToHit = false;
            // Just destory it I guess
            Destroy(this.gameObject);
        }
    }

    public void DestroyNote()
    {
        Destroy(this.gameObject);
    }
}
