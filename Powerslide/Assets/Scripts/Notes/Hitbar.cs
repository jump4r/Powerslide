using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbar : Tappable {

    public static Hitbar instance; // Horrible name

    public GameObject DragNoteDebugger;

    [SerializeField]
    private Vector3 offset = Vector3.zero;
    [SerializeField]
    private float distanceFromRayOrigin = 0;

    // Use this for initialization
    void Start()
    {
        if (!instance)
        {
            instance = this;
        }

        else
        {
            Destroy(this);
        }
    }

    // Called on Finger Down
    public void SetSliderRelativeToFinger(int fingerId, Vector3 fingerHitPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(fingerId).position);
        transform.position = new Vector3(fingerHitPosition.x, transform.position.y, transform.position.z);
        Debug.Log("Android Debug: Set Position: " + transform.position);
        offset = transform.position - fingerHitPosition;
        distanceFromRayOrigin = (ray.origin - fingerHitPosition).magnitude;
    }

    // Called On Finger Move
    public void Move(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        Vector3 oldPosition = transform.position;
        Vector3 newXPosition = ray.GetPoint(distanceFromRayOrigin) + offset;
        Vector3 newPos = new Vector3(newXPosition.x, oldPosition.y, oldPosition.z);

        //
        if (Player.instance.activeNoteDrag != null)
        {
            if (Player.instance.activeNoteDrag.CheckIfOnPath(newPos))
            {
                MoveSliderRelativeToDragPosition(Player.instance.activeNoteDrag.GetXRelPos());
                return;
            }
        }

        // Else set the position to the original intended value
        transform.position = newPos;
        // Debug.Log("Android Debug: Set Position: " + transform.position);
    }

    // If the player is 'close enough' to a drag note, we should account for the delay and place them right on top of the note.
    private void MoveSliderRelativeToDragPosition(float newX)
    {
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }


    public void Tapped(Vector3 position)
    {
        if (ActiveNotes.Count <= 0) // no active notes
        {
            return;
        }

        // To do, is tap position close enough to call it a successfull tap?
       
    }

    public void Held()
    {
        if (ActiveNotes.Count <= 0) // no active notes
        {
            return;
        }

        // Debug.Log("Android Debug: Held (NotePath");
        ActiveNotes[0].Held();
    }

    // Finger lifted from the screen
    public void Lifted()
    {
        SetRenderMaterial(pathColorDefault);

        if (ActiveNotes.Count <= 0)
        {
            return;
        }

        ActiveNotes[0].Lift();
    }
}
