using UnityEngine;
using System.Collections;
 
public class Note : NoteBase {

    // When the note is created, fill in the necissary information from NoteSpawner.cs
    public override void Construct(Vector3 spawnPosition, float offset, int notePathID, string NoteName)
    {
        transform.position = spawnPosition;
        startPosition = spawnPosition;

        EndTime = offset;
        this.notePathID = notePathID;
        gameObject.name = NoteName;
        // Debug.Log("Notepath is " + this.notePathID);
        // Debug.Log(gameObject.name + " Spawned at: " + Conductor.songPosition + ", should end by " + EndTime);
    }

	void OnEnable () {
        type = NoteType.Regular;
        StartTime = Conductor.songPosition;
    }


    public override void Tapped(int notePathID)
    {
        if (this.notePathID == notePathID)
        {
            NotePath.NotePaths[notePathID].RemoveActiveNote(this);
            CalculateError();
        }
    }


    public override void ChangeMaterial(Material mat)
    {
        gameObject.GetComponent<Renderer>().material = mat;
    }

    protected override void ResetNote()
    {
        active = true;
        IsTapped = false;
        isReadyToHit = false;

        ChangeMaterial(Def);
    }

}
