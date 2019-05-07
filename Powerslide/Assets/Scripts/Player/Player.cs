using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]

public class Player : MonoBehaviour {

    public static Player instance = null; 

    // Behaviours of Touch Objects
    public Dictionary<int, Finger> FingerDictionary; // Touch t, int fingerID
    public List<int> fingerIDList;
    public List<Finger> FingerList;
    public bool hitNotePathWithFinger = false;

    public int layermask = 1 << 8; // Layermask is broken as fuck.

    private bool sliderEnabled = false; // Dragging the slider.
    public bool isSliderOnPath;

    private AudioClip hitSound;

    // Dragging Variables
    private bool dragNoteEnabled = false;
    public NoteDrag activeNoteDrag;

    // I don't really know what these are for tbh
    public Vector3 offset = Vector3.zero;
    public float distanceFromRayOrigin = 0;

    // Bot
    [SerializeField]
    private bool enableBot = false;

    public GameObject Slider;
	// Use this for initialization
	void Start () {
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(this.gameObject);
        }

        hitSound = Resources.Load("Sound FX/hitsound.wav") as AudioClip;
        FingerDictionary = new Dictionary<int, Finger>();
        fingerIDList = new List<int>();
        FingerList = new List<Finger>();
	}

    // Getters and Setters
    public void SetActiveDragNote(NoteDrag drag)
    {
        activeNoteDrag = drag;
    }
	    
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                Finger finger = new Finger(Input.GetTouch(i), Input.GetTouch(i).fingerId);
                FingerDictionary.Add(Input.GetTouch(i).fingerId, finger);
            }

            if (Input.GetTouch(i).phase == TouchPhase.Ended)
            {
                FingerDictionary[Input.GetTouch(i).fingerId].LiftFinger();
                FingerDictionary.Remove(Input.GetTouch(i).fingerId);
            }
        }

        // Todo, get these two functions out of the Player.cs file
        PlayHitSound();

        // --- OR ---
        // If there there is a drag note, we need to check to see if the slider is close enough to the drag note.
        // This doesn't really belong here
        CheckDragNote();

        foreach (KeyValuePair<int, Finger> finger in FingerDictionary)
        {
            finger.Value.UpdateFinger();
        } 
	}

    // Play a hit sound if we hit a NotePath with our finger.
    public void PlayHitSound()
    {
        if (hitNotePathWithFinger)
        {
            // GetComponent<AudioSource>().Play();
            hitNotePathWithFinger = false;
        }
    }

    // Checks to see if the slider is on the path of the drag note.
    private void CheckDragNote()
    {
        if (activeNoteDrag != null)
        {
            isSliderOnPath = activeNoteDrag.CheckIfOnPath(Slider.transform.position);
        }

        else
        {
            isSliderOnPath = false;
        }
    }
}
