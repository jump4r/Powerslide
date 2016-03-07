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
    private Vector3 startingPoint;
    private Vector3 endingPoint;

    void OnEnable()
    {
        type = NoteType.Drag;
    }

    private void ParseDefinition(string def) 
    {
        // Fill definitions.
        string[] splitString = def.Split(',');
        StartTime = int.Parse(splitString[0]);
        numSections = int.Parse(splitString[1]);
        startPath = int.Parse(splitString[2]);
        endPath = int.Parse(splitString[3]);
        length = int.Parse(splitString[4]);
    }

    //  the starting and ending positions of theslider.
    private void CalculatePositions()
    {
        startingPoint = startPosition;
        endingPoint = new Vector3(NotePath.NotePaths[endPath].transform.position.x, startPosition.y + (length * playerSpeedMult * Mathf.Sin(xRotation)), startPosition.z + (length + playerSpeedMult * Mathf.Cos(xRotation)));
    }

    private void Update()
    {
        float rTP = (Conductor.songPosition - StartTime) / (8f * Conductor.spb);
    }
}
