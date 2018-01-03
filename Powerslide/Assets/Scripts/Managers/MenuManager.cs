using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MenuManager : MonoBehaviour {

    public static MenuManager instance = null;

    [SerializeField]
    private GameObject Menu;
    [SerializeField]
    private GameObject DifficultyModal;

    // Player finger variables to determine where to drag, what to do when pressed etc.
    private Vector3 mouseDownPos { get; set; }
    private Vector3 mouseUpPos { get; set; }
    private Vector3 lastMousePosition { get; set; }

    private bool scrolling = false; // is the player scrolling?
    public bool passedScrollThreshold = false;

	// Use this for initialization
	void Start () {
		if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(this.gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (!Menu.activeInHierarchy)
        {
            return;
        }

		if (Input.GetMouseButtonDown(0))
        {
            HandleFingerDown(Input.mousePosition);
            Debug.Log("Stop Coroutine");
            BeatmapSelectMenu.instance.StopAllCoroutines(); // Stop Auto Scroll
        }

        else if (Input.GetMouseButtonUp(0))
        {
            HandleFingerUp(Input.mousePosition);
        }

        else if (scrolling)
        {
            // If we have passed a certain degree, marked 'scrolled' as true,
            // That means that buttons will be deactivated 
            float totalYDifference = Mathf.Abs(mouseDownPos.y - lastMousePosition.y);

            if (totalYDifference > 50)
            {
                passedScrollThreshold = true;
            }

            Vector3 currentMousePos = Input.mousePosition;
            float yDifference = currentMousePos.y - lastMousePosition.y;

            // Scroll the through the maps
            BeatmapSelectMenu.instance.ScrollBeatmaps(yDifference);

            lastMousePosition = currentMousePos;
        }
	}

    private void HandleFingerDown(Vector3 pos)
    {
        mouseDownPos = pos;
        lastMousePosition = pos;
        scrolling = true;
    }

    // Reset everything
    private void HandleFingerUp(Vector3 pos)
    {
        mouseUpPos = pos;
        scrolling = false;
        passedScrollThreshold = false;
    }

    // Pathing may need to change for android
    public void EnableDifficultyModal(string path)
    {
        // IF we have passed the scroll threshold, do nothing.
        if (passedScrollThreshold)
        {
            Debug.Log("Passed scroll threshold.");
            return;
        }

        Debug.Log(path);
        DirectoryInfo info = new DirectoryInfo(path);
        foreach (FileInfo file in info.GetFiles("*txt"))
        {
            Debug.Log(file.Name);
        }

        DifficultyModal.SetActive(true);
        DifficultyModal.GetComponent<DifficultyModal>().Initialize(info.GetFiles("*txt"));
    }

    public void DisableDifficultyModal()
    {
        DifficultyModal.SetActive(false);
    }
}
