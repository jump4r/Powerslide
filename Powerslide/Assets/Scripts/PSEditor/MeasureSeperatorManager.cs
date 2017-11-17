using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeasureSeperatorManager : MonoBehaviour {

    public static MeasureSeperatorManager instance = null;

    private float xSpawnRotation = 55f * Mathf.PI / 180f;
    private float defaultDistanceFromHitboard = NoteHelper.Whole * 2f * Settings.PlayerSpeedMult;

    private Vector3 basePosition;
    private Quaternion baseRotation;

    public GameObject MSQuarter;
    public GameObject MSEighth;
    public GameObject MSSixteenth;

    public GameObject BoardObject;

    private List<MeasureSeperator> activeSeperators;

	// Use this for initialization
	void Start () {
		if (instance == null)
        {
            instance = this;
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

    public void InitializeSeperators()
    {
        float currentSongPos = EditorConductor.instance.offset - (EditorConductor.instance.spb * NoteHelper.Whole * 2f);

        Debug.Log("Start Song Pos: " + currentSongPos + ", Ending Measure Spawning at " + EditorConductor.instance.spb * NoteHelper.Whole * 2f);

        while (currentSongPos < EditorConductor.instance.spb * NoteHelper.Whole * 2f)
        {
            if (currentSongPos < 0)
            {
                currentSongPos += EditorConductor.instance.spb;
                continue;
            }
            Debug.Log("Spawn Note At Timestamp: " + currentSongPos);
            SpawnSeperator(currentSongPos);
            currentSongPos += EditorConductor.instance.spb;
        }
    }

    private void SpawnSeperator(float songPosition)
    {
        float ratio = songPosition / (EditorConductor.instance.spb * NoteHelper.Whole * 2f);
        Vector3 spawnPosition = new Vector3(0f, BoardObject.transform.position.y + defaultDistanceFromHitboard * ratio * Mathf.Sin(xSpawnRotation) + 0.1f, BoardObject.transform.position.z + defaultDistanceFromHitboard * ratio * Mathf.Cos(xSpawnRotation));

        MeasureSeperator tmp = Instantiate(MSQuarter, spawnPosition, baseRotation).GetComponent<MeasureSeperator>();
        tmp.Construct(songPosition, basePosition);
        activeSeperators.Add(tmp);
    }

	// Update is called once per frame
	void Update () {
		
	}
}