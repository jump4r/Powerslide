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
	void onEnable () {
        type = NoteType.Hold;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; // We will only need two vertices for this, the begining and the end.
	}
	
	// Update is called once per frame
	void Update () {
        float rTP = (Conductor.songPosition - StartTime) / (8f * Conductor.spb);
        transform.position = new Vector3(startPosition.x, startPosition.y - (8f * playerSpeedMult * Mathf.Sin(xRotation) * rTP), startPosition.z - (8f * playerSpeedMult * Mathf.Cos(xRotation) * rTP));

        lineRenderer.SetPosition(0, holdStartPoint);
        lineRenderer.SetPosition(1, holdEndPoint);

        CalculatePositions(); // Recalculate the positions of the points
    }

    public override void ParseDefinition(string def)
    {
        string[] splitString = def.Split();

        notePathID = int.Parse(splitString[1]);
        length = int.Parse(splitString[2]);
    }

    // May be unneccisary 
    public override void Construct(int NotePathID)
    {
        this.notePathID = NotePathID;
    }

    private void CalculatePositions()
    {
        holdStartPoint = this.transform.position;
        holdEndPoint = new Vector3(NotePath.NotePaths[notePathID].transform.position.x, holdStartPoint.y + (length * playerSpeedMult * Mathf.Sin(xRotation)), holdStartPoint.z + (length * playerSpeedMult * Mathf.Cos(xRotation)));
    }
}
