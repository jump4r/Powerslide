﻿using UnityEngine;
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
    public bool active = true; // Has the note been hit yet?
    public bool IsTapped = false; // Has the note been tapped?
    public bool isReadyToHit = false;

    // Access Game Manager so we can update the score
    [SerializeField]
    protected ScoreManager sm;

    // Score Variables
    public Material Def; // Defautl Material
    public Material LineMat;
    public Material Score100;
    public Material Score50;

    public const int HIT_PERFECT = 300;
    public const int HIT_GOOD = 100;
    public const int HIT_BAD = 50;
    public const int HIT_MISS = 0;

    // Speed based things
    public int bpm = 100; // Beats per minute
    public float velocity; // = playerSpeedMult * (1 / (bpm / 60s)) or 1 / spb, we are going to travel 1 meter per beat. Increase the playerSpeedMult for increase
    public float spb; // Seconds per beat, how many seconds pass for one beat to occur (ie. 1spb = 60bpm, lower = higher bpm).
    public float playerSpeedMult; // Player speed mulitplier
    public float rTP = 0f; // Ratio of how long has passed between the note spawn and the hit marker. Basically : StartTime / PlannedEndTime

    // Start and End position, rotation, and timings
    public Vector3 startPosition;
    public Vector3 endPosition;
    public static readonly float xRotation = 55f * Mathf.PI / 180f;
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

    // The finger currently hitting the note
    protected int fingerId;

    // Use this for initialization
    void Start() {
        playerSpeedMult = Settings.PlayerSpeedMult;
        startPosition = (this.gameObject.transform.position);
        endPosition = new Vector3(startPosition.x, startPosition.y - (8f * playerSpeedMult * Mathf.Sin(xRotation)), startPosition.z - (8f * playerSpeedMult * Mathf.Cos(xRotation)));
        StartTime = Conductor.songPosition;

        // Get The GM
        sm = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();

        // Set materials for 50/100 scores
        Def = Resources.Load("Materials/Playnote", typeof(Material)) as Material;
        LineMat = Resources.Load("Materials/Playnote", typeof(Material)) as Material;
        Score100 = Resources.Load("Materials/100Score", typeof(Material)) as Material;
        Score50 = Resources.Load("Materials/50Score", typeof(Material)) as Material;

        // Load a personal hitsound for each object as a test.
        hitSound = Resources.Load("Sound FX/hitsound") as AudioClip;
    }

    // Update is called once per frame
    void Update()
    {
        // Do not update the note when it isn't active
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        rTP = 1f - (EndTime - Conductor.songPosition) / (NoteHelper.Whole * Conductor.spb); // Ratio of completion for the song
        transform.position = new Vector3(startPosition.x, startPosition.y - (NoteHelper.Whole * playerSpeedMult * Mathf.Sin(xRotation) * rTP), startPosition.z - (NoteHelper.Whole * playerSpeedMult * Mathf.Cos(xRotation) * rTP));
        CheckToActivateNote(); // Bad Form will fix later
        CheckToDeactivateNote();
    }

    // Virtual Functions
    public virtual void ChangeMaterial(Material mat) { }
    public virtual void Construct(Vector3 spawnPosition, float offset, int NotePathID, string NoteName) { } // Construct a Regular note
    public virtual void Construct(Vector3 spawnPosition, float offset, int NotePathID, float length, bool isTransition, string NoteName) { } // Construct a Hold note
    public virtual void Construct(Vector3 spawnPosition, float offset, int startPath, int endPath, bool direction, string NoteName) { } // Construct a Flick note.
    public virtual void Construct(Vector3 spawnPosition, float offset, int startPath, int endPath, float length, NoteDragType noteDragType, string NoteName) { } // Construct a Drag Note

    protected virtual void ResetNote() { }
    public virtual void ParseDefinition(string def) { } // Parse the definition of the note
    public virtual void SetFingerId(int id) { } // Set the finger id of the note

    // Actions to take given state change from Finger
    public virtual void Tapped() { }
    public virtual void Tapped(int notePathID) { }
    public virtual void Held() { }
    public virtual void Held(int notePathID) { }
    public virtual void Transitioned(int startPathID, int endPathID) { }
    public virtual void Lift() { }
    public virtual void Lift(int notePathID) { }

    // Hold Note Virutal Functions
    public virtual void IsBeingHeld() { }
    public virtual void CalculateHoldStartError() { }
    public virtual void CalculateHoldEndError() { }

    // Check to see if the note has gotten close enough to the hitbar.
    public virtual void CheckToActivateNote()
    {
        if (rTP > 0.9f && !isReadyToHit)
        {
            // Prepare to activiate a note for the first time
            if (type != NoteType.Drag)
            {
                NotePath.NotePaths[notePathID].AddActiveNote(this);
            }
            // Debug.Log(gameObject.name + " is ready to hit at: " + Conductor.songPosition + ", should be around " + EndTime);
            isReadyToHit = true;
        }
    }

    public virtual void CheckToDeactivateNote()
    {
        if (rTP > 1.1f && isReadyToHit)
        {
            // MISSED NOTE IF 0, Deactivate Note
            // We dont' want to destroy notes that have a "length" field.
            if (type != NoteType.Drag && type != NoteType.Hold && type != NoteType.Transition)
            {
                // Update the score if we haven't already (In the case where the player missed the note)
                if (active)
                {
                    sm.UpdateAccuracy(0f);
                    sm.UpdateScore(HIT_MISS);
                }
                DestroyNote();
            }
        }
    }

    // Difference between Player hit and perfect timing.
    public void CalculateError()
    {
        float delta = Mathf.Abs(Conductor.songPosition - EndTime); // Error calculation

        // Update the Score based on the accuracy of the hit.
        if (delta < Conductor.spb / 4f)
        {
            sm.UpdateAccuracy(1f);
            sm.UpdateScore(HIT_PERFECT);
        }
        else
        {
            sm.UpdateAccuracy(0.5f);
            sm.UpdateScore(HIT_GOOD);
        }
        active = false;
        //Invoke("DestroyNote", .2f);
        DestroyNote();
    }

    public void DestroyNote()
    {
        NotePath.NotePaths[notePathID].RemoveActiveNote(this);
        ResetNote();
        gameObject.transform.position = new Vector3(-99f, -99f, -99f);
        gameObject.SetActive(false);
    }
}
