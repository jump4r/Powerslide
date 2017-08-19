using UnityEngine;
using System.Collections;

public enum NoteType
{
    Regular = 0,
    Flick = 1,
    Drag = 2,
    NULL = 3
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]


public class NoteBase : MonoBehaviour {

    public string definition;
    public bool active = true;

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
    public float EndTime;
    [HideInInspector]
    public float StartTime;

    // Path note is traveling down
    public int notePathID = 4;
    public int pathEnd; // Used in flick notes, although this should really not be here
    public NoteType type;
    public int noteValue = 0;

    // Use this for initialization
    void Start () {
        playerSpeedMult = 2f;
        startPosition = (this.gameObject.transform.position);
        endPosition = new Vector3(startPosition.x, startPosition.y - (8f * playerSpeedMult * Mathf.Sin(xRotation)), startPosition.z - (8f * playerSpeedMult * Mathf.Cos(xRotation)));
        // Debug.Log("Distance from Start to finish of note: " + Vector3.Distance(startPosition, endPosition));
        StartTime = Conductor.songPosition;
        EndTime = StartTime + (8f * Conductor.spb); // 8 beats after the spawn time, that's the end time
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
    }

    // Virtual Functions
    public virtual void ChangeMaterial() { }
    public virtual void Construct(int NotePathID) { } // Construct the note

    // Difference between Player hit and perfect timing.
    public void CalculateError()
    {
        float delta = Mathf.Abs(Conductor.songPosition - EndTime); // Error calculation
        Debug.Log("Error Calculation results in: " + delta);
        if (delta < Conductor.spb / 2f)
            noteValue = 100;

        else
            noteValue = 50;

        ChangeMaterial();
    }

    // Trigger events for notes, add them to the collective note pile
    private void OnTriggerEnter(Collider col)
    {
        if (col.name != "Hitbar") return; // Wrong collision
       


        // Prepare to activiate a note for the first time
        if (noteValue == 0 && type != NoteType.Drag)
        {
            NotePath.NotePaths[notePathID].AddActiveNote(this);
            Debug.Log("Note in lane " + notePathID + " activated.");
        }
        //noteValue += 50;
        //Debug.Log("Activiate: " + noteValue);
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.name != "Hitbar") return; // Wrong collision
        if (this.gameObject.GetComponent<NoteDrag>() != null) return; // We are not removing the object for drag notes >:3

        //noteValue -= 50;

        // MISSED NOTE IF 0, Deactivate Note
        if (noteValue == 0 && type != NoteType.Drag)
        {
            NotePath.NotePaths[notePathID].RemoveActiveNote(this);
            Debug.Log("Note in lane " + notePathID + " deactivated.");
        }
        //Debug.Log("Deactivate:" + noteValue);
    }
}
