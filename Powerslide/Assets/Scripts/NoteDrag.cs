﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class NoteDrag : NoteBase {

    // Definition of a DRAGNOTE: [offset,numSections,startPath,endPath,length]
    // Length is calculated in number of beats (seconds), 

    // Player notes: Drag slider is hit a lot by accident, remove it in 

    // Collisions will probably be handled differently here
    // 1) Check to see whether or not the starting Point is in line with HitBar
    // 2) Start checking after Conductor.songPosition + 8 * playerSpeedMult * spb has been reached. 

    public AudioClip beatTest;

    [HideInInspector]
    public bool Active = false; // Start tracking the movement of the "finger" or the 
    
    private int numSections; // Number of sections in the note
    private float length; // Calculated in beats
    private int startPath;
    private int endPath;

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
        lineRenderer.SetVertexCount(2); // ATM, we only need two points, but we may need n + 1 depending on the number of segments
        // Debug.Log("Drag OnEnable Position: " + transform.position);
      
    }

    public override void ParseDefinition(string def) 
    {
        // Fill definitions.
        string[] splitString = def.Split(',');
        StartTime = float.Parse(splitString[0]);
        numSections = int.Parse(splitString[1]);
        startPath = int.Parse(splitString[2]);
        endPath = int.Parse(splitString[3]);
        length = int.Parse(splitString[4]);

        // This needs to be here for generic purposes
        notePathID = startPath;
        EndSliderTime = StartTime + (8f * spb) + (length + spb);
    }

    //  the starting and ending positions of theslider.
    private void CalculatePositions()
    {
        // Debug.Log("Drag Note Position: " + transform.position);
        beginningPoint = this.transform.position; // We might need a StartPosition here. OR MAYBE THIS IS REDUNDANT
        endingPoint = new Vector3(NotePath.NotePaths[endPath].transform.position.x, beginningPoint.y + (length * playerSpeedMult * Mathf.Sin(xRotation)), beginningPoint.z + (length * playerSpeedMult * Mathf.Cos(xRotation)));
        // Debug.Log("Beginning Point: " + beginningPoint);
        // Debug.Log("Ending Point: " + endingPoint);
        // Debug.Log("Distance between the two points: " + Vector3.Distance(beginningPoint, endingPoint));
        Debug.Log("Length: " + length);
    }

    private void Update()
    {
        float rTP = (Conductor.songPosition - StartTime) / (8f * Conductor.spb);
        transform.position = new Vector3(startPosition.x, startPosition.y - (8f * playerSpeedMult * Mathf.Sin(xRotation) * rTP), startPosition.z - (8f * playerSpeedMult * Mathf.Cos(xRotation) * rTP));

        lineRenderer.SetPosition(0, beginningPoint);
        lineRenderer.SetPosition(1, endingPoint);
        
        CalculatePositions(); // Recalculate the positions of the points

        // Potentially Activate real note
        if (Conductor.songPosition > EndTime && Conductor.songPosition < EndTime + (length * Conductor.spb) && !Active)
        {
            Active = true;
            NotePath.NotePaths[notePathID].AddActiveNote(this);
            GameObject.Find("Player").GetComponent<Player>().SetActiveDragNote(this);
            // AudioSource.PlayClipAtPoint(beatTest, transform.position); // At the current moment, I'm not sure what this is doing, so I'm commenting it out
            
            // Debug.Log("We have activated a Drag Note in lane: " + notePathID);
        }

        // Potentially Activate real note
        if (Conductor.songPosition > EndTime + (length * Conductor.spb) && Active)
        {
            Active = false;
            NotePath.NotePaths[notePathID].RemoveActiveNote(this);
            GameObject.Find("Player").GetComponent<Player>().SetActiveDragNote(this);
            // Debug.Log("We have deactivated a Drag Note in lane: " + notePathID);
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

    public void OnPath(Transform sliderPosition)
    {
        float xRelPos = GetXRelPos();
        //Debug.Log("Slider X Position: " + sliderPosition.position.x + ", Relative xPos of Drag Note: " + xRelPos);
        
        if (Mathf.Abs(xRelPos - sliderPosition.position.x) < 1.14f / 2f)
        {
            lineRenderer.material = Score100;
        }

        else
        {
            lineRenderer.material = Score50;
        }
    }
}
