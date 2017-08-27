﻿using UnityEngine; 
using System.Collections;

public class NoteSpawner : MonoBehaviour {

    private float xRotation; // This isn't hard coded I promise I'll edit when needed.
    private static Quaternion baseRotation;
    private static Vector3 basePosition;

    private float distanceFromHitboard; // distances = mpb * beatsFromHitbaord * speedMultiplier
    private float pSM = 2.0f; // Player Speed Mulitplier

    // Timing Variables
    public float BPM;
    private float spb; // Seconds per beat
    private float velocity; // Note travel velocity, notes move at a rate of 1 (Unity) meter per 1 beat.

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
        distanceFromHitboard = 8f * pSM; // Distance from the 
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
        Transition = Resources.Load("Prefabs/Transition") as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    // Spawns a regular note
    // Definition of a NOTE: [offset, noteType, startPath]
    public static void SpawnNote()
    {
        // Change the name of the note for easier debugging.
        noteID = "Note " + noteIndex.ToString();
        noteIndex++;

        // Spawn the note along a random NotePath
        int randomStartNotePath = (int)Random.Range(0, 4);
        Vector3 spawnPosition = new Vector3(NotePath.NotePaths[randomStartNotePath].transform.position.x, basePosition.y, basePosition.z);
        GameObject tmp = Instantiate(Note, spawnPosition, baseRotation) as GameObject;
      
        tmp.GetComponent<NoteBase>().Construct(randomStartNotePath, noteID); // ROFL APPARENTLY THIS WORKS. $$$$$$YENYENYENYENWONWONWONWON

    }

    // Spawns a drag note.
    // Definition of a DRAGNOTE: [offset, noteType, numSections, startPath,endPath,length]
    // Player Notes: Keep randomness note generation, but add more variables to make generation more predicatable for practice.
    public static void SpawnDrag()
    {
        // Debug.Log("Spawning A DRAG: ");
        int randomStartNotePath = (int)Random.Range(0, 4);
        int randomEndNotePath = (int)Random.Range(0, 4);
        string def = Conductor.songPosition.ToString() + "," + 2.ToString() + "," + randomStartNotePath.ToString() + "," + randomEndNotePath.ToString() + "," + 2.ToString();
        
        Vector3 spawnPosition = new Vector3(NotePath.NotePaths[randomStartNotePath].transform.position.x, basePosition.y, basePosition.z);
        // Debug.Log("Drag Spawning Position: " + spawnPosition);
        GameObject tmp = Instantiate(Drag, spawnPosition, baseRotation) as GameObject;
        tmp.GetComponent<NoteDrag>().ParseDefinition(def);
    }

    // Spawnds a hold note
    // Definition of a HOLDNOTE: [offset, startPath, length, isTransition]]
    // isTransition - bool to decide weather the note is a transition note or not.
    public static void SpawnHold(int notePath, string isTransition, int length)
    {
        // Change the name of the note for easier debugging.
        noteID = "Note " + noteIndex.ToString();
        noteIndex++;

        int randomStartNotePath = notePath; //(int)Random.Range(0, 4);
        string def = Conductor.songPosition.ToString() + "," + randomStartNotePath.ToString() + "," + length + "," + isTransition;

        Vector3 spawnPosition = new Vector3(NotePath.NotePaths[randomStartNotePath].transform.position.x, basePosition.y, basePosition.z);
      
        GameObject tmp = Instantiate(Hold, spawnPosition, baseRotation);
        tmp.GetComponent<NoteHold>().ParseDefinition(def);
        tmp.GetComponent<NoteBase>().Construct(randomStartNotePath, noteID);
    }

    // Spawns a flick note
    // Definition of a FLICKNOTE: [offset,startPath,endPath,flickDirection]
    public static void SpawnFlick(int startPath, int endPath, string direction)
    {
        // Change the name of the note for easier debugging.
        noteID = "Note " + noteIndex.ToString();
        noteIndex++;

        string def = Conductor.songPosition.ToString() + "," + startPath + "," + endPath + "," + direction; // startin with a left flick

        Vector3 spawnPosition = new Vector3((NotePath.NotePaths[startPath].transform.position.x + NotePath.NotePaths[endPath].transform.position.x) / 2, basePosition.y, basePosition.z);
        GameObject tmp = Instantiate(Flick, spawnPosition, baseRotation);
        tmp.GetComponent<NoteFlick>().ParseDefinition(def);
        tmp.GetComponent<NoteFlick>().Construct(startPath, noteID, "l");
    }

}
