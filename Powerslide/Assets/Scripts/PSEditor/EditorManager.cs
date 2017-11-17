using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class EditorManager : MonoBehaviour {

    private bool loading = false;
    public WWW beatmapWWW;
    public EditorBeatmap beatmap;

    public void OpenBeatmapFile()
    {
        string beatmapFilePath = EditorUtility.OpenFilePanel("Select Beatmap", ".", "txt");
        if (beatmapFilePath.Length != 0)
        {
            loading = true;
            StartCoroutine(LoadFromFile(beatmapFilePath));
            StartCoroutine(ParseBeatmap());
        }
        return;
    }

    private IEnumerator LoadFromFile(string beatmapFilePath)
    {
        Debug.Log("Start Loading");
        this.beatmapWWW = new WWW("file://" + beatmapFilePath);
        loading = false;
        yield return 0;
    }

    private IEnumerator ParseBeatmap()
    {
        while (loading)
        {
            yield return new WaitForSeconds(0.1f);
        }

        string beatmapText = beatmapWWW.text;
        Debug.Log("Loading Complete: " + beatmapText);
        beatmap = new EditorBeatmap(beatmapText);
        EditorConductor.instance.Construct(beatmap);
        EditorConductor.instance.Play();
    }
}
