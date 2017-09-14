using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    // Singleton
    public static ScoreManager instance = null;

    // Score and Accuracy Markers
    public static float Accuracy = 1.00f;
    public static int TotalNotes = 0;
    public static float PlayerHitNotes = 0f;
    public static int Score = 0;

    // UI Objects
    public Text AccuracyText;
    public Text ScoreText;

    // We only want a single Score Manager
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            ScoreText = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<Text>();
            AccuracyText = GameObject.FindGameObjectWithTag("AccuracyText").GetComponent<Text>();
            if (ScoreText == null || AccuracyText == null)
            {
                Debug.Log("Error Loading Score/Accuracy UI Elements");
            }
        }

        // Display the results on the results screen, really I should make a score manager....
        else if (scene.buildIndex == 3)
        {
            GameObject.Find("Score").GetComponent<Text>().text = Score.ToString("########0");
            GameObject.Find("Accuracy").GetComponent<Text>().text = Accuracy.ToString("p");
        }

        else
        {
            Debug.Log("Current Build Index: " + scene.buildIndex);
        }
    }

    public static void ResetScoreManager()
    {
        Accuracy = 100f;
        Score = 0;
        TotalNotes = 0;
        PlayerHitNotes = 0f;
    }

    public void UpdateScore(int noteScore)
    {
        Score += noteScore;
        string rtn = Score.ToString("0000000");
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

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
