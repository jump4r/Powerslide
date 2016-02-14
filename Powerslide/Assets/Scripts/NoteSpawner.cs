using UnityEngine;
using System.Collections;

public enum NoteType
{
    Regular = 0,
    Flick = 1
}

public class NoteSpawner : MonoBehaviour {

    private float xRotation; // This isn't hard coded I promise I'll edit when needed.
    private Quaternion baseRotation;
    private Vector3 basePosition;

    private float distanceFromHitboard = 20f; // distances = mpb * beatsFromHitbaord * speedMultiplier

    // Timing Variables
    public float BPM;
    private float spb; // Seconds per beat
    private float velocity; // Note travel velocity, notes move at a rate of 1 (Unity) meter per 1 beat.

    // Falling note prefabs.
    public GameObject Note;
    public GameObject Flick;
    public GameObject BoardObject;

    public NotePath Path;

	// Use this for initialization
	void Start () { 
        // TODO: Given a list of distance, a BPM, and Velocity Mulitplier, prepare to spawn notes.
        // Set up the rotation, Spawn the hitmarkers
        baseRotation = BoardObject.transform.rotation;
        xRotation = 55f * Mathf.PI / 180f;
        basePosition = new Vector3(0f, BoardObject.transform.position.y + distanceFromHitboard * Mathf.Sin(xRotation) + 0.1f, BoardObject.transform.position.z + distanceFromHitboard * Mathf.Cos(xRotation));
        Debug.Log("Spawning Notes at: " + basePosition);
        InvokeRepeating("SpawnNote", 1f, 1f);                                                                          
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void SpawnNote()
    {
        // Randomness test
        Vector3 spawnPosition = new Vector3(NotePath.NotePaths[Random.Range(0, 3)].transform.position.x, basePosition.y, basePosition.z);
        Instantiate(Note, spawnPosition, baseRotation);
    }
}
