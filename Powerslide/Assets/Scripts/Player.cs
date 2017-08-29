using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]

public class Player : MonoBehaviour {

    // Behaviours of Touch Objects
    public Dictionary<int, Finger> FingerDictionary; // Touch t, int fingerID
    public List<int> fingerIDList;
    public List<Finger> FingerList;
    private bool hitNotePathWithFinger = false;

    public LayerMask mask; // We only want to detect certain collisions
    public int layermask = 1 << 8; // Layermask is broken as fuck.

    private bool sliderEnabled = false; // Dragging the slider.
    private static int fingersTouching = 0; // Number of fingers touching the screen

    private NoteType hitNoteType;
    private AudioClip hitSound;

    // Dragging Variables
    // Stretch Goal: See if we can get it so that we can have two flicks at the same time
    private bool dragNoteEnabled = false;
    private NoteDrag activeNoteDrag;

    // Flick variables
    private bool flickNoteEnabled = false;
    private NotePath endFlickPath;
    private NoteFlick activeNoteFlick;

    // Hold Note
    private bool holdNoteEnabled = false;
    private NoteHold activeNoteHold;

    // I don't really know what these are for tbh
    private Vector3 offset = Vector3.zero;
    private float distanceFromRayOrigin = 0;

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
    public void SetHoldNoteEnabled(bool enabled) { holdNoteEnabled = enabled; }
	    
