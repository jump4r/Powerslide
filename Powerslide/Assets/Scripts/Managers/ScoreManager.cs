using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager instance = null;

    // Score and Accuracy Markers
    public float Accuracy = 1.00f;
    public int TotalNotes = 0;
    public float PlayerHitNotes = 0f;
    public int Score = 0;

    private string letterGrade { get; set; }

    // Combo
    public int currentCombo = 0;
    private int maxCombo { get; set; }
    private int numMisses = 0;

    // UI Objects
    public Text AccuracyText;
    public Text ScoreText;
    public Text ComboText;



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

        DontDestroyOnLoad(this.gameObject);
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
            ComboText = GameObject.FindGameObjectWithTag("ComboText").GetComponent<Text>();

            if (ScoreText == null || AccuracyText == null)
            {
                Debug.LogWarning("Error Loading Score/Accuracy UI Elements");
            }
        }

        // Display the results on the results screen, really I should make a score manager....
        else if (scene.buildIndex == 2)
        {
            GameObject.Find("Score").GetComponent<Text>().text = Score.ToString("000000000");
            GameObject.Find("Accuracy").GetComponent<Text>().text = Accuracy.ToString("p");
            GameObject.Find("LetterGrade").GetComponent<Text>().text = CalculateGrade();
        }
    }

    public void ResetScoreManager()
    {
        Accuracy = 100f;
        Score = 0;
        TotalNotes = 0;
        PlayerHitNotes = 0f;
    }

    public void UpdateScore(int noteScore)
    {
        Score += noteScore;
        string rtn = Score.ToString("000000000");
        ScoreText.text = rtn;
    }

    public void UpdateAccuracy(float noteAccuracy)
    {
        TotalNotes++;
        PlayerHitNotes += noteAccuracy;
        Accuracy = PlayerHitNotes / TotalNotes;
        //Debug.Log("Testing Accuracy: " + PlayerHitNotes + " / " + TotalNotes);
        //Debug.Log("Testing Accuracy: " + Accuracy.ToString("p"));
        AccuracyText.text = Accuracy.ToString("p");

        if (noteAccuracy != 0)
        {
            AddToCombo();
        }

        else
        {
            ResetCombo();
        }
    }

    public void AddToCombo()
    {
        currentCombo++;
        
        if (currentCombo > maxCombo)
        {
            maxCombo = currentCombo;
        }

        UpdateCombo();
    }

    public void ResetCombo()
    {
        currentCombo = 0;

        UpdateCombo();
    }

    public void UpdateCombo()
    {
        ComboText.text = "Combo: " + currentCombo.ToString();
    }

    private string CalculateGrade()
    {
        if (Accuracy > 0.98f)
        {
            return "S";
        }

        else if (Accuracy > 0.92f)
        {
            return "A";
        }

        else if (Accuracy > 0.85f)
        {
            return "B";
        }

        else if (Accuracy > 0.75f)
        {
            return "C";
        }

        else
        {
            return "D";
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
