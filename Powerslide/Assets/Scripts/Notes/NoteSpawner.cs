﻿using UnityEngine; 
using System.Collections;

public class NoteSpawner : MonoBehaviour {

    private float xRotation; // This isn't hard coded I promise I'll edit when needed.
    private static Quaternion baseRotation;
    private static Vector3 basePosition;

    private float distanceFromHitboard; // distances = mpb * beatsFromHitbaord * speedMultiplier

    // Timing Variables
    public float BPM;

    // Falling note prefabs.
    public static GameObject Note;
    public static GameObject Flick;
    public static GameObject Drag;
    public static GameObject Hold;
    public static GameObject Transition;
    public GameObject BoardObject;

    public NotePath Path;

    private static string noteID;
    private static int noteIndex = 0;

	// Use this for initialization
	void Start () {
        // TODO: Given a list of distance, a BPM, and Velocity Mulitplier, prepare to spawn notes.
        // Set up the rotation, Spawn the hitmarkers
        distanceFromHitboard = NoteHelper.Whole * Settings.PlayerSpeedMult; // how far away we are going to spawn the notes.
        baseRotation = BoardObject.transform.rotation;
        xRotation = 55f * Mathf.PI / 180f;
        basePosition = new Vector3(0f, BoardObject.transform.position.y + distanceFromHitboard * Mathf.Sin(xRotation) + 0.1f, BoardObject.transform.position.z + distanceFromHitboard * Mathf.Cos(xRotation));
        // Debug.Log("Spawning Notes at: " + basePosition);
       
        // InvokeRepeating("SpawnNote", 1f, 1f);

        // Set the notes
        Note = Resources.Load("Prefabs/Note") as GameObject;
        Flick = Resources.Load("Prefabs/Flick") as GameObject;
        Drag = Resources.Load("Prefabs/Drag") as GameObject;
        Hold = Resources.Load("Prefabs/Hold") as GameObject;
	}

    public static void SpawnHitObject(string raw_hitObject)
    {
        string[] hitObject = raw_hitObject.Split(',');
        NoteType hitObjectType;
        if (hitObject.Length > 2)
        {
            hitObjectType = (NoteType)int.Parse(hitObject[1]);
            switch (hitObjectType)
            {
                case (NoteType.Regular):
                    SpawnNote(raw_hitObject.Split(','));
                    break;

                case (NoteType.Hold):
                    SpawnHold(raw_hitObject.Split(','));
                    break;

                case (NoteType.Flick):
                    SpawnFlick(raw_hitObject.Split(','));
                    break;

                case (NoteType.Drag):
                    SpawnDrag(raw_hitObject.Split(','));
                    break;

                default:
                    break;
            }
        }
    }

    // Spawns a regular note
    // Definition of a NOTE: [offset, noteType, startPath]
    public static void SpawnNote(string[] noteDef)
    {
        // Change the name of the note for easier debugging.
        noteID = "Note " + noteIndex.ToString();
        noteIndex++;

        // Parse Def
        float offset = float.Parse(noteDef[0]) / 1000f;
        int startPath = int.Parse(noteDef[2]);

        Vector3 spawnPosition = new Vector3(NotePath.NotePaths[startPath].transform.position.x, basePosition.y, basePosition.z);

        Note n = (Note)GetNoteFromPool(NoteType.Regular);
        n.Construct(spawnPosition, offset, startPath, noteID); // ROFL APPARENTLY THIS WORKS. $$$$$$YENYENYENYENWONWONWONWON
        n.gameObject.SetActive(true);

    }

    // Spawns a drag note.
    // Definition of a DRAGNOTE: [offset, noteType, numSections, startPath,endPath,length,DragNoteType] - DEPRECATED
    
