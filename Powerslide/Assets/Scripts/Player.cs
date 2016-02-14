using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public Touch touch;
    private int touchIndexCache; // For detecting which finger to follow when dragging
    public LayerMask mask; // We only want to detect certain collisions
    public int layermask = 1 << 8; // Layermask is broken as fuck.

    private static int fingersTouching = 0; // Number of fingers touching the screen

    // Dragging Variables
    // Stretch Goal: See if we can get it so that we can have two flicks at the same time
    private bool sliderDragEnabled = false;

    // Flick variables
    private bool flickDragEnabled = false;
    private NotePath endFlickPath;
    private NoteFlick activeNoteFlick;

    private Vector3 offset = Vector3.zero;
    private float distanceFromRayOrigin = 0;

    public GameObject Slider;
	// Use this for initialization
	void Start () {

	}
	    
	// Update is called once per frame
	void Update () {

        // Detect a touch from the screen.
        if (Input.GetButtonDown("Touch"))
        {
            GameObject note = GameObject.Find("Note");
            note.GetComponent<Note>().ChangeMaterial();

            // Add one to the touches list.
            fingersTouching += 1;

            // Player clicks...
            // Determine which Notepath was hit.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit[] hitObjects = Physics.RaycastAll(ray, 1000f, layermask);
            NotePath hitPath;
            Debug.Log("Number of objects hit: " + hitObjects.Length);
            for (int i = 0; i < hitObjects.Length; i++)
            {
                // If we hit a Notepath...
                if (hitObjects[i].collider.tag == "NotePath")
                {
                    hitPath = hitObjects[i].collider.gameObject.GetComponent<NotePath>();
                    Debug.Log("Hit Object: " + hitPath.name);
                    flickDragEnabled = hitPath.CheckIfValidHit();
                    if (flickDragEnabled)
                    {
                        Debug.Log("Time to drag!");
                        endFlickPath = NotePath.NotePaths[hitPath.ActiveNotes[0].pathEnd];
                        activeNoteFlick = (NoteFlick)hitPath.ActiveNotes[0];
                    }
                    break;
                }

                // Drag the sliderbar
                else if (hitObjects[i].collider.tag == "SliderBar")
                {
                    Debug.Log("We have hit the sliderbar");
                    // First, center the slider to the mouse position, and set the offset
                    Slider.transform.position = new Vector3(hitObjects[i].point.x, Slider.transform.position.y, Slider.transform.position.z);
                    offset = Slider.transform.position - hitObjects[i].point;
                    distanceFromRayOrigin = (ray.origin - hitObjects[i].point).magnitude;
                    sliderDragEnabled = true;
                    // Here we will cache the touch ID and send it to the Sliderbar
                }
            }
        }

        // Remove that finger from the amount of total fingers touching
        if (Input.GetButtonUp("Touch"))
        {
            fingersTouching -= 1;
            sliderDragEnabled = false;
            flickDragEnabled = false;   
        }

        // If we are dragging an the slider
        else if (sliderDragEnabled)
        {
            // This will be replaced with finger location via finger ID
            // TODO: See if there is an easier way to do this 1-Dimmentional Movement
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 oldPosition = Slider.transform.position;
            Vector3 newXPosition = ray.GetPoint(distanceFromRayOrigin) + offset;
            Slider.transform.position = new Vector3(newXPosition.x, oldPosition.y, oldPosition.z);
        }

        // If the player is currently flicking
        if (flickDragEnabled)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hitObjects = Physics.RaycastAll(ray, 1000f, layermask);
            Debug.Log("We have hit: " + hitObjects.Length);
            for (int i = 0; i < hitObjects.Length; i++)
            {
                if (hitObjects[i].collider.tag == "NotePath")
                {
                    Debug.Log("Currently dragging along path: " + hitObjects[i].collider.gameObject.GetComponent<NotePath>().NotePathID);
                    if (hitObjects[i].collider.gameObject.GetComponent<NotePath>().NotePathID == endFlickPath.NotePathID && activeNoteFlick != null)
                    {
                        activeNoteFlick.ChangeMaterial();
                        activeNoteFlick = null;
                    }
                }
            }

            /* Code that should work, but for some reason, doesn't 
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, mask))
            {
                Debug.Log(hit.collider.name);
                Debug.Log("We are currently dragging along path " + hit.collider.gameObject.GetComponent<NotePath>().NotePathID);
            } */
        }
	}
}
