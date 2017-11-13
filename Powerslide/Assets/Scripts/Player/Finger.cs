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

public class Finger : MonoBehaviour {

    private Player player;
    private Slider slider = null;

    public Touch touch;
    public int FingerID;

    public NoteBase ActiveNote; // The note this finger is currently tapping/holding
    public NoteType ActiveNoteType;
    private FingerState fingerState;

    // Where is the finger on the hitboard
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
        RaycastHit[] hitObjects = player.GetHitObjects(Input.GetTouch(FingerID).position);

        for (int i = 0; i < hitObjects.Length; i++)
        {
            // If we hit a Notepath...
            if (hitObjects[i].collider.tag == "NotePath")
            {
                NotePath hitPath = hitObjects[i].collider.gameObject.GetComponent<NotePath>();
                notePathID = hitPath.NotePathID;
                NoteType noteType = hitPath.CheckIfValidHit();

                if (noteType != NoteType.NULL)
                {
                    ActiveNote = hitPath.ActiveNotes[0];
                }

                hitPath.Tapped();

                player.hitNotePathWithFinger = true;
                break;
            }

            else if (hitObjects[i].collider.tag == "SliderBar")
            {
                slider = player.Slider.GetComponent<Slider>();
                slider.SetSliderRelativeToFinger(FingerID, hitObjects[i].point);
                fingerState = FingerState.SLIDE;
                break;
            }
        }
    }
	
    public void ResetFinger()
    {
        Debug.Log("ANDROID DEBUG: Finger has be reset");
        ActiveNote = null;
        fingerState = FingerState.HOLD;
    }

    public void UpdateFinger()
    {
        if (fingerState == FingerState.SLIDE)
        {
            // player.MoveSlider(Input.GetTouch(FingerID).position);
            if (slider != null)
            {
                slider.Move(Input.GetTouch(FingerID).position);
                return;
            }
        }

        // Do not update finger on first press, instead update to a HOLD state.
        else if (fingerState == FingerState.TAP)
        {
            fingerState = FingerState.HOLD;
            return;
        }

        if (ActiveNote != null && (ActiveNote.type == NoteType.Hold || ActiveNote.type == NoteType.Transition || ActiveNote.type == NoteType.Flick))
        {
            RaycastHit[] hitObjects = player.GetHitObjects(Input.GetTouch(FingerID).position);
            int objectIndex = player.GetRelevantHitObjectIndex(Input.GetTouch(FingerID).position);

            for (int i = 0; i < hitObjects.Length; i++)
            {
                NotePath np = hitObjects[i].collider.gameObject.GetComponent<NotePath>();
                // CASE: HOLD NOTE //
                // For HOLD notes (Not Transition notes), players need to tap the note before it gets activiated. If the enableHoldNote flag is not active, this should not count as a note press.
                if (ActiveNote.type == NoteType.Hold && !ActiveNote.IsTapped)
                {
                    Debug.Log("Android Debug: note has not been primed, skip");
                    continue;
                }

                // In the case where the player's finger slides off of the NotePath, we need to check to see if the HoldNote is finished.
                if ((ActiveNote.type == NoteType.Hold || ActiveNote.type == NoteType.Transition) && np.NotePathID != ActiveNote.notePathID)
                {
                    ActiveNote.CalculateHoldEndError();
                }

                // If the player is on the on the path, tell them to keep on truckin
                else if ((ActiveNote.type == NoteType.Hold || ActiveNote.type == NoteType.Transition) && np.NotePathID == ActiveNote.notePathID)
                {
                    ActiveNote.IsBeingHeld();
                }

                // CASE: FLICK NOTE //
                // In the case when the flick note is finished.
                else if (ActiveNote.type == NoteType.Flick && np.NotePathID == ActiveNote.endPath)
                {
                    Debug.Log("Android Debug: Flick has been finished");
                    ActiveNote.CalculateError();
                    ResetFinger();
                }

                else if (ActiveNote.type == NoteType.Flick && np.NotePathID == ActiveNote.startPath)
                {
                    Debug.Log("Android Debug: Continue Flicking");
                }
            }
        }
    }
}
