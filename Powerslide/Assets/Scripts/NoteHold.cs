using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class NoteHold : NoteBase {

    // Hold Note: Player presses down to begin, and continues to hold the note until it finishes.
    // DEFINITION: [offset, startPath, length], where a length of "1" would represent a quarter note.

    [HideInInspector]
    public bool Active = false; // Determine to see whether the finger is still being held down.

    private float length;
    // public int PathID;

    // Hold note calculations
    private Vector3 holdStartPoint;
    private Vector3 holdEndPoint;

    // Line Renderer
    private LineRenderer lineRenderer;

	// Use this for initialization
	void OnEnable () {
        type = NoteType.Hold;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; // We will only need two vertices for this, the begining and the end.
	}
	
	// Update is called once per frame
	void Update () {

        CheckToRemoveFromActiveNotesList();
        float rTP = (Conductor.songPosition - StartTime) / (8f * Conductor.spb);
        transform.position = new Vector3(startPosition.x, startPosition.y - (8f * playerSpeedMult * Mathf.Sin(xRotation) * rTP), startPosition.z - (8f * playerSpeedMult * Mathf.Cos(xRotation) * rTP));

        // Set the vertex positions of the Hold Line.
        CalculatePositions(); // Recalculate the positions of the points
        if (lineRenderer == null) { Debug.Log("Line Renderer not loaded");  }
        lineRenderer.SetPosition(0, holdStartPoint);
        lineRenderer.SetPosition(1, holdEndPoint);

    }

    public override void ParseDefinition(string def)
    {
        string[] splitString = def.Split(',');

        notePathID = int.Parse(splitString[1]);
        length = int.Parse(splitString[2]);
    }

    // May be unneccisary 
    public override void Construct(int NotePathID, string NoteName)
    {
        this.notePathID = NotePathID;
        gameObject.name = NoteName;

    }

    // Calculate the difference between the projected and actual hold STARTS (error when the player begins a hold note)
    public void CalculateHoldStartError()
    {
        float delta = /*Mathf.Abs*/(Conductor.songPosition - EndTime);
        //Debug.Log("Expected Hold Note STARTTIME: " + EndTime + ", Actual STARTTIME: " + Conductor.songPosition);
        Debug.Log("Delta: " + delta + ", threshold for 50 score, greater than: " + Conductor.spb / 2f);
        if (delta < Conductor.spb / 2f)
        {
            noteValue = 50;
        }

        else
        {
            noteValue = 100;
        }
        ChangeMaterial();
    }

    public new void CalculateError()
    {
        float holdNoteEndTime = EndTime + Conductor.spb * length;
        float delta = Mathf.Abs(Conductor.songPosition - holdNoteEndTime);
        Debug.Log("Expected Hold Note Endtime: " + holdNoteEndTime + ", Actual Endtime: " + Conductor.songPosition);
        if (delta < Conductor.spb / 2f)
        {
            noteValue = 100;
        }
        
        else
        {
            noteValue = 50;
        }
        ChangeMaterial();
    }

    public override void ChangeMaterial()
    {
        if (noteValue == 100)
        {
            lineRenderer.material = Score100;
        }

        else if (noteValue == 50)
        {
            lineRenderer.material = Score50;
        }
    }

    private void CalculatePositions()
    {
        holdStartPoint = this.transform.position;
        holdEndPoint = new Vector3(NotePath.NotePaths[notePathID].transform.position.x, holdStartPoint.y + (length * playerSpeedMult * Mathf.Sin(xRotation)), holdStartPoint.z + (length * playerSpeedMult * Mathf.Cos(xRotation)));
    }

    private void CheckToRemoveFromActiveNotesList()
    {
        if (Conductor.songPosition >= EndTime + (Conductor.spb * length))
        {
            Debug.Log("Note in Note Path " + notePathID + " is finished");
            NotePath.NotePaths[this.notePathID].RemoveActiveNote(this);
            Destroy(this.gameObject);
        }
    }
    
}
