using UnityEngine;
using System.Collections;
 
public class Note : NoteBase {

    // When the note is created, fill in the necissary information from NoteSpawner.cs
    public override void Construct(float offset, int notePathID, string NoteName)
    {
        EndTime = offset;
        this.notePathID = notePathID;
        gameObject.name = NoteName;
        // Debug.Log("Notepath is " + this.notePathID);
        Debug.Log(gameObject.name + " Spawned at: " + Conductor.songPosition + ", should end by " + EndTime);
    }

	void OnEnable () {
        type = NoteType.Regular;
	}

    
    public override void ChangeMaterial(Material mat)
    {
        gameObject.GetComponent<Renderer>().material = mat;
    }

    public override void ParseDefinition(string def)
    {
        base.ParseDefinition(def);
    }
}
