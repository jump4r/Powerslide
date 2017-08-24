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

    public int startPath;
    // public int endPath; // Already declared in NoteBase, even though it should probably be declared here.

    private NoteBase nextNote; 

	// Use this for initialization
	void OnEnable () {
        // TODO: Not really sure yet, but make this work with object pooling
        type = NoteType.Flick;
        leftFlick = Resources.Load("Materials/LeftFlick") as Material;
        rightFlick = Resources.Load("Materials/RightFlick") as Material;
	}

    // Definition of a flick note: [offset,startpath,endpath,direction]
    public override void ParseDefinition(string def)
    {
        string[] splitString = def.Split(',');

        // Set the variables from the definition
        StartTime = float.Parse(splitString[0]);
        startPath = int.Parse(splitString[1]);
        endPath = int.Parse(splitString[2]);
        direction = splitString[3];
    }

    public override void Construct(int NotePathID, string NoteName, string direction)
    {
        NotePathID = startPath; // redundant
        gameObject.name = NoteName;
        SetFlickMaterial(); // last var not neded
    }

    // Sets the material on the Flick Note.
    public void SetFlickMaterial()
    {
        if (direction == "r")
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


}
