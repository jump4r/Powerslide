using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EditorNoteHold : EditorNoteBase {

    public float length;

    public bool isTransition;

    private LineRenderer lineRenderer;

    private Vector3 holdStartPoint;
    private Vector3 holdEndPoint;

    private void OnEnable()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.numPositions = 2;
    }

    public override void Construct(float offset, int notePathId, float length, bool isTransition, Vector3 spawnPosition)
    {
        this.offset = offset;
        this.notePathId = notePathId;
        this.length = length;
        this.isTransition = isTransition;
        this.startPosition = spawnPosition;

        if (isTransition)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void Update()
    {
        rTP = 1f - ((offset - EditorConductor.instance.songPosition) / (EditorConductor.instance.spb * NoteHelper.Whole * 2f));
        transform.position = new Vector3(startPosition.x,
                                         startPosition.y - (NoteHelper.Whole * 2f * Settings.PlayerSpeedMult * Mathf.Sin(xRotation) * rTP),
                                         startPosition.z - (NoteHelper.Whole * 2f * Settings.PlayerSpeedMult * Mathf.Cos(xRotation) * rTP));
        CalculatePositions();

        if (rTP > 1f && activateHitSound)
        {
            PlayHitSound();
        }
    }

    private void CalculatePositions()
    {
        holdStartPoint = this.transform.position;
        holdEndPoint = new Vector3(NotePath.NotePaths[notePathId].transform.position.x, holdStartPoint.y + (length * Settings.PlayerSpeedMult * Mathf.Sin(xRotation)), holdStartPoint.z + (length * Settings.PlayerSpeedMult * Mathf.Cos(xRotation)));

        lineRenderer.SetPosition(0, holdStartPoint);
        lineRenderer.SetPosition(1, holdEndPoint);
    }

    public override void SetLength(float length)
    {
        this.length = length;
    }

    public override string ToString()
    {
        string rtn = (Mathf.RoundToInt(offset * 1000)).ToString() + ",3," + notePathId + "," + length;
        rtn += (isTransition) ? ",true" : ",false";

        return rtn;
    }
}
