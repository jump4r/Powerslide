using UnityEngine;
using System.Collections;

public enum FlickType
{
    RIGHT = 0,
    LEFT = 1
};

public class NoteFlick : NoteBase {

    public bool direction; // True - right, False = Left, could easily be done with a boolean but better for readibility
    private Material leftFlick;
    private Material rightFlick;

    // Getters/Setters
    public override void SetFingerId(int id) { fingerId = id; }

	// Use this for initialization
	void OnEnable () {
        // TODO: Not really sure yet, but make this work with object pooling
        type = NoteType.Flick;
        leftFlick = Resources.Load("Materials/LeftFlick") as Material;
        rightFlick = Resources.Load("Materials/RightFlick") as Material;
	}

    public override void Construct(Vector3 spawnPosition, float offset, int startPath, int endPath, bool direction, string NoteName)
    {
        EndTime = offset;
        notePathID = startPath; // redundant

        transform.position = spawnPosition;
        startPosition = spawnPosition;

        this.startPath = startPath;
        this.endPath = endPath;
        gameObject.name = NoteName;
        SetFlickMaterial(direction); // last var not neded
    }

    // Sets the material on the Flick Note.
    public void SetFlickMaterial(bool dir)
    {
        gameObject.GetComponent<Renderer>().material = dir ? rightFlick : leftFlick;
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
