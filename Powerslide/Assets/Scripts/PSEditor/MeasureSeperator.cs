using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureSeperator : MonoBehaviour {

    public float timestamp;
    private Vector3 defaultStartPosition;
    private readonly float defaultXRotation = 55f * Mathf.PI / 180f; 

    private float rTP;

    public void Construct(float timestamp, Vector3 defaultStartPosition)
    {
        this.timestamp = timestamp;
        this.defaultStartPosition = defaultStartPosition;
        rTP = 1f;
    }

    private void Update()
    {
        if (EditorConductor.instance.source.isPlaying)
        {
            rTP = 1f - ((timestamp - EditorConductor.instance.songPosition) / (NoteHelper.Whole * 2f * EditorConductor.instance.spb));
            transform.position = new Vector3(0, defaultStartPosition.y - (NoteHelper.Whole * 2f * Settings.PlayerSpeedMult * Mathf.Sin(defaultXRotation) * rTP), 
                                                defaultStartPosition.z - (NoteHelper.Whole * 2f * Settings.PlayerSpeedMult * Mathf.Cos(defaultXRotation) * rTP));
        }
    
    }
}
