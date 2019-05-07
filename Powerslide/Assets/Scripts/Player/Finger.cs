using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FingerState
{
    RESET = 0,
    TAP = 1,
    HOLD = 2,
    TRANSITION = 3,
    SLIDE = 4
}

public class Finger {

    private Player player;
    private Slider slider = null;

    public Touch touch;
    public int FingerID;

    public NoteBase ActiveNote; // The note this finger is currently tapping/holding
    public NoteType ActiveNoteType;
    private FingerState fingerState;

    // Where is the finger on the hitboard
    private int previousNotePathID;
    private int notePathID;
    public bool isMovingSlider = false;
    public bool enableHoldNote = false; // Finger must tuch the screen before activating a hold note (as opposed to dragging from a different section of the screen)

    public Finger(Touch touch, int FingerID)
    {
        this.touch = touch;
        this.FingerID = FingerID;
        fingerState = FingerState.TAP;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        // Initial Finger Tap
        FingerTap();  
    }

    private void FingerTap()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(FingerID).position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, player.layermask))
        {
            // If we hit a Notepath...
            if (hit.collider.tag == "NotePath")
            {
                NotePath hitPath = hit.collider.gameObject.GetComponent<NotePath>();
                notePathID = hitPath.NotePathID;
                previousNotePathID = notePathID;
                NoteType noteType = hitPath.CheckIfValidHit();

                if (noteType != NoteType.NULL)
                {
                    ActiveNote = hitPath.ActiveNotes[0];
                }

                hitPath.Tapped();

                player.hitNotePathWithFinger = true;
            }

            // Edit this code to match what we want for new drag notes.
            else if (hit.collider.tag == "Hitbar")
            {
                // To be replaced with setting finger position to the hitbar.
                // slider = player.Slider.GetComponent<Slider>();
                // slider.SetSliderRelativeToFinger(FingerID, hit.point);
                fingerState = FingerState.SLIDE;
            }
        }
    }
	
    public void ResetFinger()
    {
        ActiveNote = null;
        fingerState = FingerState.HOLD;
    }

    public void UpdateFinger()
    {

        // Update fingerState to check for transitions. 
        bool cancelUpdate = UpdateFingerState();

        if (cancelUpdate)
        {
            return;
        }

        if (fingerState == FingerState.SLIDE)
        {
            if (slider != null)
            {
                slider.Move(Input.GetTouch(FingerID).position);
                return;
            }
        }

        else if (fingerState == FingerState.HOLD)
        {
            NotePath.NotePaths[notePathID].Held();
        }

        else if (fingerState == FingerState.TRANSITION)
        {
            NotePath.NotePaths[previousNotePathID].Transitioned(notePathID);
            previousNotePathID = notePathID;
        }
    }

    // Returns a cancellation request to the Update Finger function
    public bool UpdateFingerState()
    {
        // Tap -> Hold
        if (fingerState == FingerState.TAP || fingerState == FingerState.TRANSITION || fingerState == FingerState.RESET)
        {
            fingerState = FingerState.HOLD;
            return true;
        }

        // Check Hold -> Transition
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(FingerID).position);
        RaycastHit hit;

        NotePath np = null;

        if (Physics.Raycast(ray, out hit, 100f, player.layermask))
        {
            if (hit.collider.tag == "NotePath")
            {
                np = hit.collider.gameObject.GetComponent<NotePath>();
                notePathID = np.NotePathID;
                // Debug.Log("Android Debug: Currently Holding on Note Path: " + hitObjects[i].collider.gameObject.GetComponent<NotePath>() + ", previous Note Path was :);
            }
        }

        if (np = null)
        {
            return true;
        }

        if (previousNotePathID != notePathID)
        {
            fingerState = FingerState.TRANSITION;
            return false;
        }

        return false;
    }

    public void LiftFinger()
    {
        NotePath.NotePaths[notePathID].Lifted();
    }
}
