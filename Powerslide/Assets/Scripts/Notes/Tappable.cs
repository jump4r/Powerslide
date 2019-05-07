using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tappable : MonoBehaviour {

    public Material pathColorDefault;
    public Material pathColorPressed;

    public List<NoteBase> ActiveNotes = new List<NoteBase>();
	
	// Update is called once per frame
	void Update () {
		
	}
    public void AddActiveNote(NoteBase n)
    {
        // Debug.Log("Android Debug: Adding " + n.name + " to the ActiveNotes list");
        ActiveNotes.Add(n);
    }

    public void RemoveActiveNote(NoteBase n)
    {
        // Debug.Log("Android Debug: Removing " + n.name + " from the ActiveNotes list");
        ActiveNotes.Remove(n);
    }

    protected void SetRenderMaterial(Material mat)
    {
        GetComponent<Renderer>().material = mat;
    }
}
