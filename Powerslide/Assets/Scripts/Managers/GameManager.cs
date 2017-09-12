using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public Beatmap beatmap;

    // Score and Accuracy Markers
    public static float Accuracy = 1.00f;
    public static int TotalNotes = 0;
    public static float PlayerHitNotes = 0f;
    public static int Score = 0;

    // UI Objects
    public Text AccuracyText;
    public Text ScoreText;

	// Use this for initialization
	void Start () {
        LoadBeatmapToConductor();
	}
	
    private void LoadBeatmapToConductor()
    {
        beatmap = GameObject.FindGameObjectWithTag("Beatmap").GetComponent<Beatmap>();
        if (beatmap != null)
        {
            GameObject.FindGameObjectWithTag("Conductor").GetComponent<Conductor>().LoadBeatmap(beatmap);
        }
    }

    public static void ResetGameManager()
    {
        Accuracy = 100f;
        Score = 0;
        TotalNotes = 0;
        PlayerHitNotes = 0f;
    }

    public void UpdateScore(int noteScore)
    {
        Score += noteScore;
        string rtn = Score.ToString("######00");
        Debug.Log("Testing String: " + rtn);
        ScoreText.text = rtn;
    }

    public void UpdateAccuracy(float noteAccuracy)
    {
        TotalNotes++;
        PlayerHitNotes += noteAccuracy;
        Accuracy = PlayerHitNotes / TotalNotes;
        Debug.Log("Testing Accuracy: " + PlayerHitNotes + " / " + TotalNotes);
        Debug.Log("Testing Accuracy: " + Accuracy.ToString("p"));
        AccuracyText.text = Accuracy.ToString("p");
    }
}
