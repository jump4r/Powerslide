using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public Beatmap beatmap;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
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
}
