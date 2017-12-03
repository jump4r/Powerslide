using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorNoteBase : MonoBehaviour
{

    public float rTP;

    protected bool activateHitSound = true;

    // Start and End Vars
    protected Vector3 startPosition;
    protected Vector3 endPosition;

    protected float xRotation = 55f * Mathf.PI / 180f;

    // Note Details
    public int notePathId;
    public float offset;
    protected NoteType type;

    // Update is called once per frame
    void Update()
    {
        rTP = 1f - ((offset - EditorConductor.instance.songPosition) / (EditorConductor.instance.spb * NoteHelper.Whole * 2f));
        transform.position = new Vector3(startPosition.x, 
                                         startPosition.y - (NoteHelper.Whole * 2f * Settings.PlayerSpeedMult * Mathf.Sin(xRotation) * rTP),
                                         startPosition.z - (NoteHelper.Whole * 2f * Settings.PlayerSpeedMult * Mathf.Cos(xRotation) * rTP));

        if (rTP > 1f && activateHitSound)
        {
            PlayHitSound();
        }

    }
    public virtual void Construct(float offset, int notePathId, Vector3 spawnPosition) { }
    public virtual void Construct(float offset, int notePathId, float length, bool isTransition, Vector3 spawnPosition) { }
    public virtual void Construct(float offset, int notePathId, int endPath, bool left, Vector3 spawnPosition) { }
    public virtual void Construct(float offset, int notePathId, int endPath, float length, NoteDragType dragType, Vector3 spawnPosition) { }

    public virtual void PlayHitSound()
    {
        if (EditorManager.instance.NoteHitSoundsActive)
        {
            SoundEffectsManager.instance.PlayOneShotHitSound(0.8f);
        }

        activateHitSound = false;
    }

    public virtual void SetLength(float length) { }
    public override string ToString()
    {
        return "Nothing"; // This will never be used.
    }

    public bool Equals(EditorNoteBase other)
    {
        return (Mathf.Abs((this.notePathId - other.notePathId)) < 0.01f && this.offset == other.offset);
    }
}