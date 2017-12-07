using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Even though I added this...

public class LevelManager : MonoBehaviour {

    public static LevelManager instance = null;

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
    }

    public void ChangeLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex); // Unity buggy asf..
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
        ScoreManager.ResetScoreManager();
    }
}
