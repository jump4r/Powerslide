using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Object Pool for Note Objects
public class NoteManager : MonoBehaviour {

    // Falling note prefabs.
    public static GameObject Note;
    public static GameObject Flick;
    public static GameObject Drag;
    public static GameObject Hold;

    // Note Object Pool
    public List<Note> NoteList;
    public List<NoteHold> HoldList;
    public List<NoteDrag> DragList;
    public List<NoteFlick> FlickList;

    // Use this for initialization
    void Start()
    {
        // Set the notes
        Note = Resources.Load("Prefabs/Note") as GameObject;
        Flick = Resources.Load("Prefabs/Flick") as GameObject;
        Drag = Resources.Load("Prefabs/Drag") as GameObject;
        Hold = Resources.Load("Prefabs/Hold") as GameObject;

        NoteList = new List<Note>();
        HoldList = new List<NoteHold>();
        DragList = new List<NoteDrag>();
        FlickList = new List<NoteFlick>();
    }

    private void PreloadNotes(GameObject Note, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject temp = Instantiate(Note);
            temp.SetActive(false);
        }
    }
}
