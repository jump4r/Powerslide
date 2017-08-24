//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[RequireComponent(typeof(LineRenderer))]
//public class NoteHoldTransition : NoteBase {

//    // Hold Note: Player presses down to begin, and continues to hold the note until it finishes.
//    // DEFINITION: [offset, startPath, length], where a length of "1" would represent a quarter note.

//    [HideInInspector]
//    public bool Active = false; // Determine to see whether the finger is still being held down.

//    private float length;
//    // public int PathID;

//    // Hold note calculations
//    private Vector3 holdStartPoint;
//    private Vector3 holdEndPoint;

//    // Line Renderer
//    private LineRenderer lineRenderer;

//    // Next Note
//    private NoteBase nextNote;

//    // Use this for initialization
//    void OnEnable () {
//        type = NoteType.Transition;
//        lineRenderer = GetComponent<LineRenderer>();
//        lineRenderer.positionCount = 2;
//        Debug.Log(lineRenderer.positionCount);
//    }

//    // May be unneccisary 
//    public override void Construct(int NotePathID, string NoteName)
//    {
//        this.notePathID = NotePathID;
//        gameObject.name = NoteName;

//    }

//    // Parse the definition of a hold note.
//    // Definition [offset, startTime, length, nextNote] 
//    // NextNote - The next note in the series of transitions.
//    public override void ParseDefinition(string def)
//    {
//        string[] splitString = def.Split(',');

//        notePathID = int.Parse(splitString[1]);
//        length = int.Parse(splitString[2]);
//    }

//    // Update is called once per frame
//    void Update () {
//        CheckToRemoveFromActiveNotesList();
//        float rTP = (Conductor.songPosition - StartTime) / (8f * Conductor.spb);
//        transform.position = new Vector3(startPosition.x, startPosition.y - (8f * playerSpeedMult * Mathf.Sin(xRotation) * rTP), startPosition.z - (8f * playerSpeedMult * Mathf.Cos(xRotation) * rTP));

//        CalculatePositions();

//        lineRenderer.SetPosition(0, holdStartPoint);
//        lineRenderer.SetPosition(1, holdEndPoint);
//    }

//    // Calculates the current positions of the hold line falling down the playfield.
//    private void CalculatePositions()
//    {
//        holdStartPoint = this.transform.position;
//        holdEndPoint = new Vector3(NotePath.NotePaths[notePathID].transform.position.x, holdStartPoint.y + (length * playerSpeedMult * Mathf.Sin(xRotation)), holdStartPoint.z + (length * playerSpeedMult * Mathf.Cos(xRotation)));
//    }

//    private void CheckToRemoveFromActiveNotesList()
//    {
//        if (Conductor.songPosition >= EndTime + length * Conductor.spb)
//        {
//            /* Transfer activity to the next note */
//            Debug.Log("Hold (Transition) is finished");
//            NotePath.NotePaths[this.notePathID].RemoveActiveNote(this);
//            Destroy(this.gameObject);
//        }
//    }


//    // Calculate the difference between the projected and actual hold STARTS (error when the player begins a hold note)
//    public void CalculateHoldStartError()
//    {
//        float delta = Mathf.Abs(Conductor.songPosition - EndTime);
//        //Debug.Log("Expected Hold Note STARTTIME: " + EndTime + ", Actual STARTTIME: " + Conductor.songPosition);
//        Debug.Log("Delta: " + delta + ", threshold for 50 score, greater than: " + Conductor.spb / 4f);
//        if (delta < Conductor.spb / 4f)
//        {
//            noteValue = 100;
//        }

//        else
//        {
//            noteValue = 50;
//        }
//        ChangeMaterial();
//    }

//    public new void CalculateError()
//    {
//        float holdNoteEndTime = EndTime + Conductor.spb * length;
//        float delta = Mathf.Abs(Conductor.songPosition - holdNoteEndTime);
//        Debug.Log("Expected Hold Note Endtime: " + holdNoteEndTime + ", Actual Endtime: " + Conductor.songPosition);
//        if (delta < Conductor.spb / 4f)
//        {
//            noteValue = 100;
//        }

//        else
//        {
//            noteValue = 50;
//        }
//        ChangeMaterial();
//    }

//    public override void ChangeMaterial()
//    {
//        if (noteValue == 100)
//        {
//            lineRenderer.material = Score100;
//        }

//        else if (noteValue == 50)
//        {
//            lineRenderer.material = Score50;
//        }
//    }
//}
