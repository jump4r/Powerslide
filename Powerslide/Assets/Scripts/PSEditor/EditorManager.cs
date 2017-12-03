using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;
using System.IO;

public class EditorManager : MonoBehaviour {

    public static EditorManager instance = null;

    public bool NoteHitSoundsActive;

    private bool loading = false;
    private string beatmapFilePath = null;
    public WWW beatmapWWW;
    public EditorBeatmap beatmap = null;

    [SerializeField]
    private LayerMask mask;

    // Debug
    [SerializeField]
    private GameObject ClickVisualizer;

    // Mouse down positions
    private Vector3 mouseDownPosition;
    private Vector3 mouseUpPosition;

    private void Start()
    {
        instance = this;   
    }

    private void Update()
    {
        if (beatmap != null)
        {

            if (Input.GetMouseButtonDown(0))
            {
                HandleClickEvent(Input.mousePosition);
            }

            else if (Input.GetMouseButtonDown(1))
            {
                HandleRightClickEvent(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                HandleMouseUpEvent(Input.mousePosition);
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                MeasureSeperatorManager.instance.ScrollForward();
            }

            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                // Debug.Log("Scroll Backward");
                MeasureSeperatorManager.instance.ScrollBackward();
            }
        }
    }

    public void OpenBeatmapFile()
    {
#if UNITY_EDITOR
        beatmapFilePath = EditorUtility.OpenFilePanel("Select Beatmap", ".", "txt");
#endif
        if (beatmapFilePath.Length != 0)
        {
            loading = true;
            StartCoroutine(LoadFromFile(beatmapFilePath));
            // StartCoroutine(ParseBeatmap());
        }
        return;
    }

    private IEnumerator LoadFromFile(string beatmapFilePath)
    {
        Debug.Log("Start Loading");
        this.beatmapWWW = new WWW("file://" + beatmapFilePath);
        loading = false;
        yield return beatmapWWW;

        ParseBeatmap();
    }

    private void ParseBeatmap()
    {
        string beatmapText = beatmapWWW.text;

        beatmap = new EditorBeatmap(beatmapText);
        EditorConductor.instance.Construct(beatmap);
        EditorConductor.instance.Play();
    }

    // Left click Mouse down event
    private void HandleClickEvent(Vector3 clickPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(clickPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, mask.value))
        {
            if (hit.collider.tag == "NotePath")
            {
                int notePathId = hit.collider.gameObject.GetComponent<NotePath>().NotePathID;
                EditorNoteManager.instance.PlaceNote(notePathId, hit.point);
            }
        }
    }

    // Left click mouse up
    private void HandleMouseUpEvent(Vector3 mouseUpPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseUpPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, mask.value))
        {
            if (hit.collider.tag == "NotePath")
            {
                int notePathId = hit.collider.gameObject.GetComponent<NotePath>().NotePathID;
                EditorNoteManager.instance.SetNote(notePathId, hit.point);
            }
        }
    }

    private void HandleRightClickEvent(Vector3 clickPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(clickPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, mask.value))
        {
            if (hit.collider.tag == "NotePath")
            {
                int notePathId = hit.collider.gameObject.GetComponent<NotePath>().NotePathID;
                EditorNoteManager.instance.RemoveNote(notePathId, hit.point);
            }
        }
    } 

    public void SaveBeatmap()
    {
        if (beatmapFilePath == null)
        {
            return;
        }

        string tempFile = Path.GetTempFileName();
        StreamReader reader = new StreamReader(beatmapFilePath);

        string line;
        List<string> lines = new List<string>();

        while ((line = reader.ReadLine()) != null)
        {
            lines.Add(line);
            if (line.Trim() == "[HitObjects]")
            {
                break;
            }
        }

        reader.Close();

        StreamWriter writer = new StreamWriter(tempFile);

        foreach (string l in lines)
        {
            writer.WriteLine(l);
        }

        foreach (EditorNoteBase note in EditorNoteManager.instance.Notes)
        {
            Debug.Log(note.ToString());
            writer.WriteLine(note.ToString());
        }

        writer.Close();

        File.Delete(beatmapFilePath);
        File.Move(tempFile, beatmapFilePath);
    } 
}
