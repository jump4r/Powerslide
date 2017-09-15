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

    // Curved Slider Variables
    private const int numSegments = 16; // The number of segments we want to represent our "curve"
    private float totalHeight;
    private float segmentHeight;
    private float activeSegment;
    private float segmentEndTime = 0f;
    private float segmentStartTime = 0f;
    private float curveStartPos;
    private float curveEndPos;

    private NoteDragType dragType;
    private List<Vector3> segments;

    public AudioClip beatTest;

    [HideInInspector]
    public bool Active = false; // Start tracking the movement of the "finger" or the 
    
    private int numSections; // Number of sections in the note
    private float length; // Calculated in beats

    public float EndSliderTime;

    // Slider Path Calculation
    private Vector3 beginningPoint;
    private Vector3 endingPoint;

    // Line renderer
    private LineRenderer lineRenderer;

    void OnEnable()
    {
        type = NoteType.Drag;
        beatTest = Resources.Load("Sound FX/normal-hitwhistle.wav") as AudioClip;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; // ATM, we only need two points, but we may need n + 1 depending on the number of segments
        // Debug.Log("Drag OnEnable Position: " + transform.position);
      
    }

    public override void ParseDefinition(string def) 
    {
        // Fill definitions.
        string[] splitString = def.Split(',');
        EndTime = float.Parse(splitString[0]); // offset
        numSections = int.Parse(splitString[1]);
        startPath = int.Parse(splitString[2]);
        endPath = int.Parse(splitString[3]);
        length = int.Parse(splitString[4]);

        // This needs to be here for generic purposes
        notePathID = startPath;
        EndSliderTime = EndTime + (length + spb);
    }

    public override void Construct(float offset, int startPath, int endPath, float length, NoteDragType dragType, string NoteName)
    {
        EndTime = offset;
        this.notePathID = startPath;
        this.endPath = endPath;
        this.length = length;
        this.dragType = dragType;
        name = NoteName;

        if (dragType == NoteDragType.Curve)
        {
            segments = new List<Vector3>(numSegments);
            for (int i = 0; i < numSegments; i++)
            {
                segments.Add(Vector3.zero);
            }
            Debug.Log("Amount of segments " + segments.Count);
            lineRenderer.positionCount = numSegments;
            segmentStartTime = EndTime;
            segmentEndTime = EndTime + ((1 / numSegments - 1) * length * Conductor.spb); // End Time of the first segment in the curved slider
            curveStartPos = NotePath.NotePaths[startPath].transform.position.x;
            curveEndPos = NotePath.NotePaths[endPath].transform.position.x;
            totalHeight = Mathf.Pow(numSegments, 2);
        }
    }

    //  the starting and ending positions of theslider.
    private void CalculatePositions()
    {   
        if (dragType == NoteDragType.Linear)
        {
            beginningPoint = this.transform.position; // We might need a StartPosition here. OR MAYBE THIS IS REDUNDANT
            endingPoint = new Vector3(NotePath.NotePaths[endPath].transform.position.x, beginningPoint.y + (length * playerSpeedMult * Mathf.Sin(xRotation)), beginningPoint.z + (length * playerSpeedMult * Mathf.Cos(xRotation)));
        }

        else if (dragType == NoteDragType.Curve)
        {
            float xOffset = (curveEndPos - curveStartPos) / (numSegments - 1); // How far apart the curve segement points will be placed.
            for (int i = 0; i < segments.Count; i++)
            {
                segments[i] = new Vector3(transform.position.x + (xOffset * i), transform.position.y + ((Mathf.Pow(i, 2) / totalHeight) * length * playerSpeedMult * Mathf.Sin(xRotation)), transform.position.z + ((Mathf.Pow(i, 2) / totalHeight) * length * playerSpeedMult * Mathf.Cos(xRotation)));
            }
        }
    }

    private void Update()
    {
        CheckToRemoveFromActiveNotesList();

        float rTP = 1f - (EndTime - Conductor.songPosition) / (NoteHelper.Whole * Conductor.spb);
        transform.position = new Vector3(startPosition.x, startPosition.y - (NoteHelper.Whole * playerSpeedMult * Mathf.Sin(xRotation) * rTP), startPosition.z - (NoteHelper.Whole * playerSpeedMult * Mathf.Cos(xRotation) * rTP));

        if (dragType == NoteDragType.Linear)
        {
            lineRenderer.SetPosition(0, beginningPoint);
            lineRenderer.SetPosition(1, endingPoint);
        }

        if (dragType == NoteDragType.Curve)
        {
            for (int i = 0; i < segments.Count; i++)
            {
                lineRenderer.SetPosition(i, segments[i]);
            }
        }
        
        CalculatePositions(); // Recalculate the positions of the points

        // Potentially Activate real note
        if (Conductor.songPosition > EndTime && Conductor.songPosition < EndTime + (length * Conductor.spb) && !Active)
        {
            Active = true;
            NotePath.NotePaths[notePathID].AddActiveNote(this);
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().SetActiveDragNote(this);
        }

        // Potentially Activate real note
        if (Conductor.songPosition > EndTime + (length * Conductor.spb) && Active)
        {
            Active = false;
            NotePath.NotePaths[notePathID].RemoveActiveNote(this);
        }
    }

    // Determine the x position of point on the hitbar
    public float GetXRelPos()
    {
        float tRatio = (Conductor.songPosition - EndTime) / (length * Conductor.spb);
        // Debug.Log("Don't be infinity: " + tRatio);
        float x0 = NotePath.NotePaths[startPath].transform.position.x;
        float x1 = NotePath.NotePaths[endPath].transform.position.x;
        float xRelPos = x0 + (x1 - x0) * tRatio;
        return xRelPos;
    }

    // Determines the x position of point on a curved slider
    public float GetXCurveRelPos()
    {
        activeSegment = 0f;
        float xOffset = (curveEndPos - curveStartPos) / (numSegments - 1); // How far apart the curve segement points will be placed.
        if (segmentEndTime <= Conductor.songPosition && activeSegment < (numSegments - 1))
        {
            Debug.Log("Change Segment");
            segmentStartTime = segmentEndTime;
            segmentEndTime += (1 / numSegments) * length * Conductor.spb;
            activeSegment++;                
        }
        float x0 = curveStartPos + (xOffset * activeSegment);
        float x1 = curveStartPos + (xOffset * (activeSegment + 1));
        float tRatio = (Conductor.songPosition - segmentStartTime) / (length * Conductor.spb);
        tRatio = Mathf.Pow(tRatio, 2f);
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
    }

    private void CheckToRemoveFromActiveNotesList()
    {
        if (Conductor.songPosition >= EndTime + (Conductor.spb * length))
        {
            // Update the objects in the NotePath's Active Notes List.
            NotePath.NotePaths[notePathID].RemoveActiveNote(this);
            Destroy(gameObject);
        }
    }
}
