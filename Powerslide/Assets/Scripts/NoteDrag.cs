using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class NoteDrag : NoteBase {

    // Definition of a DRAGNOTE: [offset,numSections,startPath,endPath,length]
    // Length is calculated in number of beats (seconds), 
    
    // Collisions will probably be handled differently here
    // 1) Check to see whether or not the starting Point is in line with HitBar
    // 2) Start checking after Conductor.songPosition + 8 * playerSpeedMult * spb has been reached. 

    private int numSections; // Number of sections in the note
    private float length; // Calculated in beats
    private int startPath;
    private int endPath;

    // Slider Path Calculation
    private Vector3 beginningPoint;
    private Vector3 endingPoint;

    // Line renderer
    private LineRenderer lineRenderer;

    void OnEnable()
    {
        type = NoteType.Drag;


        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetVertexCount(2); // ATM, we only need two points, but we may need n + 1 depending on the number of segments
        Debug.Log("Drag OnEnable Position: " + transform.position);
    }

    public void ParseDefinition(string def) 
    {
        // Fill definitions.
        string[] splitString = def.Split(',');
        StartTime = float.Parse(splitString[0]);
        numSections = int.Parse(splitString[1]);
        startPath = int.Parse(splitString[2]);
        endPath = int.Parse(splitString[3]);
        length = int.Parse(splitString[4]);
    }

    //  the starting and ending positions of theslider.
    private void CalculatePositions()
    {
        Debug.Log("Drag Note Position: " + transform.position);
        beginningPoint = this.transform.position; // We might need a StartPosition here. OR MAYBE THIS IS REDUNDANT
        endingPoint = new Vector3(NotePath.NotePaths[endPath].transform.position.x, startPosition.y + (length * playerSpeedMult * Mathf.Sin(xRotation)), startPosition.z + (length + playerSpeedMult * Mathf.Cos(xRotation)));
    }

    private void Update()
    {
        float rTP = (Conductor.songPosition - StartTime) / (8f * Conductor.spb);
        transform.position = new Vector3(startPosition.x, startPosition.y - (8f * playerSpeedMult * Mathf.Sin(xRotation) * rTP), startPosition.z - (8f * playerSpeedMult * Mathf.Cos(xRotation) * rTP));

        lineRenderer.SetPosition(0, beginningPoint);
        lineRenderer.SetPosition(1, endingPoint);
        CalculatePositions(); // Recalculate the positions of the points
    }
}
