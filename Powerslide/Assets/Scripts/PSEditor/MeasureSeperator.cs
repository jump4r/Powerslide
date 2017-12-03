﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureSeperator : MonoBehaviour {

    public MeasureSeperatorType type;
    public float timestamp;
    public int beatSegment; // Section of the beat (based off of a 
    public Vector3 defaultStartPosition;
    private readonly float defaultXRotation = 55f * Mathf.PI / 180f;

    private Material baseMat;
    public Material selectedMat;

    public float rTP;

    private bool setAsClosest = true;

    private bool playHitSound = true;

    public void Construct(float timestamp, Vector3 defaultStartPosition, MeasureSeperatorType type, int beatSegment)
    {
        this.timestamp = timestamp;
        this.defaultStartPosition = defaultStartPosition;
        this.beatSegment = beatSegment;
        this.type = type;
        baseMat = GetComponent<Renderer>().material;
        rTP = 1f;

        if ((int)type > (int)MeasureSeperatorManager.instance.selectedType)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (EditorConductor.instance.source.isPlaying)
        {
            UpdateSeperatorPosition();

#if UNITY_EDITOR 
            if (rTP > 1f && playHitSound)
            {
                PlayHitSound();
            }
#endif
        }

    }   

    private void PlayHitSound()
    {
        // Play the Measure hitsounds instead of the note hit sounds.
        if (!EditorManager.instance.NoteHitSoundsActive)
        {
            SoundEffectsManager.instance.PlayOneShotHitSound(0.6f);
            playHitSound = false;
        }
    }

    public void UpdateSeperatorPosition()
    {
        rTP = 1f - ((timestamp - EditorConductor.instance.songPosition) / (NoteHelper.Whole * 2f * EditorConductor.instance.spb));
        transform.position = new Vector3(0, defaultStartPosition.y - (NoteHelper.Whole * 2f * Settings.PlayerSpeedMult * Mathf.Sin(defaultXRotation) * rTP),
                                            defaultStartPosition.z - (NoteHelper.Whole * 2f * Settings.PlayerSpeedMult * Mathf.Cos(defaultXRotation) * rTP));

        // Bug here, hacky fix set to .99
        if (rTP >= 0.98f && setAsClosest && gameObject.activeInHierarchy)
        {
            SetClosest();
        }

        else
        {
            setAsClosest = true;
        }
    }

    public void SetClosest()
    {
        if (MeasureSeperatorManager.instance.closestSeperator != null)
        {
            MeasureSeperatorManager.instance.closestSeperator.UnsetClosest();
        }
        MeasureSeperatorManager.instance.closestSeperator = this;
        setAsClosest = false;
        gameObject.GetComponent<Renderer>().material = selectedMat;
    }

    // Reset setAsClosest
    public void UnsetClosest()
    {
        setAsClosest = true;
        gameObject.GetComponent<Renderer>().material = baseMat;
    }

    public void ResetHitSound()
    {
        playHitSound = true;
    }
}
