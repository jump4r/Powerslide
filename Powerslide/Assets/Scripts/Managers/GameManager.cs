using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public Beatmap beatmap;
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
}
