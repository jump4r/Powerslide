using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class NoteHold : NoteBase {

    // Hold Note: Player presses down to begin, and continues to hold the note until it finishes.
    // DEFINITION: [offset, startPath, length], where a length of "1" would represent a quarter note.

    [HideInInspector]
    public bool Active = false; // Determine to see whether the finger is still being held down.

    private bool isBeingHeld = false;

    private float length;
    // public int PathID;

    // Hold note calculations
    private Vector3 holdStartPoint; // segment[0] of the line
    private Vector3 holdEndPoint; // segment[1] of the line
    private Vector3 notePosition; // The position of the note as it falls

    // Line Renderer
    private LineRenderer lineRenderer;
    private Vector3 hitbarPosition { get; set; }

    // Is this particular hold note a transition?
    private bool isTransitionNote;

    // Use this for initialization
    void OnEnable() {
        type = NoteType.Hold;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.numPositions = 2; // We will only need two vertices for this, the begining and the end.
        lineRenderer.sortingOrder = -1;
        lineRenderer.material = LineMat;

        hitbarPosition = GameObject.FindGameObjectWithTag("Hitbar").transform.position;
        isBeingHeld = false;
    }

    // Update is called once per frame
    void Update() {

        rTP = 1f - (EndTime - Conductor.songPosition) / (NoteHelper.Whole * Conductor.spb);
        transform.position = new Vector3(startPosition.x, startPosition.y - (NoteHelper.Whole * playerSpeedMult * Mathf.Sin(xRotation) * rTP), startPosition.z - (NoteHelper.Whole * playerSpeedMult * Mathf.Cos(xRotation) * rTP));

        CheckToActivateNote();
        CheckToRemoveFromActiveNotesList();

        // Set the vertex positions of the Hold Line.
        CalculatePositions(); // Recalculate the positions of the points

        lineRenderer.SetPosition(0, holdStartPoint);
        lineRenderer.SetPosition(1, holdEndPoint);

    }

    // May be unneccisary 
    public override void Construct(Vector3 spawnPosition, float offset, int NotePathID, float length, bool isTransitionNote, string NoteName)
    {
        startPosition = spawnPosition;
        transform.position = spawnPosition;

        EndTime = offset;
        this.notePathID = NotePathID;
        gameObject.name = NoteName;
        this.length = length;
        this.isTransitionNote = isTransitionNote;

        lineRenderer.material = LineMat;

        // If this note should be constructed as a Transition note, remove the mesh renderer
        if (isTransitionNote)
        {
            GetComponent<MeshRenderer>().enabled = false;
            type = NoteType.Transition;
        }

        else
        {
            GetComponent<MeshRenderer>().enabled = true;
            type = NoteType.Hold;
        }
    }

    public override void Tapped(int notePathID)
    {
        IsTapped = true;
        isBeingHeld = true;

        GetComponent<Renderer>().enabled = false; // Remove the "Note" and leave only the line.
        CalculateHoldStartError();
    }

    public override void Held(int notePathID)
    {
        if (!IsTapped && !isTransitionNote)
        {
            Debug.Log("Android Debug: Note has not been primed, returning");
            return;
        }

        IsBeingHeld();
    }

    // Transitioned on hold note means that player moved from one Note Path to another
    // In this case, we need to check to see if the player had already tapped the note, so we can calculate the hold end error. 
    public override void Transitioned(int startPathID, int endPathID)
    {
        if (endPathID == this.notePathID)
        {
            Held(endPathID);
        }

        else if (IsTapped)
        {
            CalculateHoldEndError();
        }
    }

    // Finger lifted from the screen
    public override void Lift(int notePathID)
    {
        if (IsTapped)
        {
            CalculateHoldEndError();
        }

        isBeingHeld = false;
    }


    // Calculate the difference between the projected and actual hold STARTS (error when the player begins a hold note)
    // This will only be done on a proper hold note and note a transitional note.
    public override void CalculateHoldStartError()
    {
        // Transition notes don't have a HoldStart
        if (type == NoteType.Transition) return;

        float delta = Mathf.Abs(Conductor.songPosition - EndTime);

        // Update the accuracy
        if (delta < Conductor.spb / 8f)
        {
            GetComponent<Renderer>().material = Score100;
            sm.UpdateAccuracy(1f);
        }

        else
        {
            GetComponent<Renderer>().material = Score50;
            sm.UpdateAccuracy(0.5f);
        }
    }

    public override void CalculateHoldEndError()
    {
        float holdNoteEndTime = EndTime + Conductor.spb * length;
        float delta = Mathf.Abs(Conductor.songPosition - holdNoteEndTime);
        Debug.Log("Expected Hold Note Endtime: " + holdNoteEndTime + ", Actual Endtime: " + Conductor.songPosition);
        if (delta < Conductor.spb / 4f)
        {
            Debug.Log("ANDROID DEBUG: Relatively perfect");
        }
        else
        {
            ChangeMaterial(Score50);
        }
    }

    public override void IsBeingHeld()
    {
        // Update Score 
        sm.UpdateScore(Mathf.RoundToInt(300f * Conductor.spb * Time.deltaTime));
        ChangeMaterial(Score100);
    }

    public override void ChangeMaterial(Material mat)
    {
        lineRenderer.material = mat;
    }

    private void CalculatePositions()
    {
        if (!isBeingHeld)
        {
            holdStartPoint = this.transform.position;
        }

        else
        {
            holdStartPoint = new Vector3(this.transform.position.x, hitbarPosition.y, hitbarPosition.z);
        }

        holdEndPoint = new Vector3(NotePath.NotePaths[notePathID].transform.position.x, transform.position.y + (length * playerSpeedMult * Mathf.Sin(xRotation)), transform.position.z + (length * playerSpeedMult * Mathf.Cos(xRotation)));
    }


    private void CheckToRemoveFromActiveNotesList()
    {
        if (Conductor.songPosition >= EndTime + (Conductor.spb * length))
        {
            DestroyNote();
        }
    }

    protected override void ResetNote()
    {
        active = true;
        Active = false;
        IsTapped = false;
        isReadyToHit = false;

        GetComponent<Renderer>().enabled = true;
        GetComponent<Renderer>().material = Def;
        lineRenderer.material = LineMat;
    }

}
