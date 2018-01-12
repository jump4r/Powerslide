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
    private Vector3[] bezierPoints; // Curves will be bezier, each containing 3 points.
    private float totalHeight;

    private float dragStartPos;
    private float dragEndPos;

    private NoteDragType dragType;
    private List<Vector3> segments;

    private float length; // Calculated in beats

    public float EndSliderTime;
    private Transform sliderTrasform;

    private bool dragNoteActive = false; // A little confusing, because we have a 'active' bool in the NoteBase, should be changed to 
    private bool isOnPath = false;

    // Line renderer
    private LineRenderer lineRenderer;
    private Vector3 hitbarPosition;

    void OnEnable()
    {
        type = NoteType.Drag;
        DragNoteDebugger = GameObject.Find("Drag Note Debugger");
        sliderTrasform = GameObject.Find("Slider").transform;
        hitbarPosition = GameObject.FindGameObjectWithTag("Hitbar").transform.position;

        // this.gameObject.AddComponent<LineRenderer>();
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.numPositions = 16;

        for (int i = 0; i < lineRenderer.numPositions; i++)
        {
            lineRenderer.SetPosition(i, Vector3.zero);
        }

        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.sortingOrder = -1;

        bezierPoints = new Vector3[3]; 
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
            lineRenderer.numPositions = 2;
        }

        else
        {
            lineRenderer.numPositions = 16;
        }

        // Load the segment points.
        segments = new List<Vector3>(lineRenderer.numPositions);

        for (int i = 0; i < lineRenderer.numPositions; i++)
        {
            segments.Add(Vector3.zero);
        }

        dragStartPos = NotePath.NotePaths[startPath].transform.position.x;
        dragEndPos = NotePath.NotePaths[endPath].transform.position.x;
        totalHeight = Mathf.Pow(lineRenderer.numPositions - 1, 2);
    }

    private void Update()
    {
        CheckToRemoveFromActiveNotesList();

        rTP = 1f - (EndTime - Conductor.songPosition) / (NoteHelper.Whole * Conductor.spb);
        transform.position = new Vector3(startPosition.x, 
                                         startPosition.y - (NoteHelper.Whole * playerSpeedMult * Mathf.Sin(xRotation) * rTP),
                                         startPosition.z - (NoteHelper.Whole * playerSpeedMult * Mathf.Cos(xRotation) * rTP));

        CalculatePositions(); // Recalculate the positions of the points

        for (int i = 0; i < lineRenderer.numPositions; i++)
        {
            try
            {
                lineRenderer.SetPosition(i, segments[i]);
            }

            catch
            {
                Debug.Log("Unable To set position");
            }
        }

        // Potentially Activate real note
        // This looks kinda ugly tbh
        if (Conductor.songPosition > EndTime && Conductor.songPosition < EndTime + (length * Conductor.spb) && !dragNoteActive)
        {
            dragNoteActive = true;
            NotePath.NotePaths[notePathID].AddActiveNote(this);
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().SetActiveDragNote(this);
        }

        // Potentially Deactivate real note
        if (Conductor.songPosition > EndTime + (length * Conductor.spb) && dragNoteActive)
        {
            dragNoteActive = false;
            NotePath.NotePaths[notePathID].RemoveActiveNote(this);
        }
    }

    //  the starting and ending positions of theslider.
    private void CalculatePositions()
    {

        float xOffset = (dragEndPos - dragStartPos) / (lineRenderer.numPositions - 1); // How far apart the curve segement points will be placed.

        if (dragNoteActive)
        {
            float xRelPos = GetXRelPos();
            float currentXDifference = Mathf.Abs(dragEndPos - xRelPos);
            for (int i = 0; i < segments.Count; i++)
            {
                float segmentXDifference = Mathf.Abs(dragEndPos - segments[i].x);

                // Note: In the case of a Linear, Straight slider, this will remove the entire line because the following iff statement is "True"
                // Solution: No matter what, have the last position in the segements array coorespond to the correct end position

                if (segmentXDifference >= currentXDifference &&  i != segments.Count - 1)
                {
                    segments[i] = new Vector3(xRelPos, hitbarPosition.y, hitbarPosition.z);
                }

                else
                {
                    float curveHeight = CurveExponential(i);
                    float newY = transform.position.y + ((curveHeight / totalHeight) * length * playerSpeedMult * Mathf.Sin(xRotation));
                    float newZ = transform.position.z + ((curveHeight / totalHeight) * length * playerSpeedMult * Mathf.Cos(xRotation));
                    segments[i] = new Vector3(transform.position.x + (xOffset * i), newY, newZ);
                }
            }
        }

        else
        {
            for (int i = 0; i < segments.Count; i++)
            {
                float curveHeight = CurveExponential(i);
                float newY = transform.position.y + ((curveHeight / totalHeight) * length * playerSpeedMult * Mathf.Sin(xRotation));
                float newZ = transform.position.z + ((curveHeight / totalHeight) * length * playerSpeedMult * Mathf.Cos(xRotation));

                segments[i] = new Vector3(transform.position.x + (xOffset * i), newY, newZ);
            }
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
            float newIndex = lineRenderer.numPositions - index;
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

    public bool CheckIfOnPath(Vector3 sliderPosition)
    {
        float xRelPos = GetXRelPos();
        //Debug.Log("Slider X Position: " + sliderPosition.position.x + ", Relative xPos of Drag Note: " + xRelPos);

#if UNITY_EDITOR
        DragNoteDebugger.transform.position = new Vector3(xRelPos, DragNoteDebugger.transform.position.y, DragNoteDebugger.transform.position.z);
#endif

        // I believe 1.14  = Width of one NotePath * 2
        if (Mathf.Abs(xRelPos - sliderPosition.x) < 1.14f / 2f) // WHAT IS THIS FLOAT LMAO
        {
            lineRenderer.material = Score100;
            sm.UpdateScore(Mathf.RoundToInt(HIT_PERFECT * Conductor.spb * Time.deltaTime));
            isOnPath = true;
            return true;
        }

        else
        {
            lineRenderer.material = Score50;
            isOnPath = false;
            return false;
        }
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
