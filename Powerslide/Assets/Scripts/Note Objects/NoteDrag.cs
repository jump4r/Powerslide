using UnityEngine;
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
        EndTime = float.Parse(splitString[0]); // offset
        numSections = int.Parse(splitString[1]);
        startPath = int.Parse(splitString[2]);
        endPath = int.Parse(splitString[3]);
        length = int.Parse(splitString[4]);

        // This needs to be here for generic purposes
        notePathID = startPath;
        EndSliderTime = EndTime + (length + spb);
    }

    public override void Construct(float offset, int startPath, int endPath, float length, string NoteName)
    {
        EndTime = offset;
        this.notePathID = startPath;
        this.endPath = endPath;
        this.length = length;
        name = NoteName;
    }

    //  the starting and ending positions of theslider.
    private void CalculatePositions()
    {
        // Debug.Log("Drag Note Position: " + transform.position);
        beginningPoint = this.transform.position; // We might need a StartPosition here. OR MAYBE THIS IS REDUNDANT
        endingPoint = new Vector3(NotePath.NotePaths[endPath].transform.position.x, beginningPoint.y + (length * playerSpeedMult * Mathf.Sin(xRotation)), beginningPoint.z + (length * playerSpeedMult * Mathf.Cos(xRotation)));
    }

    private void Update()
    {
        float rTP = 1f - (EndTime - Conductor.songPosition) / (NoteHelper.Whole * Conductor.spb);
        transform.position = new Vector3(startPosition.x, startPosition.y - (NoteHelper.Whole * playerSpeedMult * Mathf.Sin(xRotation) * rTP), startPosition.z - (NoteHelper.Whole * playerSpeedMult * Mathf.Cos(xRotation) * rTP));

        lineRenderer.SetPosition(0, beginningPoint);
        lineRenderer.SetPosition(1, endingPoint);
        
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

    public void CheckIfOnPath(Transform sliderPosition)
    {
        float xRelPos = GetXRelPos();
        //Debug.Log("Slider X Position: " + sliderPosition.position.x + ", Relative xPos of Drag Note: " + xRelPos);
        
        // I believe 1.14  = Width of one NotePath * 2
        if (Mathf.Abs(xRelPos - sliderPosition.position.x) < 1.14f / 2f) // WHAT IS THIS FLOAT LMAO
        {
            lineRenderer.material = Score100;
            gm.UpdateScore(Mathf.RoundToInt(HIT_PERFECT * Conductor.spb * Time.deltaTime));
        }

        else
        {
            lineRenderer.material = Score50;
        }
    }
}
