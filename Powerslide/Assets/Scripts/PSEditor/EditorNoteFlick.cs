using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorNoteFlick : EditorNoteBase {

    public bool direction; // True = right, False = left
    public int endPath;

	// Use this for initialization
	void Start () {
		
	}

    public override void Construct(float offset, int notePathId, int endPath, bool direction, Vector3 spawnPosition)
    {
        this.offset = offset;
        this.notePathId = notePathId;
        this.endPath = endPath;
        this.direction = direction;

        if (endPath == -1) // Note spawned via player click
        {
            this.startPosition = spawnPosition;
        }

        else // Note Spawned via Beatmap
        {
            Debug.Log(endPath);
            float middleXPosition = (NotePath.NotePaths[notePathId].transform.position.x + NotePath.NotePaths[endPath].transform.position.x) / 2f;
            this.startPosition = new Vector3(middleXPosition, spawnPosition.y, spawnPosition.z);
            SetMaterial();
        }
    }

    public void SetNote(int endPath)
    {
        this.endPath = endPath;

        float middleXPosition = (NotePath.NotePaths[notePathId].transform.position.x + NotePath.NotePaths[endPath].transform.position.x) / 2f;
        startPosition = new Vector3(middleXPosition, startPosition.y, startPosition.z);

        if (notePathId < endPath)
        {
            direction = true;
        }

        else
        {
            direction = false;
        }

        SetMaterial();
    }

    private void SetMaterial()
    {
        if (direction)
        {
            GetComponent<Renderer>().material = Resources.Load("Materials/RightFlick") as Material;
        }

        else
        {
            GetComponent<Renderer>().material = Resources.Load("Materials/LeftFlick") as Material;
        }
    }

    public override string ToString()
    {
        string rtn = (Mathf.RoundToInt(offset * 1000)).ToString() + ",1," + notePathId + "," + endPath + ",";
        rtn += (direction) ? "true" : "false";
        return rtn;
    }
}
