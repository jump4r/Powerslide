using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finger : MonoBehaviour {

    public Touch touch;
    public int FingerID;

    public NoteBase ActiveNote; // The note this finger is currently tapping/holding
    public NoteType ActiveNoteType;

    public bool isMovingSlider = false;
    public bool enableHoldNote = false; // Finger must tuch the screen before activating a hold note (as opposed to dragging from a different section of the screen)

    public Finger(Touch touch, int FingerID)
    {
        this.touch = touch;
        this.FingerID = FingerID; 
    }
	
    public void ResetFinger()
    {
        Debug.Log("ANDROID DEBUG: Finger has be reset");
        ActiveNote = null;
        isMovingSlider = false;
        enableHoldNote = false;
    }
}
