using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Even though I added this...
using System.IO;

public enum Screen
{
    MAIN = 0,
    SONGSELECT = 1,
    SETTINGS = 2,
    TUTORIAL = 3,
}

public enum LevelEnum
{
    SELECT = 0,
    PLAYGROUND = 1,
    RESULTS = 2,
}

public class LevelManager : MonoBehaviour {

    public Dictionary<Screen, GameObject> ScreenDictionary;

    public GameObject LoadingScreen;
    public GameObject MainMenuScreen;
    public GameObject SettingsScreen;
    public GameObject SongSelectScreen;

    public GameObject DifficultySelectModal;

    public static LevelManager instance = null;

    private Screen currentScreen = Screen.MAIN;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
        }

        ScreenDictionary = new Dictionary<Screen, GameObject>();
        ScreenDictionary.Add(Screen.MAIN, MainMenuScreen);
        ScreenDictionary.Add(Screen.SETTINGS, SettingsScreen);
        ScreenDictionary.Add(Screen.SONGSELECT, SongSelectScreen);
    }

    public void ChangeLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex); // Unity buggy asf..
    }

    public void ChangeLevel(LevelEnum level)
    {
        int levelIndex = (int)level;

        Debug.Log("Windows Debug: Loading Level " + levelIndex);

        SceneManager.LoadScene(levelIndex);
    }

    // Managing Screens
    public void ChangeMenuScreen(int newScreen)
    {
        ScreenDictionary[currentScreen].SetActive(false);

        currentScreen = (Screen)newScreen;

        ScreenDictionary[currentScreen].SetActive(true);
    }

    public void SetLoadingScreen(bool status)
    {
        LoadingScreen.SetActive(status);
    }

    // Pathing may need to change for android
    public void EnableDifficultyModal(string path)
    {
        Debug.Log(path);
        DirectoryInfo info = new DirectoryInfo(path);
        foreach (FileInfo file in info.GetFiles("*txt"))
        {
            Debug.Log(file.Name);
        }

        DifficultySelectModal.SetActive(true);
    }

    public void DisableDifficultyModal()
    {
        DifficultySelectModal.SetActive(false);
    }

    public void ResetPlayground()
    {
        Destroy(GameObject.FindGameObjectWithTag("Beatmap"));
    }
    
    // For now, we can just destroy it.
    public void ResetGameManager()
    {
        Destroy(GameObject.FindGameObjectWithTag("GameManager"));
    }

    public void ResetScoreManager()
    {
        ScoreManager.instance.ResetScoreManager();
    }
}
