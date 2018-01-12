using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slider : MonoBehaviour
{
    // I don't really know what these are for tbh
    [SerializeField]
    private Vector3 offset = Vector3.zero;
    [SerializeField]
    private float distanceFromRayOrigin = 0;

    // Called on Finger Down
    public void SetSliderRelativeToFinger(int fingerId, Vector3 hitObjectPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(fingerId).position);
        transform.position = new Vector3(hitObjectPosition.x, transform.position.y, transform.position.z);
        // Debug.Log("Android Debug: Set Position: " + transform.position);
        offset = transform.position - hitObjectPosition;
        distanceFromRayOrigin = (ray.origin - hitObjectPosition).magnitude;
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
            if ( Player.instance.activeNoteDrag.CheckIfOnPath(newPos) )
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
}
