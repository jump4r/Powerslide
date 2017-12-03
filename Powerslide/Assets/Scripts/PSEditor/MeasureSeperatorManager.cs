using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public enum MeasureSeperatorType
{
    Quarter = 0,
    Eighth = 1,
    Sixteenth = 2
}

public class MeasureSeperatorManager : MonoBehaviour {

    public static MeasureSeperatorManager instance = null;

    public MeasureSeperatorType selectedType;
    private int selectedBeat = 0;
    public Scrollbar scroll;

    private float xSpawnRotation = 55f * Mathf.PI / 180f;
    private float defaultDistanceFromHitboard = NoteHelper.Whole * 2f * Settings.PlayerSpeedMult;

    private Vector3 basePosition;
    private Quaternion baseRotation;

    public GameObject MSQuarter;
    public GameObject MSEighth;
    public GameObject MSSixteenth;

    public GameObject BoardObject;


    private List<MeasureSeperator> activeSeperators;
    public MeasureSeperator closestSeperator; // This will be used when we need to place a note, it will be faster than looping through the entire list to find where the player should place their note.

    // Use this for initialization
    void Start () {
		if (instance == null)
        {
            instance = this;
            selectedType = MeasureSeperatorType.Quarter;
            activeSeperators = new List<MeasureSeperator>();

            basePosition = new Vector3(0f, BoardObject.transform.position.y + defaultDistanceFromHitboard * Mathf.Sin(xSpawnRotation) + 0.1f, 
                                           BoardObject.transform.position.z + defaultDistanceFromHitboard * Mathf.Cos(xSpawnRotation));
            baseRotation = BoardObject.transform.rotation;
        }

        else
        {
            Destroy(this.gameObject);
        }
	}

    private void Update()
    {
        // Add a new measure seperator if needed.
        if (EditorConductor.instance.source.isPlaying && activeSeperators.Count > 0) 
        {
            MeasureSeperator final = activeSeperators[activeSeperators.Count - 1];

            if (final.timestamp < EditorConductor.instance.songPosition + (EditorConductor.instance.spb * NoteHelper.Whole * 2f))
            {
                int beat = (final.beatSegment + 1) % 4;
                MeasureSeperatorType type = GetMeasureSeperatorTypeFromBeat(beat);
                MeasureSeperator newInstance = SpawnSeperator(final.timestamp + EditorConductor.instance.spb * NoteHelper.Sixteenth, type, beat);
                activeSeperators.Add(newInstance);
            }
        }
    }

    public void InitializeSeperators()
    {
        float currentSongPos = EditorConductor.instance.offset;

        while (currentSongPos < EditorConductor.instance.offset * NoteHelper.Whole * 2f)
        {
            if (currentSongPos < 0)
            {
                currentSongPos += EditorConductor.instance.spb * NoteHelper.Sixteenth;
                continue;
            }

            MeasureSeperatorType spawnType = GetMeasureSeperatorTypeFromBeat(selectedBeat);
            MeasureSeperator seperator = SpawnSeperator(currentSongPos, spawnType, selectedBeat);

            activeSeperators.Add(seperator);

            selectedBeat = (selectedBeat + 1) % 4;
            currentSongPos += EditorConductor.instance.spb * NoteHelper.Sixteenth;
        }
    }

    private MeasureSeperatorType GetMeasureSeperatorTypeFromBeat(int beat)
    {
        MeasureSeperatorType type = MeasureSeperatorType.Quarter; // A default must be set, but this means nothing.

        if (beat == 0)
        {
            type = MeasureSeperatorType.Quarter;
        }

        else if (beat == 2)
        {
            type = MeasureSeperatorType.Eighth;
        }

        else if (beat == 1f || beat == 3)
        {
            type = MeasureSeperatorType.Sixteenth;
        }

        return type;
    }


