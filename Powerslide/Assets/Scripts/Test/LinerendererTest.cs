using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinerendererTest : MonoBehaviour
{

    public float movementSpeed = 0.1f;
    LineRenderer currentLine;
    int numPoints;

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Updating");

        if (Input.GetKeyDown(KeyCode.F))
        {
            GameObject newLine = new GameObject();
            currentLine = newLine.AddComponent<LineRenderer>();
            currentLine.startWidth = 0.1f;
            currentLine.endWidth = 0.1f;
        }
        else if (Input.GetKey(KeyCode.F))
        {
            currentLine.numPositions = numPoints + 1;
            currentLine.SetPosition(numPoints, transform.position);
            numPoints++;
        }

        //Movement
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(transform.forward * movementSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(transform.right * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(-transform.forward * movementSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-transform.right * movementSpeed);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(transform.up * movementSpeed);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(-transform.up * movementSpeed);
        }
    }
}
