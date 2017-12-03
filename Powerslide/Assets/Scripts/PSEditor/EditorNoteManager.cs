using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorNoteManager : MonoBehaviour {

    public static EditorNoteManager instance = null;

    private float distanceFromHitboard;
    private Vector3 basePosition;
    public Transform BoardObject;
    private float xRotation = 55f * Mathf.PI / 180f;

    // Currently Selected Variables
    public NoteType selectedNoteType = NoteType.Regular;
    public NoteDragType selectedNoteDragType = NoteDragType.Linear;
    private EditorNoteBase selectedNote = null;

    public GameObject EditorNoteRegularPrefab;
    public GameObject EditorNoteFlickPrefab;
    public GameObject EditorNoteHoldPrefab;
    public GameObject EditorNoteDragPrefab;

    public Text DragText;

    public List<EditorNoteBase> Notes;

    // Use this for initialization
    void Start() {
        if (instance == null)
        {
            instance = this;
            Notes = new List<EditorNoteBase>();
            distanceFromHitboard = Settings.PlayerSpeedMult * NoteHelper.Whole * 2f;
            basePosition = new Vector3(0f, BoardObject.position.y + distanceFromHitboard * Mathf.Sin(xRotation) + 0.1f, 
                                           BoardObject.position.z + distanceFromHitboard * Mathf.Cos(xRotation));
        }

        else
        {
            Destroy(this.gameObject);
        }

    }

    public void PreloadNotes(List<string> stringNotes)
    {
        Debug.Log("Preload Notes");
        foreach (string line in stringNotes)
        {
            if (line.Trim() == "")
            {
                continue;
            }

            string[] split = line.Trim().Split(',');

            if (split[1] == "0")
            {
                SpawnRegularNote(float.Parse(split[0]) / 1000f, int.Parse(split[2]));
            }

            else if (split[1] == "1")
            {
                SpawnFlickNote(float.Parse(split[0]) / 1000f, int.Parse(split[2]), int.Parse(split[3]), bool.Parse(split[4]));
            }

            else if (split[1] == "2")
            {
                NoteDragType tempType;
                if (split[6] == "L")
                {
                    tempType = NoteDragType.Linear;
                }

                else if (split[6] == "C")
                {
                    tempType = NoteDragType.Curve;
                }

                else
                {
                    tempType = NoteDragType.Root;
                }

                SpawnDragNote(float.Parse(split[0]) / 1000f, int.Parse(split[3]), float.Parse(split[5]), tempType, int.Parse(split[4]));
            }

            else if (split[1] == "3")
            {
                SpawnHoldNote(float.Parse(split[0]) / 1000f, int.Parse(split[2]), float.Parse(split[3]), bool.Parse(split[4]));
            }

        }
    }

    // notePath to place the note, and the position in world space where the 
    // notepath was clicked.
    // Mouse Down Event
    public void PlaceNote(int notePathId, Vector3 point, float length = 1f, int endPath = -1)
    {
        // Get the closest Measure to the click, so we can snap notes to the measure.
        MeasureSeperator seperator = MeasureSeperatorManager.instance.FindClosestOffsetFromPoint(point);

        foreach (EditorNoteBase note in Notes)
        {
            if (seperator.timestamp == note.offset && notePathId == note.notePathId)
            {
                Debug.Log("There is already a note in this position, returning");
                return;
            }
        }

        switch(selectedNoteType)
        {
            case NoteType.Regular:
                selectedNote = SpawnRegularNote(seperator.timestamp, notePathId);
                break;
            case NoteType.Hold:
                selectedNote = SpawnHoldNote(seperator.timestamp, notePathId, length, false);
                break;
            case NoteType.Transition:
                selectedNote = SpawnHoldNote(seperator.timestamp, notePathId, length, true);
                break;
            case NoteType.Flick:
                selectedNote = SpawnFlickNote(seperator.timestamp, notePathId, endPath);
                break;
            case NoteType.Drag:
                selectedNote = SpawnDragNote(seperator.timestamp, notePathId, length, selectedNoteDragType, notePathId);
                break;
        }
    }

    // Mouse Up event
    public void SetNote(int endPath, Vector3 point)
    {

        Debug.Log("Call Set Note");

        if (selectedNote == null)
        {
            return;
        }

        // Set the Hold Note length based on the mouse up
        if (selectedNoteType == NoteType.Hold || selectedNoteType == NoteType.Transition)
        {
            EditorNoteHold temp = selectedNote as EditorNoteHold;
            MeasureSeperator seperator = MeasureSeperatorManager.instance.FindClosestOffsetFromPoint(point);
            int difference = Mathf.RoundToInt(((seperator.timestamp - temp.offset) / EditorConductor.instance.spb) * 4f);
            float length = difference / 4f;
            temp.SetLength(length);
        }


        // Set the end path for flick note
        else if (selectedNoteType == NoteType.Flick)
        {
            if (Mathf.Abs(selectedNote.notePathId - endPath) != 1)
            {
                Debug.Log("Start: " + selectedNote.notePathId + ", End: " + endPath + ", Removing");
                RemoveNote(selectedNote);
                selectedNote = null;
                return;
            }
            EditorNoteFlick temp = selectedNote as EditorNoteFlick;
            temp.SetNote(endPath);
        }

        // Set the End locations and length for drag note
        else if (selectedNoteType == NoteType.Drag)
        {
            EditorNoteDrag temp = selectedNote as EditorNoteDrag;
            MeasureSeperator seperator = MeasureSeperatorManager.instance.FindClosestOffsetFromPoint(point);
            int difference = Mathf.RoundToInt(((seperator.timestamp - temp.offset) / EditorConductor.instance.spb) * 4f);
            float length = difference / 4f;
            temp.SetNote(length, endPath);
        }

        selectedNote = null;
    }

    // Remove note given a mouse Click;
    public void RemoveNote(int notePathId, Vector3 point)
    {
        // Get the closest Measure to the click, so we can determine at which note to remove
        MeasureSeperator seperator = MeasureSeperatorManager.instance.FindClosestOffsetFromPoint(point);
        
        for (int i = 0; i < Notes.Count; i++)
        {
            // Debug.Log("Seperator: " + seperator.timestamp + ", Note offset: " + Notes[i].offset);
            // Need to round float to 3 decimal points tho.
            if (Mathf.Abs((seperator.timestamp - Notes[i].offset)) < 0.01f && notePathId == Notes[i].notePathId)
            {
                Destroy(Notes[i].gameObject);
                Notes.RemoveAt(i);
                return;
            }
        }

        Debug.Log("Note note found");
    }

    // Remove note given note
    public void RemoveNote(EditorNoteBase note)
    {
        for (int i = 0; i < Notes.Count; i++)
        {
            if (note.Equals(Notes[i]))
            {
                Destroy(Notes[i].gameObject);
                Notes.RemoveAt(i);
                return;
            }
        }
    }

    private EditorNoteBase SpawnRegularNote(float timestamp, int notePathId)
    {
        Vector3 spawnPosition = new Vector3(NotePath.NotePaths[notePathId].transform.position.x, basePosition.y + 0.1f, basePosition.z);
        EditorNoteRegular note = Instantiate(EditorNoteRegularPrefab, spawnPosition, BoardObject.rotation).GetComponent<EditorNoteRegular>();

        note.Construct(timestamp, notePathId, spawnPosition);
        AddNoteToList(note);
        return note;
    }

    private EditorNoteBase SpawnHoldNote(float timestamp, int notePathId, float length, bool isTransition)
    {
        Vector3 spawnPosition = new Vector3(NotePath.NotePaths[notePathId].transform.position.x, basePosition.y + 0.1f, basePosition.z);
        EditorNoteHold note = Instantiate(EditorNoteHoldPrefab, spawnPosition, BoardObject.rotation).GetComponent<EditorNoteHold>();

        note.Construct(timestamp, notePathId, length, isTransition, spawnPosition);
        AddNoteToList(note);
        return note;
    }

    private EditorNoteBase SpawnFlickNote(float timestamp, int notePathId, int endPath = -1, bool direction = false)
    {
        Vector3 spawnPosition = new Vector3(NotePath.NotePaths[notePathId].transform.position.x, basePosition.y + 0.1f, basePosition.z);
        EditorNoteFlick note = Instantiate(EditorNoteFlickPrefab, spawnPosition, BoardObject.rotation).GetComponent<EditorNoteFlick>();

        note.Construct(timestamp, notePathId, endPath, direction, spawnPosition);
        AddNoteToList(note);
        return note;
    }

    private EditorNoteBase SpawnDragNote(float timestamp, int notePathId, float length, NoteDragType type, int endPath = -1)
    {
        Vector3 spawnPosition = new Vector3(NotePath.NotePaths[notePathId].transform.position.x, basePosition.y + 0.1f, basePosition.z);
        EditorNoteDrag note = Instantiate(EditorNoteDragPrefab, spawnPosition, BoardObject.rotation).GetComponent<EditorNoteDrag>();

        note.Construct(timestamp, notePathId, endPath, length, type, spawnPosition);
        AddNoteToList(note);
        return note;
    }

    private void AddNoteToList(EditorNoteBase note)
    {
        Notes.Add(note);
        SortNotes();
    }

    private void SortNotes()
    {
        Notes.Sort(delegate (EditorNoteBase n1, EditorNoteBase n2) { return n1.offset.CompareTo(n2.offset); });
    }

    public void SetSelectedNoteType(NoteType newNoteType)
    {
        selectedNoteType = newNoteType;
    }

    public void SetSelectedNoteToRegular(){ selectedNoteType = NoteType.Regular; }
    public void SetSelectedNoteToHold() { selectedNoteType = NoteType.Hold; }
    public void SetSelectedNoteToFlick() { selectedNoteType = NoteType.Flick; }
    public void SetSelectedNoteToTransition() { selectedNoteType = NoteType.Transition; }
    public void SetSelectedNoteToDrag()
    {
        if (selectedNoteType != NoteType.Drag)
        {
            selectedNoteType = NoteType.Drag;
            return;
        }

        else
        {
            selectedNoteDragType = (NoteDragType)(((int)selectedNoteDragType + 1) % 3);
            switch (selectedNoteDragType)
            {
                case (NoteDragType.Curve):
                    DragText.text = "Drag (Curve)";
                    break;
                case (NoteDragType.Linear):
                    DragText.text = "Drag (Linear)";
                    break;
                case (NoteDragType.Root):
                    DragText.text = "Drag (Root)";
                    break;
            }
        }
    }
}
