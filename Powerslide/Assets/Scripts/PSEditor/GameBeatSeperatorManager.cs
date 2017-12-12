using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class GameBeatSeperatorManager : MonoBehaviour
{

    public static GameBeatSeperatorManager instance = null;

    private float xSpawnRotation = 55f * Mathf.PI / 180f;
    private float defaultDistanceFromHitboard = NoteHelper.Whole * 2f * Settings.PlayerSpeedMult;

    private Vector3 basePosition;
    private Quaternion baseRotation;

    public GameObject MSQuarter;
    public GameObject MSEighth;
    public GameObject MSSixteenth;

    public GameObject BoardObject;


    private List<GameBeatSeperator> activeSeperators;
    public GameBeatSeperator closestSeperator; // This will be used when we need to place a note, it will be faster than looping through the entire list to find where the player should place their note.

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }
    // Use this for initialization
    void Start()
    {

        instance = this;

        activeSeperators = new List<GameBeatSeperator>();

        basePosition = new Vector3(0f, BoardObject.transform.position.y + defaultDistanceFromHitboard * Mathf.Sin(xSpawnRotation) + 0.1f,
                                        BoardObject.transform.position.z + defaultDistanceFromHitboard * Mathf.Cos(xSpawnRotation));
        baseRotation = BoardObject.transform.rotation;

        InitializeSeperators();
    }

    private void Update()
    {
        // Add a new measure seperator if needed.
        if (Conductor.instance.source.isPlaying && activeSeperators.Count > 0)
        {
            GameBeatSeperator final = activeSeperators[activeSeperators.Count - 1];

            if (final.timestamp < Conductor.songPosition + (Conductor.spb * NoteHelper.Whole * 2f))
            {
                GameBeatSeperator newInstance = SpawnSeperator(final.timestamp + Conductor.spb * NoteHelper.Quarter, MeasureSeperatorType.Quarter, 0);
                activeSeperators.Add(newInstance);
            }
        }
    }

    public void InitializeSeperators()
    {
        float currentSongPos = Conductor.offset;

        while (currentSongPos < Conductor.offset * NoteHelper.Whole * 2f)
        {
            if (currentSongPos < 0)
            {
                currentSongPos += Conductor.spb * NoteHelper.Quarter;
                continue;
            }

            GameBeatSeperator seperator = SpawnSeperator(currentSongPos, MeasureSeperatorType.Quarter, 0);

            activeSeperators.Add(seperator);

            currentSongPos += Conductor.spb * NoteHelper.Quarter;
        }
    }


    private GameBeatSeperator SpawnSeperator(float songPosition, MeasureSeperatorType type, int selectedBeat)
    {
        float ratio = songPosition / (Conductor.spb * NoteHelper.Whole * 2f);
        Vector3 spawnPosition = new Vector3(0f, BoardObject.transform.position.y + defaultDistanceFromHitboard * ratio * Mathf.Sin(xSpawnRotation) + 0.1f, BoardObject.transform.position.z + defaultDistanceFromHitboard * ratio * Mathf.Cos(xSpawnRotation));

        GameBeatSeperator tmp = Instantiate(MSQuarter, spawnPosition, baseRotation).GetComponent<GameBeatSeperator>();
        tmp.Construct(songPosition, basePosition, type, selectedBeat);

        return tmp;
    }

    public GameBeatSeperator FindClosestOffsetFromPoint(Vector3 hitPoint)
    {
        int closestIndex = activeSeperators.FindIndex(s => s.timestamp == closestSeperator.timestamp);
        GameBeatSeperator rtnSeperator = null;
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