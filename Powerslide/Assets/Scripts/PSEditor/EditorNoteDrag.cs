using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EditorNoteDrag : EditorNoteBase {

    public float length;

    public int endPath;

    private LineRenderer lineRenderer;
    private int numSegments = 16;
    private List<Vector3> segments;
    private float totalHeight;

    private float dragStartPoint;
    private float dragEndPoint;

    private NoteDragType dragType;

    // Use this for initialization
    void OnEnable () {
        lineRenderer = GetComponent<LineRenderer>();
	}

    public override void Construct(float offset, int notePathId, int endPath, float length, NoteDragType dragType, Vector3 spawnPosition)
    {
        this.offset = offset;
        this.notePathId = notePathId;
        this.endPath = endPath;
        this.length = length;
        this.dragType = dragType;
        this.startPosition = spawnPosition;

        if (this.dragType == NoteDragType.Curve || this.dragType == NoteDragType.Root)
        {
            numSegments = 16;
        }

        else
        {
            numSegments = 2;
        }

        lineRenderer.numPositions = numSegments; // Kinda uneeded imo
        segments = new List<Vector3>(numSegments);
        for (int i = 0; i < numSegments; i++)
        {
            segments.Add(Vector3.zero);
        }

        dragStartPoint = NotePath.NotePaths[notePathId].transform.position.x;
        dragEndPoint = NotePath.NotePaths[endPath].transform.position.x;
        totalHeight = Mathf.Pow(numSegments-1, 2);

        Debug.Log("Drag Note: " + notePathId + ", " + endPath + ", " + length + ", " + lineRenderer.numPositions);

        CalculatePositions();

        for (int i = 0; i < lineRenderer.numPositions; i++)
        {
            lineRenderer.SetPosition(i, segments[i]);
            Debug.Log("Set Position " + i + ": " + segments[i]);
        }
    }

    private void Update()
    {
        rTP = 1f - ((offset - EditorConductor.instance.songPosition) / (EditorConductor.instance.spb * NoteHelper.Whole * 2f));
        transform.position = new Vector3(startPosition.x,
                                         startPosition.y - (NoteHelper.Whole * 2f * Settings.PlayerSpeedMult * Mathf.Sin(xRotation) * rTP),
                                         startPosition.z - (NoteHelper.Whole * 2f * Settings.PlayerSpeedMult * Mathf.Cos(xRotation) * rTP));

        for (int i = 0; i < lineRenderer.numPositions; i++)
        {
            lineRenderer.SetPosition(i, segments[i]);
        }

        CalculatePositions();

        if (rTP > 1f && activateHitSound)
        {
            PlayHitSound();
        }

        else if (rTP < 1f)
        {
            activateHitSound = true;
        }
    }

    public void SetNote(float length, int endPath)
    {
        this.endPath = endPath;
        dragEndPoint = NotePath.NotePaths[endPath].transform.position.x;
        this.length = length;

        CalculatePositions();
    }

    private void CalculatePositions()
    {
        float xOffset = (dragEndPoint - dragStartPoint) / (numSegments - 1); // How far apart the curve segement points will be placed.
        for (int i = 0; i < segments.Count; i++)
        {
            float curveHeight = CurveExponential(i);
            segments[i] = new Vector3(transform.position.x + (xOffset * i),
                                      transform.position.y + ((curveHeight / totalHeight) * length * Settings.PlayerSpeedMult * Mathf.Sin(xRotation)),
                                      transform.position.z + ((curveHeight / totalHeight) * length * Settings.PlayerSpeedMult * Mathf.Cos(xRotation)));
            // Debug.Log("Setting Line Positions: " + segments[i]);
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

    public override string ToString()
    {
        string rtn = (Mathf.RoundToInt(offset * 1000)).ToString() + ",2,2," + notePathId.ToString() + "," + endPath.ToString() + "," + length.ToString()  + ",";
        if (dragType == NoteDragType.Curve)
        {
            rtn += "C";
        }

        else if (dragType == NoteDragType.Root)
        {
            rtn += "R";
        }

        else
        {
            rtn += "L";
        }

        return rtn;
    }

}