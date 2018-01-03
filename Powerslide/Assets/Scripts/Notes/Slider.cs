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

    public void SetSliderRelativeToFinger(int fingerId, Vector3 hitObjectPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(fingerId).position);
        transform.position = new Vector3(hitObjectPosition.x, transform.position.y, transform.position.z);
        // Debug.Log("Android Debug: Set Position: " + transform.position);
        offset = transform.position - hitObjectPosition;
        distanceFromRayOrigin = (ray.origin - hitObjectPosition).magnitude;
    }

    public void Move(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        Vector3 oldPosition = transform.position;
        Vector3 newXPosition = ray.GetPoint(distanceFromRayOrigin) + offset;
        transform.position = new Vector3(newXPosition.x, oldPosition.y, oldPosition.z);
        // Debug.Log("Android Debug: Set Position: " + transform.position);
    }
}