	// Update is called once per frame
	void Update () {

        // BUTTON DOWN ACTION
        ///////////////////////////////////////////////////////////////////
        //For use in Android development 
        // Search for the touch to be added
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                Finger finger = new Finger(Input.GetTouch(i), Input.GetTouch(i).fingerId);

                //////////////////////////////////////////////////////////////////////
                // Player clicks...
                // Determine which Notepath was hit.
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                RaycastHit[] hitObjects = Physics.RaycastAll(ray, 1000f, layermask);
                NotePath hitPath;

                for (int j = 0; j < hitObjects.Length; i++)
                {
                    // If we hit a Notepath...
                    if (hitObjects[j].collider.tag == "NotePath")
                    {
                        hitPath = hitObjects[j].collider.gameObject.GetComponent<NotePath>();
                        // Debug.Log("Hit Object: " + hitPath.name);
                        hitNoteType = hitPath.CheckIfValidHit();

                        if (hitNoteType != NoteType.NULL)
                        {
                            // Add a finger to the list
                            finger.ActiveNote = hitPath.ActiveNotes[0];
                        }

                        if (hitNoteType == NoteType.Regular)
                        {
                            Debug.Log("ANDROID DEBUG: Hit a regular note, we do everything we need to here.");
                            hitPath.ActiveNotes[0].CalculateError();
                            hitPath.ActiveNotes.Remove(hitPath.ActiveNotes[0]);
                        }

                        // This seems pointless tbh
                        else if (hitNoteType == NoteType.Drag)
                        {
                            Debug.Log("Activate Drag Note");
                            dragNoteEnabled = true;
                            activeNoteDrag = (NoteDrag)hitPath.ActiveNotes[0];
                        }

                        // If we hit a hold note (not a transition hold note), we need to see how far the player was from a perfect hit.
                        else if (hitNoteType == NoteType.Hold)
                        {
                            holdNoteEnabled = true;
                            activeNoteHold = (NoteHold)hitPath.ActiveNotes[0];
                            activeNoteHold.CalculateHoldStartError();
                        }

                        // We hit the NotePath with our finger, but we only want to play the notepath once, so I'll call it at the end of update
                        // For now, we'll set a flag.
                        hitNotePathWithFinger = true;
                        break;
                    }

                    // Else, if we hit the Slider Bar, the player will begin to slide a note.
                    // Create a finger object, set the isSliderMoving flag to true, and begin to move the slider.
                    else if (hitObjects[j].collider.tag == "SliderBar")
                    {
                        
                        // First, center the slider to the mouse position, and set the offset
                        Slider.transform.position = new Vector3(hitObjects[j].point.x, Slider.transform.position.y, Slider.transform.position.z);
                        offset = Slider.transform.position - hitObjects[j].point;
                        distanceFromRayOrigin = (ray.origin - hitObjects[j].point).magnitude;
                        Debug.Log("ANDROID DEBUG: We have hit the sliderbar");
                        finger.isMovingSlider = true;
                        break; // IRONICALLY, REMOVING THIS BREAK WILL BREAK EVERYTHING AND I DON'T KNOW WHY.
                    }
                }
                // Make the necissary edits to the finger, then add it to the list
                FingerDictionary.Add(Input.GetTouch(i).fingerId, finger);
                Debug.Log("ANDROID DEBUG: ADDED FingerID" + Input.GetTouch(i).fingerId.ToString() + ", total number of fingers: " + FingerDictionary.Count);
            }
        }
        PlayHitSound();

        // For android, loop through the current list of Touches, and calculate for each one.
        // If the player has a finger currently touching the screen.
        // Used so that the player doesn't have to lift a finger every hold/flick note
        if (Input.GetButton("Touch"))
        {
            RaycastHit[] hitObjects = GetHitObjects();
            NotePath hitPath;
            for (int i = 0; i < hitObjects.Length; i++)
            {
                if (hitObjects[i].collider.tag == "NotePath")
                {
                    hitPath = hitObjects[i].collider.gameObject.GetComponent<NotePath>();
                    hitNoteType = hitPath.CheckIfValidHit();

                    if (hitNoteType == NoteType.Flick)
                    {
                        if (!flickNoteEnabled)
                        {
                            endFlickPath = NotePath.NotePaths[hitPath.ActiveNotes[0].endPath];
                            activeNoteFlick = (NoteFlick)hitPath.ActiveNotes[0];
                        }
                        flickNoteEnabled = true;
                    }

                    else if (hitNoteType == NoteType.Hold || hitNoteType == NoteType.Transition)
                    {
                        if (!holdNoteEnabled)
                        {
                            activeNoteHold = (NoteHold)hitPath.ActiveNotes[0];
                            activeNoteHold.IsBeingHeld();
                        }
                        holdNoteEnabled = true;
                    }
                }
            }
        }

        // IMPORTANT, THIS WILL HAVE TO CHANGE IN ORDER TO ALLOW MULTIPLE FINGERS TO BE TOUCHING AT THE SAME TIME. 
        // Remove that finger from the amount of total fingers touching
        // BUTTON UP, THIS CAN PROBABLY BE IN THE SAME LOOP
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Ended)
            {
                FingerDictionary.Remove(Input.GetTouch(i).fingerId);
                Debug.Log("ANDROID DEBUG: REMOVED " + Input.GetTouch(i).fingerId.ToString() + ", total number of fingers: " + FingerDictionary.Count);
            }
            //////////////////////////////////////////////////////////////////////
            // This could also mean that we released a hold note.
            if (activeNoteHold != null)
            {
                activeNoteHold.CalculateHoldEndError();
                activeNoteHold = null;
            }
            sliderEnabled = false;
            flickNoteEnabled = false;
            holdNoteEnabled = false;
        }

        // There might be a better way to do this, but we need to check to see if the touches in the FingersList are finished or not
        // If we are dragging an the slider
        // 
        foreach (KeyValuePair<int, Finger> finger in FingerDictionary)
        {
            if (finger.Value.isMovingSlider)
            {
                Debug.Log("ANDROID DEBUG: Call MoveSlider()");
                MoveSlider(Input.GetTouch(finger.Key).position);
            }
        }

        // --- OR ---
        // Probably not going to use the slider though, so I should start thinking of a workaround
        if (activeNoteDrag != null)
        {
            activeNoteDrag.CheckIfOnPath(Slider.transform);
        }

        // IF Currently is either encountering a HOLD note or a FLICK note, we need to account for where the finger is at the current time
        // For a hold note: We need to check to see if the finger is still on the right path
        // Same for a transition note, however we may change this to check constantly.
        // For a flick note: We need to check to see if the flick is finshed.
        if (holdNoteEnabled || flickNoteEnabled)
        {
            RaycastHit[] hitObjects = GetHitObjects();
            for (int i = 0; i < hitObjects.Length; i++)
            {
                if (hitObjects[i].collider.tag == "NotePath")
                {

                    // In the case where the player's finger slides off of the NotePath, we need to check to see if the HoldNote is finished.
                    if (activeNoteHold != null && hitObjects[i].collider.gameObject.GetComponent<NotePath>().NotePathID != activeNoteHold.notePathID)
                    {
                        activeNoteHold.CalculateHoldEndError();
                        // activeNoteHold.Active = false;
                        holdNoteEnabled = false;
                        activeNoteHold = null;
                    }

                    // In the case when the flick note is finished.
                    else if (activeNoteFlick != null && hitObjects[i].collider.gameObject.GetComponent<NotePath>().NotePathID == endFlickPath.NotePathID)
                    {
                        Debug.Log("Flick has been finished");
                        activeNoteFlick.CalculateError();
                        activeNoteFlick = null;
                        flickNoteEnabled = false;
                    }
                }
            }
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

    private void MoveSlider(Vector3 position)
    {
        // This will be replaced with finger location via finger ID
        // TODO: See if there is an easier way to do this 1-Dimmentional Movement
        Ray ray = Camera.main.ScreenPointToRay(position);
        Vector3 oldPosition = Slider.transform.position;
        Vector3 newXPosition = ray.GetPoint(distanceFromRayOrigin) + offset;
        Slider.transform.position = new Vector3(newXPosition.x, oldPosition.y, oldPosition.z);
    }

    private RaycastHit[] GetHitObjects()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hitObjects = Physics.RaycastAll(ray, 1000f, layermask);
        return hitObjects;
    }
}
