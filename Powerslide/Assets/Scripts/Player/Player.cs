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

    private AudioClip hitSound;

    // Dragging Variables
    // Stretch Goal: See if we can get it so that we can have two flicks at the same time
    private bool dragNoteEnabled = false;
    private NoteDrag activeNoteDrag;

    // I don't really know what these are for tbh
    public Vector3 offset = Vector3.zero;
    public float distanceFromRayOrigin = 0;

    public GameObject Slider;
	// Use this for initialization
	void Start () {
        hitSound = Resources.Load("Sound FX/hitsound.wav") as AudioClip;
        FingerDictionary = new Dictionary<int, Finger>();
        fingerIDList = new List<int>();
        FingerList = new List<Finger>();
	}

    // Getters and Setters
    public void SetActiveDragNote(NoteDrag drag) { activeNoteDrag = drag;  }
	    
	// Update is called once per frame
	void Update ()
    {
        //For use in Android development 
        // Search for the touch to be added
        for (int i = 0; i < Input.touchCount; i++)
        {
            // BUTTON DOWN ACTION
            ///////////////////////////////////////////////////////////////////
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                Finger finger = new Finger(Input.GetTouch(i), Input.GetTouch(i).fingerId);

                // Make the necissary edits to the finger, then add it to the list
                FingerDictionary.Add(Input.GetTouch(i).fingerId, finger);
            }


            // BUTTON UP, THIS CAN PROBABLY BE IN THE SAME LOOP
            if (Input.GetTouch(i).phase == TouchPhase.Ended)
            {
                FingerDictionary[Input.GetTouch(i).fingerId].LiftFinger();
                FingerDictionary.Remove(Input.GetTouch(i).fingerId);
            }
        }

        PlayHitSound();

        // --- OR ---
        // If there there is a drag note, we need to check to see if the slider is close enough to the drag note.
        // This doesn't really belong here
        CheckDragNote();

        // Things to do in the FingerDictionary loop
        /* 
         * If a finger is currently moving the slider, we need to check to see if the slider is close enougfh to the drag note.
         * IF Currently is either encountering a HOLD note or a FLICK note, we need to account for where the finger is at the current time
         * For a hold note: We need to check to see if the finger is still on the right path
         * Same for a transition note, however we may change this to check constantly.
         * Lastly, for a flick note: We need to check to see if the flick is finshed.
         * 
        */

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
            GetComponent<AudioSource>().Play();
            hitNotePathWithFinger = false;
        }
    }


    private void CheckDragNote()
    {
        if (activeNoteDrag != null)
        {
            activeNoteDrag.CheckIfOnPath(Slider.transform);
        }
    }
}
