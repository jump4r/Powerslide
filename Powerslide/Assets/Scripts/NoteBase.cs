using UnityEngine;
using System.Collections;
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

    // Path note is traveling down
    public int notePathID = 4;
    public int pathEnd; // Used in flick notes, although this should really not be here
    public NoteType type;
    public int noteValue = 0;

    // Use this for initialization
    void Start () {
        playerSpeedMult = 1f;
        bpm = 100;
        spb = (1f / (bpm / 60f));
        Debug.Log("Seconds Per Beat: " + spb);
        velocity = (1f / spb) * playerSpeedMult;
	}


    // Update is called once per frame
    void Update()
    {
        // I'm sure that we'll have a base note class that we'll add later
        transform.Translate(Vector3.down * Time.deltaTime * velocity);
    }

    // Trigger events for notes, add them to the collective note pile
    private void OnTriggerEnter(Collider col)
    {
        if (col.name != "Hitbar") return; // Wrong collision

        // Prepare to activiate a note for the first time
        if (noteValue == 0)
        {
            NotePath.NotePaths[notePathID].AddActiveNote(this);
            Debug.Log("Note in lane " + notePathID + " activated.");
        }
        noteValue += 50;
        //Debug.Log("Activiate: " + noteValue);
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.name != "Hitbar") return; // Wrong collision

        noteValue -= 50;

        // MISSED NOTE IF 0, Deactivate Note
        if (noteValue == 0)
        {
            NotePath.NotePaths[notePathID].RemoveActiveNote(this);
            Debug.Log("Note in lane " + notePathID + " deactivated.");
        }
        //Debug.Log("Deactivate:" + noteValue);
    }

    // Virtual Functions
    public virtual void ChangeMaterial() { }
}
