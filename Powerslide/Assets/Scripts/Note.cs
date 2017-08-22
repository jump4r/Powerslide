using UnityEngine;
using System.Collections;
 
public class Note : NoteBase {

    // When the note is created, fill in the necissary information from NoteSpawner.cs
    public override void Construct(int notePathID, string NoteName)
    {
        this.notePathID = notePathID;
        gameObject.name = NoteName;
        // Debug.Log("Notepath is " + this.notePathID);
    }

	void OnEnable () {
        type = NoteType.Regular;
	}

    
    public override void ChangeMaterial()
    {
        if (noteValue > 50)
        {
            Debug.Log("Changing Material to: GREEN");
            gameObject.GetComponent<Renderer>().material = Score100;
        }

       else if (noteValue > 0)
        {
            Debug.Log("Changing Material to: RED");
            gameObject.GetComponent<Renderer>().material = Score50;
        }
    }

    public override void ParseDefinition(string def)
    {
        base.ParseDefinition(def);
    }
}