    private MeasureSeperator SpawnSeperator(float songPosition, MeasureSeperatorType type, int selectedBeat)
    {
        float ratio = songPosition / (EditorConductor.instance.spb * NoteHelper.Whole * 2f);
        Vector3 spawnPosition = new Vector3(0f, BoardObject.transform.position.y + defaultDistanceFromHitboard * ratio * Mathf.Sin(xSpawnRotation) + 0.1f, BoardObject.transform.position.z + defaultDistanceFromHitboard * ratio * Mathf.Cos(xSpawnRotation));

        MeasureSeperator tmp;
        switch (type) {
            case MeasureSeperatorType.Quarter:
                tmp = Instantiate(MSQuarter, spawnPosition, baseRotation).GetComponent<MeasureSeperator>();
                break;
            case MeasureSeperatorType.Eighth:
                tmp = Instantiate(MSEighth, spawnPosition, baseRotation).GetComponent<MeasureSeperator>();
                break;
            case MeasureSeperatorType.Sixteenth:
                tmp = Instantiate(MSSixteenth, spawnPosition, baseRotation).GetComponent<MeasureSeperator>();
                break;
            default:
                tmp = Instantiate(MSQuarter, spawnPosition, baseRotation).GetComponent<MeasureSeperator>();
                break;

        }

        tmp.Construct(songPosition, basePosition, type, selectedBeat);

        return tmp;
    }

    public void ScrollForward()
    {
        float songPosition = EditorConductor.instance.songPosition;

        int index = 0;
        bool spawnNewInstance = false;
        for (index = 0; index < activeSeperators.Count; index++)
        {
            if (activeSeperators[index].timestamp < songPosition + 0.05f)
            {
                continue;
            }

            // Else, Set the the spong position and the closestSeperator var- to the current Seperator, an
            else 
            {
                EditorConductor.instance.SetSongPosition(activeSeperators[index].timestamp);
                spawnNewInstance = true;

                if (activeSeperators[index].gameObject.activeInHierarchy)
                {
                    activeSeperators[index].SetClosest();
                }
                
                break;
            }
        }

        // Spawn New Instance and Destory the first instance
        if (spawnNewInstance)
        {
            MeasureSeperator finalInstance = activeSeperators[activeSeperators.Count - 1];
            int beat = (finalInstance.beatSegment + 1) % 4;
            MeasureSeperatorType type = GetMeasureSeperatorTypeFromBeat(beat);
            MeasureSeperator newInstance = SpawnSeperator(finalInstance.timestamp + EditorConductor.instance.spb * NoteHelper.Sixteenth, type, beat);
            activeSeperators.Add(newInstance);
        }

        foreach (MeasureSeperator seperator in activeSeperators)
        {
            seperator.UpdateSeperatorPosition();
        }
    }

    public void ScrollBackward()
    {
        float songPosition = EditorConductor.instance.songPosition;
        MeasureSeperator lastInstance = null;

        int index = 0;
        for (index = 0; index < activeSeperators.Count; index++)
        {

            if (activeSeperators[index].timestamp < songPosition - 0.05f)
            {
                lastInstance = activeSeperators[index];
                continue;
            }

            else
            {
                break;
            }
        }

        if (lastInstance != null)
        {
            EditorConductor.instance.SetSongPosition(lastInstance.timestamp);
            if (lastInstance.gameObject.activeInHierarchy)
            {
                lastInstance.SetClosest();
                lastInstance.ResetHitSound();
            }
        }

        foreach (MeasureSeperator seperator in activeSeperators)
        {
            seperator.UpdateSeperatorPosition();
        }
    }

    public void UpdateSeperatorSelectedType()
    {
        int newType = (int)(scroll.value * 2);

        if (newType == (int)selectedType)
        {
            return;
        }

        selectedType = (MeasureSeperatorType)(scroll.value * 2);
        
        foreach (MeasureSeperator seperator in activeSeperators)
        {
            if ((int)seperator.type > (int)selectedType)
            {
                seperator.gameObject.SetActive(false);
            }

            else
            {
                if (!seperator.gameObject.activeInHierarchy)
                {
                    seperator.gameObject.SetActive(true);
                    seperator.UpdateSeperatorPosition();
                }
            }
        }
    }

    public MeasureSeperator FindClosestOffsetFromPoint(Vector3 hitPoint)
    {
        int closestIndex = activeSeperators.FindIndex(s => s.timestamp == closestSeperator.timestamp);
        MeasureSeperator rtnSeperator = null;
        float lastDistance = float.MaxValue;
        float distance = 0;
        for (int i = closestIndex; i < activeSeperators.Count; i++)
        {

            if (!activeSeperators[i].gameObject.activeInHierarchy)
            {
                continue;
            }

            distance = Vector3.Distance(hitPoint, activeSeperators[i].transform.position);

            if (distance < lastDistance)
            {
                lastDistance = distance;
                rtnSeperator = activeSeperators[i];
            }

            else
            {
                break;
            }
        }

        return rtnSeperator;
    }
}