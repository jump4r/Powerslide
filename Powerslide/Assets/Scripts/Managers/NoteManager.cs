using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Object Pool for Note Objects
public class NoteManager : MonoBehaviour {

    // instance
    public static NoteManager instance = null;

    private Vector3 spawnPosition = new Vector3(-99f, -99f, -99f);
    [SerializeField]
    private Transform BoardObject;

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

    private const int noteAmount = 30;
    private const int dragAmount = 10;

    private 

    // Use this for initialization
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(this.gameObject);
        }

        // Set the notes
        Note = Resources.Load("Prefabs/Note") as GameObject;
        Flick = Resources.Load("Prefabs/Flick") as GameObject;
        Drag = Resources.Load("Prefabs/Drag") as GameObject;
        Hold = Resources.Load("Prefabs/Hold") as GameObject;

        NoteList = new List<Note>();
        HoldList = new List<NoteHold>();
        DragList = new List<NoteDrag>();
        FlickList = new List<NoteFlick>();

        PreloadNotes();
    }

    private void PreloadNotes()
    {
        for (int i = 0; i < noteAmount; i++)
        {
            Note tempNote = Instantiate(Note, spawnPosition, BoardObject.rotation).GetComponent<Note>();
            NoteList.Add(tempNote);
            tempNote.gameObject.SetActive(false);

            NoteFlick tempFlick = Instantiate(Flick, spawnPosition, BoardObject.rotation).GetComponent<NoteFlick>();
            FlickList.Add(tempFlick);
            tempFlick.gameObject.SetActive(false);

            NoteHold tempHold = Instantiate(Hold, spawnPosition, BoardObject.rotation).GetComponent<NoteHold>();
            HoldList.Add(tempHold);
            tempHold.gameObject.SetActive(false);
        }

        for(int i = 0; i < dragAmount; i++)
        {
            NoteDrag tempDrag = Instantiate(Drag, spawnPosition, BoardObject.rotation).GetComponent<NoteDrag>();
            DragList.Add(tempDrag);
            tempDrag.gameObject.SetActive(false);
        }
    }
}
