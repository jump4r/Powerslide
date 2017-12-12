using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// The type of drag note
public enum NoteDragType
{
    Linear = 0,
    Curve = 1,
    Root = 2
}

[RequireComponent(typeof(LineRenderer))]
public class NoteDrag : NoteBase {

    // Definition of a DRAGNOTE: [offset,numSections,startPath,endPath,length]
    // Length is calculated in number of beats (seconds), 

    // Player notes: Drag slider is hit a lot by accident, remove it in 

    // Collisions will probably be handled differently here
    // 1) Check to see whether or not the starting Point is in line with HitBar
    // 2) Start checking after Conductor.songPosition + 8 * playerSpeedMult * spb has been reached. 

    // DEBUG VARS
    private GameObject DragNoteDebugger;

    // Drag Note Positional Variables
    private int numSegments = 16; // The number of segments we want to represent our "curve". A numSegments of 2 will be used for a straight line
    private float totalHeight;

    private float dragStartPos;
    private float dragEndPos;

    private NoteDragType dragType;
    private List<Vector3> segments;

    private float length; // Calculated in beats

    public float EndSliderTime;
    private Transform sliderTrasform;

    private bool dragNoteActive = false; // A little confusing, because we have a 'active' bool in the NoteBase, should be changed to 


    // Line renderer
    private LineRenderer lineRenderer;

    void OnEnable()
    {
        type = NoteType.Drag;
        DragNoteDebugger = GameObject.Find("Drag Note Debugger");
        sliderTrasform = GameObject.Find("Slider").transform;
        lineRenderer = GetComponent<LineRenderer>();
        
    }

    public override void Construct(Vector3 spawnPosition, float offset, int startPath, int endPath, float length, NoteDragType dragType, string NoteName)
    {
        startPosition = spawnPosition;
        transform.position = spawnPosition;

        EndTime = offset;
        this.notePathID = startPath;
        this.startPath = startPath;
        this.endPath = endPath;
        this.length = length;
        this.dragType = dragType;
        name = NoteName;

        // Determine number of segments needed
        if (dragType == NoteDragType.Linear)
        {
            numSegments = 2;
        }

        else
        {
            numSegments = 16;
        }

        // Load the segment points.
        segments = new List<Vector3>(numSegments);
        for (int i = 0; i < numSegments; i++)
        {
            segments.Add(Vector3.zero);
        }

        
        lineRenderer.numPositions = numSegments;

        Debug.Log("Amount of segments " + lineRenderer.numPositions);

        dragStartPos = NotePath.NotePaths[startPath].transform.position.x;
        dragEndPos = NotePath.NotePaths[endPath].transform.position.x;
        totalHeight = Mathf.Pow(numSegments-1, 2);
    }

   
    private void Update()
    {
        CheckToRemoveFromActiveNotesList();

        rTP = 1f - (EndTime - Conductor.songPosition) / (NoteHelper.Whole * Conductor.spb);
        transform.position = new Vector3(startPosition.x, 
                                         startPosition.y - (NoteHelper.Whole * playerSpeedMult * Mathf.Sin(xRotation) * rTP),
                                         startPosition.z - (NoteHelper.Whole * playerSpeedMult * Mathf.Cos(xRotation) * rTP));

        for (int i = 0; i < lineRenderer.numPositions; i++)
        {
            lineRenderer.SetPosition(i, segments[i]);
        }

        CalculatePositions(); // Recalculate the positions of the points

        // Potentially Activate real note
        // This looks kinda ugly tbh
        if (Conductor.songPosition > EndTime && Conductor.songPosition < EndTime + (length * Conductor.spb) && !dragNoteActive)
        {
            dragNoteActive = true;
            NotePath.NotePaths[notePathID].AddActiveNote(this);
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().SetActiveDragNote(this);
        }

        // Potentially Activate real note
        if (Conductor.songPosition > EndTime + (length * Conductor.spb) && dragNoteActive)
        {
            dragNoteActive = false;
            NotePath.NotePaths[notePathID].RemoveActiveNote(this);
        }
    }

    //  the starting and ending positions of theslider.
    private void CalculatePositions()
    {
        float xOffset = (dragEndPos - dragStartPos) / (numSegments - 1); // How far apart the curve segement points will be placed.
        for (int i = 0; i < segments.Count; i++)
        {
            float curveHeight = CurveExponential(i);
            segments[i] = new Vector3(transform.position.x + (xOffset * i),
                                      transform.position.y + ((curveHeight / totalHeight) * length * playerSpeedMult * Mathf.Sin(xRotation)),
                                      transform.position.z + ((curveHeight / totalHeight) * length * playerSpeedMult * Mathf.Cos(xRotation)));
        }
    }

    // For Root Types, return square root,
    // For Linear and Curve Types, return the index squared 
    private float CurveExponential(float index)
    {
        // This is actually more complicated than I expected.
        // We can't do a simple square root, but rather we're doing an "upside down x^2"
        if (dragType == NoteDragType.Root)
        {
            if (index == 0) return 0;
            float newIndex = numSegments - index;
            return totalHeight - Mathf.Pow(newIndex, 2f);
        }

        return Mathf.Pow(index, 2f);
    }


    // Determines the x position of point on a curved slider
    public float GetXRelPos()
    {
        float x0 = NotePath.NotePaths[startPath].transform.position.x;
        float x1 = NotePath.NotePaths[endPath].transform.position.x;
        float tRatio = (Conductor.songPosition - EndTime) / (length * Conductor.spb);

        // We need to do some more math depending on what the curve type is
        if (dragType == NoteDragType.Curve)
        {
            tRatio = Mathf.Sqrt(tRatio);
        }

        else if (dragType == NoteDragType.Root) // Incorrect
        {
            tRatio = Mathf.Pow(tRatio, 2f);
        }

        float xRelPos = x0 + (x1 - x0) * tRatio;
        return xRelPos;
    }

    public void CheckIfOnPath(Transform sliderPosition)
    {
        float xRelPos = GetXRelPos();
        //Debug.Log("Slider X Position: " + sliderPosition.position.x + ", Relative xPos of Drag Note: " + xRelPos);

        // I believe 1.14  = Width of one NotePath * 2
        if (Mathf.Abs(xRelPos - sliderPosition.position.x) < 1.14f / 2f) // WHAT IS THIS FLOAT LMAO
        {
            lineRenderer.material = Score100;
            sm.UpdateScore(Mathf.RoundToInt(HIT_PERFECT * Conductor.spb * Time.deltaTime));
        }

        else
        {
            lineRenderer.material = Score50;
        }
#if UNITY_EDITOR
        DragNoteDebugger.transform.position = new Vector3(xRelPos, DragNoteDebugger.transform.position.y, DragNoteDebugger.transform.position.z);
#endif
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
        dragNoteActive = false;
        IsTapped = false;
        isReadyToHit = false;

        ChangeMaterial(Def);
        lineRenderer.material = LineMat;

        Destroy(gameObject);
    }
}
