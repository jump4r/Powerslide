using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Even though I added this...

public class LevelManager : MonoBehaviour {
    
    public void ChangeLevel(int levelIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelIndex); // Unity buggy asf..
    }

    // Trying to specific
    public void LoadGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void ResetPlayground()
    {
        Destroy(GameObject.FindGameObjectWithTag("Beatmap"));
        GameManager.ResetGameManager();
    }
}
