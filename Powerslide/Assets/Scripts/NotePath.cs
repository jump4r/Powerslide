using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NotePath : MonoBehaviour {

    public int NotePathID; // ID of the
    public GameObject[] pathObjects;

    public static List<NotePath> NotePaths = new List<NotePath>();
    public List<NoteBase> ActiveNotes = new List<NoteBase>();

    

	// Use this for initialization
	void Start () {
        // Add the path to a static list, and then sort it
        NotePaths.Add(this);
        NotePaths.Sort((x, y) => x.NotePathID.CompareTo(y.NotePathID));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AddActiveNote(NoteBase n)
    {
        Debug.Log("Adding " + n.name + " from the ActiveNotes list");
        ActiveNotes.Add(n);
    }

    public void RemoveActiveNote(NoteBase n) {
        Debug.Log("Removing " + n.name + " from the ActiveNotes list");
        ActiveNotes.Remove(n);
    }

    // See if there is an active note that we cna hit
    // Returns based on the need for a drag, or just the need for a hit
    // TODO: In the case of a Hold -> Flick, the flick note will activate, before the hold note deactivates.  This will also be true in high BPM triples, so I must account for this.
    public NoteType CheckIfValidHit()
    {
        if (ActiveNotes.Count <= 0) // No active notes
        {
            return NoteType.NULL;
        }

        else
        {
            return ActiveNotes[0].type; // This is so much simpler
        }
    }


}
