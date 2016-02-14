using UnityEngine;
using System.Collections;

public class Note : NoteBase {

    // When the note is created, fill in the necissary information from NoteSpawner.cs
    public void Construct(int notePathID)
    {
        this.notePathID = notePathID;
    }

	void OnEnable () {
        type = NoteType.Regular;
	}

    
    public override void ChangeMaterial()
    {
        if (noteValue > 50)
        {
            gameObject.GetComponent<Renderer>().material = Score100;
        }

       else if (noteValue > 0)
        {
            gameObject.GetComponent<Renderer>().material = Score50;
        }
    }
}
