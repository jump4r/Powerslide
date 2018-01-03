﻿using UnityEngine;
using System.Collections;

public enum FlickType
{
    RIGHT = 0,
    LEFT = 1
};

public class NoteFlick : NoteBase {

    public bool direction; // True - right, False = Left, could easily be done with a boolean but better for readibility
    // private Material leftFlick;         // LEGACY
    // private Material rightFlick;        // LEGACY

    private SpriteRenderer sr { get; set; }
    private Animator animator { get; set; }
    private Sprite leftFlickSprite { get; set; }
    private Sprite rightFlickSprite { get; set; }

    // Getters/Setters
    public override void SetFingerId(int id) { fingerId = id; }

	// Use this for initialization
	void OnEnable () {
        // TODO: Not really sure yet, but make this work with object pooling
        type = NoteType.Flick;
        //leftFlick = Resources.Load("Materials/LeftFlick") as Material;
        // rightFlick = Resources.Load("Materials/RightFlick") as Material;

        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        leftFlickSprite = Resources.Load("Materials/FlickLeftSpritesheet") as Sprite;
        rightFlickSprite = Resources.Load("Materials/FlickRightSpritesheet") as Sprite;
	}

    public override void Construct(Vector3 spawnPosition, float offset, int startPath, int endPath, bool direction, string NoteName)
    {
        EndTime = offset;
        notePathID = startPath; // redundant

        transform.position = spawnPosition;
        startPosition = spawnPosition;

        this.startPath = startPath;
        this.endPath = endPath;
        gameObject.name = NoteName;
        SetFlickMaterial(direction); // last var not neded
    }

    // Sets the material on the Flick Note.
    public void SetFlickMaterial(bool dir)
    {
        // gameObject.GetComponent<Renderer>().material = dir ? rightFlick : leftFlick;
        if (dir)
        {
            sr.sprite = rightFlickSprite;
            Debug.Log("Right Flick");
        }

        else
        {
            sr.sprite = leftFlickSprite;
            sr.flipX = true;
            Debug.Log("Left Flick");
        }
    }

    // Changes the material of the note depending on the 
    public override void ChangeMaterial(Material mat)
    {
        gameObject.GetComponent<Renderer>().material = mat;
    }

    public override void Transitioned(int startPathID, int endPathID)
    {
        if (this.startPath == startPathID && this.endPath == endPathID && isReadyToHit)
        {
            CalculateError();
        }
    }

    protected override void ResetNote()
    {
        active = true;
        IsTapped = false;
        isReadyToHit = false;

    }
}