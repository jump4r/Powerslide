using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]

public class Player : MonoBehaviour {

    public Dictionary<int, Touch> fingersList; // Touch t, int fingerID
    public List<int> fingerIDList;

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
        fingersList = new Dictionary<int, Touch>();
        fingerIDList = new List<int>();
	}

    // Getters and Setters
    public void SetActiveDragNote(NoteDrag drag) { activeNoteDrag = drag;  }
    public void SetHoldNoteEnabled(bool enabled) { holdNoteEnabled = enabled; }
	    
	// Update is called once per frame
	void Update () {

        // What to do what they player FIRST touches a note.
        if (Input.GetButtonDown("Touch"))
        {
            ///////////////////////////////////////////////////////////////////
            //For use in Android development 

            Debug.Log("ANDROID DEBUG: ADDED " + Input.touches[0].phase.ToString() + ", total number of fingers: " + Input.touches.Length);
            // Search for the touch to be added
            for (int i = 0; i < Input.touches.Length; i++)
            {
                if (Input.touches[i].phase == TouchPhase.Began)
                {
                    fingersList.Add(Input.touches[i].fingerId, Input.touches[i]);
                    fingerIDList.Add(Input.touches[i].fingerId);
                }
            }

            //////////////////////////////////////////////////////////////////////
            // Player clicks...
            // Determine which Notepath was hit.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hitObjects = Physics.RaycastAll(ray, 1000f, layermask);
            NotePath hitPath;

            for (int i = 0; i < hitObjects.Length; i++)
            {
                // If we hit a Notepath...
                if (hitObjects[i].collider.tag == "NotePath")
                {
                    hitPath = hitObjects[i].collider.gameObject.GetComponent<NotePath>();
                    // Debug.Log("Hit Object: " + hitPath.name);
                    hitNoteType = hitPath.CheckIfValidHit();

                    // This seems pointless tbh
                    if (hitNoteType == NoteType.Drag)
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

                    // Play hitsound after hitting the Notepath
                    PlayHitSound();
                    break;
                }

                // Drag the sliderbar
                else if (hitObjects[i].collider.tag == "SliderBar")
                {
                    Debug.Log("We have hit the sliderbar");
                    // First, center the slider to the mouse position, and set the offset
                    Slider.transform.position = new Vector3(hitObjects[i].point.x, Slider.transform.position.y, Slider.transform.position.z);
                    offset = Slider.transform.position - hitObjects[i].point;
                    distanceFromRayOrigin = (ray.origin - hitObjects[i].point).magnitude;
                    sliderEnabled = true;
                    // Here we will cache the touch ID and send it to the Sliderbar
                }
            }
        }

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
        if (Input.GetButtonUp("Touch"))
        {
            ///////////////////////////////////////////////////////////////////
            //For use in Android development 

            Debug.Log("ANDROID DEBUG: REMOVED"  + Input.touches[0].phase.ToString() + ", total number of fingers: " + Input.touches.Length);
            // Search for the touch to be added
            for (int i = 0; i < Input.touches.Length; i++)
            {
                if (Input.touches[i].phase == TouchPhase.Ended)
                {
                    fingersList.Remove(fingerIDList[i]);
                    fingerIDList.Remove(i);
                }
            }
            //////////////////////////////////////////////////////////////////////
            // This could also mean that we released a hold note.
            if (activeNoteHold != null)
            {
                activeNoteHold.CalculateHoldEndError();
                activeNoteHold = null;
            }

            fingersTouching -= 1;
            sliderEnabled = false;
            flickNoteEnabled = false;
            holdNoteEnabled = false;
        }

        // If we are dragging an the slider
        else if (sliderEnabled)
        {
            MoveSlider();
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

    public void PlayHitSound()
    {
        GetComponent<AudioSource>().Play();
    }

    private void MoveSlider()
    {
        // This will be replaced with finger location via finger ID
        // TODO: See if there is an easier way to do this 1-Dimmentional Movement
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