    // New Drag Note - Start & End Paths now reprsent different sections on the board.
    public static void SpawnDrag(string[] def)
    {
        // Change the name of the note for easier debugging.
        noteID = "Note " + noteIndex.ToString();
        noteIndex++;

        float offset = float.Parse(def[0]) / 1000f;
        int numSections = int.Parse(def[2]);
        int startPath = int.Parse(def[3]);
        int endPath = int.Parse(def[4]);
        float length = float.Parse(def[5]);
        string dragNoteTypeString = def[6];
        NoteDragType dragNoteType;
        switch(dragNoteTypeString)
        {
            case ("L"):
                dragNoteType = NoteDragType.Linear;
                break;
            case ("C"):
                dragNoteType = NoteDragType.Curve;
                break;
            case ("R"):
                dragNoteType = NoteDragType.Root;
                break;
            default:
                dragNoteType = NoteDragType.Linear;
                break;
        }

        string definition = offset + "," + numSections + "," + startPath + "," + endPath + "," + length;

        Vector3 spawnPosition = new Vector3(NotePath.NotePaths[startPath].transform.position.x, basePosition.y, basePosition.z);
        NoteDrag n = (NoteDrag)GetNoteFromPool(NoteType.Drag);

        NoteDrag temp = Instantiate(Drag, spawnPosition, baseRotation).GetComponent<NoteDrag>();
        temp.Construct(spawnPosition, offset, startPath, endPath, length, dragNoteType, noteID);

    }
    

    // Spawnds a hold note
    // Definition of a HOLDNOTE: [offset, noteType, startPath, length, isTransition]
    // isTransition - bool to decide weather the note is a transition note or not.
    public static void SpawnHold(string[] def)
    {
        // Change the name of the note for easier debugging.
        noteID = "Note " + noteIndex.ToString();
        noteIndex++;

        if (def.Length != 5)
        {
            Debug.Log("Something went wrong spawning the hold note");
            return;
        } 

        float offset = float.Parse(def[0]) / 1000f;
        int path = int.Parse(def[2]);
        float length = float.Parse(def[3]);
        bool isTransition = bool.Parse(def[4]);

        string definition = offset + "," + path.ToString() + "," + length.ToString() + "," + isTransition;

        Vector3 spawnPosition = new Vector3(NotePath.NotePaths[path].transform.position.x, basePosition.y, basePosition.z);
      
        NoteHold n = (NoteHold) GetNoteFromPool(NoteType.Hold);
        n.Construct(spawnPosition, offset, path, length, isTransition, noteID);
        n.gameObject.SetActive(true);
    }

    // Spawns a flick note
    // Definition of a FLICKNOTE: [offset, noteType, startPath,endPath,flickDirection]
    public static void SpawnFlick(string[] def)
    {
        // Change the name of the note for easier debugging.
        noteID = "Note " + noteIndex.ToString();
        noteIndex++;

        if (def.Length != 5)
        {
            Debug.Log("Note type doesn't match the note parameters");
            return;
        }

        float offset = float.Parse(def[0]) / 1000f;
        int startPath = int.Parse(def[2]);
        int endPath = int.Parse(def[3]);
        bool direction = bool.Parse(def[4]);

        string definition = Conductor.songPosition.ToString() + "," + startPath + "," + endPath + "," + direction;

        Vector3 spawnPosition = new Vector3((NotePath.NotePaths[startPath].transform.position.x + NotePath.NotePaths[endPath].transform.position.x) / 2, basePosition.y, basePosition.z);
        NoteFlick n = (NoteFlick)GetNoteFromPool(NoteType.Flick);
        n.Construct(spawnPosition, offset, startPath, endPath, direction, noteID);
        n.gameObject.SetActive(true);
    }

    private static NoteBase GetNoteFromPool(NoteType type)
    {
        NoteBase rtn = null;
        switch (type)
        {
            case (NoteType.Regular):
                foreach (Note n in NoteManager.instance.NoteList)
                {
                    if (!n.gameObject.activeInHierarchy)
                    {
                        rtn = n as NoteBase;
                        break;
                    }
                }
                break;

            case (NoteType.Hold):
                foreach (NoteHold n in NoteManager.instance.HoldList)
                {
                    if (!n.gameObject.activeInHierarchy)
                    {
                        rtn = n as NoteBase;
                        break;
                    }
                }
                break;

            case (NoteType.Flick):
                foreach (NoteFlick n in NoteManager.instance.FlickList)
                {
                    if (!n.gameObject.activeInHierarchy)
                    {
                        rtn = n as NoteBase;
                        break;
                    }
                }
                break;

            case (NoteType.Drag):
                foreach (NoteDrag n in NoteManager.instance.DragList)
                {
                    if (!n.gameObject.activeInHierarchy)
                    {
                        rtn = n as NoteBase;
                        break;
                    }
                }
                break;
        }

        if (rtn == null) { Debug.Log("Something went wrong"); }

        return rtn;
    }

}
