using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;
using System.Linq;
using System;

public enum BeatmapState
{
    Empty = 0,
    Selected = 1,
    Loading = 2,
    Loaded = 3
}

public class Beatmap : MonoBehaviour {

    private BeatmapState state = BeatmapState.Empty;
    private bool beatmapLoaded = false;

    [SerializeField]
    private string beatmapFilename { get; set; }
    [SerializeField]
    private string beatmapDirectory { get; set; }

    private string raw_beatmap { get; set; }
    private List<string> beatmapSplitText;

    // General
    public string SongName = "";
    public AudioClip song;
    public int OsuOffset { get; private set; }
    public int RawOffset { get; private set; }
    public int OffsetDifference; // Because we are using osu to generate maps, which will sometimes trim audio files, we must get the real offset from a program like audiacity, and then add the offset difference to each HitObject
    public float BPM;

    // Notes List
    public List<string> Notes;

    public void SetFilename(string beatmapFilename)
    {
        this.beatmapFilename = beatmapFilename;
    }

    public void SetDirectoryName(string name)
    {
        this.beatmapDirectory = name;
    }

    // Use this for initialization
    void Start() {
        beatmapSplitText = new List<string>();
        Notes = new List<string>();
    }

    private void Update()
    {
        if (state == BeatmapState.Loading)
        {
            Debug.Log("Loading Beatmap");
        }
    }

    // To do, find a way to load a file on Android without making said object a TextAsset
    /* Tried list
     * Application.persistantDataPath -> useful for when we want players to write & save, however, not so much at this point
     * Application.dataPath does not point to the right location on android
     * */
    private IEnumerator LoadFile(string filename)
    {
        state = BeatmapState.Loading;

        WWW beatmapWWW = new WWW("file://" + filename);
        yield return beatmapWWW;

        // Load beatmap
        string file = beatmapWWW.text;

        beatmapSplitText = file.Split('\n').ToList();

        bool successfulLoad = beatmapSplitText.Count > 0 ? true : false;

        if (!successfulLoad)
        {
            Debug.LogWarning("Failed to load beatmap file");
            yield return false;
        }

        else
        {
            for (int i = 0; i < beatmapSplitText.Count; i++)
            {
                beatmapSplitText[i] = beatmapSplitText[i].Trim();
            }

            state = BeatmapState.Loaded;

            ParseBeatmap();

            yield return LoadAudio(beatmapDirectory);

            if (song != null)
            {
                 LevelManager.instance.ChangeLevel(LevelEnum.PLAYGROUND);
            }

            else
            {
                Debug.LogWarning("Failed to load audio");
            }
        }
    }

    // Beatmap has been selected for loading into the playground
    public void BeatmapSelected()
    {
        LevelManager.instance.SetLoadingScreen(true);
        gameObject.transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        StartCoroutine(LoadFile(beatmapFilename));
    }

    // Load up the variables from the beatmap file.
    public void ParseBeatmap()
    {
        // beatmapSplitText = raw_beatmap.Split('\n');
        for (int i = 0; i < beatmapSplitText.Count; i++)
        {
            if (string.Compare(beatmapSplitText[i], "[General]") == 0)
            {
                SongName = GetInfoFromLine(beatmapSplitText[i + 1]);
                OsuOffset = int.Parse(GetInfoFromLine(beatmapSplitText[i + 2]));
                RawOffset = int.Parse(GetInfoFromLine(beatmapSplitText[i + 3]));
                OffsetDifference = RawOffset - OsuOffset;
                BPM = float.Parse(GetInfoFromLine(beatmapSplitText[i + 4]));
            }
            // Load up the Hit Objects in thew Notes list
            if (string.Compare(beatmapSplitText[i], "[HitObjects]") == 0)
            {
                // Debug.Log("Get Hit Objects:");
                while (i + 1 < beatmapSplitText.Count)
                {
                    if (beatmapSplitText[i + 1].Trim() == "")
                    {
                        i++;
                        continue;
                    }

                    // Debug.Log("Adding Hit Object To List");
                    Notes.Add(beatmapSplitText[i+1]);
                    i++;
                }
            }
            else { /* Debug.Log(beatmapSplitText[i]); */ }
        }
    }

    private string GetInfoFromLine(string line)
    {
        string rtn = line.Split(':')[1].Trim();
        // Debug.Log("Returning: " + rtn);
        return rtn;
    }

    private IEnumerator LoadAudio(string directory)
    {

        FileInfo[] files;

        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            files = new DirectoryInfo(directory).GetFiles("*.ogg");
            Debug.Log("Windows Debug: Loading OGG File");
        }

        else
        {
            files = new DirectoryInfo(directory).GetFiles("*.mp3");
        }

        if (files.Length < 1)
        {
            Debug.Log("No mp3 files found in this directory");
        }

        var songWWW = new WWW("file://" + files[0].FullName);
        song = songWWW.GetAudioClip(true);

        while (song.loadState == AudioDataLoadState.Unloaded || song.loadState == AudioDataLoadState.Loading)
        {
            yield return new WaitForEndOfFrame();
        }
    }
}
