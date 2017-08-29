using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finger : MonoBehaviour {

    public Touch touch;
    public int FingerID;

    public NoteBase ActiveNote; // The note this finger is currently tapping/holding
    public NoteType ActiveNoteType;

    public bool isMovingSlider = false;

    public Finger(Touch touch, int FingerID)
    {
        this.touch = touch;
        this.FingerID = FingerID; 
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
