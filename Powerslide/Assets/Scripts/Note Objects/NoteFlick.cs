using UnityEngine;
using System.Collections;

public enum FlickType
{
    RIGHT = 0,
    LEFT = 1
};

public class NoteFlick : NoteBase {

    public string direction; // l - left, r - right, could easily be done with a boolean but better for readibility
    private Material leftFlick;
    private Material rightFlick;

    // public int endPath; // Already declared in NoteBase, even though it should probably be declared here.

    private NoteBase nextNote; 

    // Getters/Setters
    public override void SetFingerId(int id) { fingerId = id; }

	// Use this for initialization
	void OnEnable () {
        // TODO: Not really sure yet, but make this work with object pooling
        type = NoteType.Flick;
        leftFlick = Resources.Load("Materials/LeftFlick") as Material;
        rightFlick = Resources.Load("Materials/RightFlick") as Material;
	}

    public override void Construct(float offset, int startPath, int endPath, string direction, string NoteName)
    {
        EndTime = offset;
        notePathID = startPath; // redundant
        this.startPath = startPath;
        this.endPath = endPath;
        gameObject.name = NoteName;
        SetFlickMaterial(direction); // last var not neded
    }

    // Sets the material on the Flick Note.
    public void SetFlickMaterial(string dir)
    {
        if (dir == "r")
        {
            gameObject.GetComponent<Renderer>().material = rightFlick;
        }

        else
        {
            gameObject.GetComponent<Renderer>().material = leftFlick;
        }
    }

    // Changes the material of the note depending on the 
    public override void ChangeMaterial(Material mat)
    {
        gameObject.GetComponent<Renderer>().material = mat;
    }

    public override void Transitioned(int startPathID, int endPathID)
    {
        if (this.startPath == startPathID && this.endPath == endPathID)
        {
            CalculateError();
        }
    }
}
