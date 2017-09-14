using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public Beatmap beatmap;

    public static GameManager instance = null;
    public static float PlayerSpeedMult = 4f;

    void Awake ()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
	// Use this for initialization
	void Start () {
        // LoadBeatmapToConductor();
	}

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            LoadBeatmapToConductor();
        }
    }

        private void LoadBeatmapToConductor()
    {
        beatmap = GameObject.FindGameObjectWithTag("Beatmap").GetComponent<Beatmap>();
        if (beatmap != null)
        {
            GameObject.FindGameObjectWithTag("Conductor").GetComponent<Conductor>().LoadBeatmap(beatmap);
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
