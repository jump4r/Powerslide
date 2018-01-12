using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BeatmapSelectMenu : MonoBehaviour {

    public static BeatmapSelectMenu instance = null;

    [SerializeField]
    private RectTransform _canvasRect;

    [SerializeField]
    private GameObject BeatmapSelectablePrefab;

    private List<BeatmapSelectable> beatmaps;
    private List<BeatmapSelectable> currentActiveBeatmaps; // Going to use later to streamline code.

    public BeatmapSelectable selectedBeatmap;

    [SerializeField]
    [Range(900f, 1100f)]
    private float yMaxDisplayHeight = 1000f;

    private float yHeight;

    [Range(250f, 350f)]
    public readonly float ySpawnOffset = 250f;

	// Use this for initialization
	void Start () {
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(this.gameObject);
        }

        yHeight = _canvasRect.rect.height;

        beatmaps = new List<BeatmapSelectable>();

        Debug.Log("Android Debug: Start Coroutine For Loading Beatmaps");

        StartCoroutine(LoadBeatmapSelectionAsync());
	}

    public IEnumerator LoadBeatmapSelectionAsync()
    {

        string path = null;
#if UNITY_EDITOR
        if (Application.isEditor)
        {
            path = Application.dataPath + "/Resources/Beatmaps";
        }
#endif


#if UNITY_ANDROID
        if  (Application.platform == RuntimePlatform.Android)
        {
            path = Application.persistentDataPath + "/Beatmaps";
        }
#endif

        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Debug.Log("Windows Datapath: " + Application.dataPath);
            path = Application.dataPath + "/Beatmaps";
        }

        DirectoryInfo[] directories = new DirectoryInfo(path).GetDirectories();

        for (int i = 0; i < directories.Length; i++)
        {
            BeatmapSelectable map = Instantiate(BeatmapSelectablePrefab).GetComponent<BeatmapSelectable>();
            map.transform.SetParent(this.transform);
            map.transform.SetAsFirstSibling();

            Vector3 spawnPosition = new Vector3(0, ySpawnOffset * -i, 0);
            map.Initialize(directories[i].Name, path + "/" + directories[i].Name, spawnPosition);


            beatmaps.Add(map);
        }

        if (beatmaps.Count > 0)
        {
            selectedBeatmap = beatmaps[0];
        }

        else
        {
            Debug.LogWarning("Warning: No Beatmaps Loaded", this.transform);
        }
        yield return true;
    }

    public void ScrollBeatmaps(float distance)
    {
        // We shouldn't scroll if we've reached the end on either side
        if (distance > 0 && selectedBeatmap.Equals(beatmaps[beatmaps.Count - 1]) || (distance < 0 && selectedBeatmap.Equals(beatmaps[0]))) {
            return;
        }

        foreach (BeatmapSelectable b in beatmaps)
        {
            b.Scroll(distance);
        }

        RecalculateSelectedBeatmap();
    }

    public IEnumerator AutoScroll(float startY, float time)
    {
        float elapsedTime = 0f;

        if (selectedBeatmap == null)
        {
            yield return null;
        }

        int beatmapIndex = FindBeatmapIndex(selectedBeatmap);

        while (elapsedTime < time)
        {
            for (int i = 0; i < beatmaps.Count; i++)
            {
                float percentage = (elapsedTime / time);
                float newY = Mathf.Lerp(startY + (beatmapIndex - i) * ySpawnOffset, (beatmapIndex - i) * ySpawnOffset, percentage);
                beatmaps[i].rTransform.anchoredPosition = new Vector2(0, newY);
                elapsedTime += Time.deltaTime;
            }
            // Debug.Log("Time: " + elapsedTime + "/" + time);
            yield return new WaitForEndOfFrame();
        }
    }

    private void RecalculateSelectedBeatmap()
    {
        int index = FindBeatmapIndex(selectedBeatmap);

        if (index < 0)
        {
            return;
        }

        if (beatmaps[index].rTransform.anchoredPosition.y > 300f)
        {
            selectedBeatmap.SetUnselected();
            selectedBeatmap = beatmaps[index + 1];
            selectedBeatmap.SetSelected();
        }

        else if (beatmaps[index].rTransform.anchoredPosition.y < -300f)
        {
            selectedBeatmap.SetUnselected();
            selectedBeatmap = beatmaps[index - 1];
            selectedBeatmap.SetSelected();
        }
    }

    private int FindBeatmapIndex(BeatmapSelectable b)
    {
        for (int i = 0; i < beatmaps.Count; i++)
        {
            if (beatmaps[i].Equals(b))
            {
                return i;
            }
        }

        return -1;
    }

    public void EnableDifficultySelector()
    {
        string data_path = selectedBeatmap.path;
    }

}
