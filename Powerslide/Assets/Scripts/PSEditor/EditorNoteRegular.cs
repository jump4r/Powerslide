using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorNoteRegular : EditorNoteBase {

	// Use this for initialization
	void Start () {
		
	}

    public override void Construct(float offset, int notePathId, Vector3 spawnPosition)
    {
        this.offset = offset;
        this.notePathId = notePathId;
        this.startPosition = spawnPosition;
    }

    public override string ToString()
    {
        return (Mathf.RoundToInt(offset * 1000)).ToString() + ",0," + notePathId;
    }

}
