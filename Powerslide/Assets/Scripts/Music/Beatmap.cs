using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;
using System;

public class Beatmap : MonoBehaviour {

    [SerializeField]
    private string beatmapFilename;
    private string raw_beatmap;
    private List<string> beatmapSplitText;

    // General
    public string SongName = "";
    public AudioClip song;
    public int OsuOffset;
    public int RawOffset;
    public int OffsetDifference; // Because we are using osu to generate maps, which will sometimes trim audio files, we must get the real offset from a program like audiacity, and then add the offset difference to each HitObject
    public float BPM;

    public List<string> Notes;

    // Use this for initialization
    void Start() {
        beatmapSplitText = new List<string>();
        Notes = new List<string>();

        bool beatmapLoaded = LoadFile(beatmapFilename);
        if (beatmapLoaded)
        {
            ParseBeatmap();
            GameObject.FindGameObjectWithTag("Conductor").GetComponent<Conductor>().LoadBeatmap(this);
        }
    }

    // To do, find a way to load a file on Android without making said object a TextAsset
    /* Tried list
     * Application.persistantDataPath -> useful for when we want players to write & save, however, not so much at this point
     * Application.dataPath does not point to the right location on android
     * */
    private bool LoadFile(string filename)
    {
        TextAsset beatmapTextAsset = Resources.Load("Beatmaps/" + filename) as TextAsset;
        // Debug.Log("Beatmap Text: " + beatmapTextAsset.text); // I'm sure this is a horrible way to do this.
        // string file = Path.GetFullPath(Application.dataPath + "\\Resources\\Beatmaps\\" + filename + ".txt");

        // Load beatmap
        string file = beatmapTextAsset.text;
        beatmapSplitText = beatmapTextAsset.text.Split('\n').ToList();
        
        if (beatmapSplitText.Count == 0)
        {
            Debug.Log("Error Loading Beatmap");
            return false;
        }
        
        for (int i = 0; i < beatmapSplitText.Count; i++)
        {
            beatmapSplitText[i] = beatmapSplitText[i].Trim();
        }

        // Debug.Log("Beatmap Split Text Count: " + beatmapSplitText[0]);
        return true;
        /*
        try
        {
            string line;
            StreamReader reader = new StreamReader(file, Encoding.Default);
            using (reader)
            {
                do
                {
                    line = reader.ReadLine();
                    if (line != null)
                    {
                        beatmapSplitText.Add(line);
                    }
                }
                while (line != null);
                reader.Close();
                Debug.Log("Loaded Beatmap, number of lines: " + beatmapSplitText.Count);
                return true;
            }
        }

        catch (Exception e)
        {
            Console.WriteLine("{0}\n", e.Message);
            Debug.Log("Error Loading Beatmap");
            return false;
        }
        */
    }

    // Load up the variables from the beatmap file.
    private void ParseBeatmap()
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
                    // Debug.Log("Adding Hit Object To List");
                    Notes.Add(beatmapSplitText[i+1]);
                    i++;
                }
            }
            else { /* Debug.Log(beatmapSplitText[i]); */ }
        }

        LoadAudio(SongName);
    }

    private string GetInfoFromLine(string line)
    {
        string rtn = line.Split(':')[1].Trim();
        // Debug.Log("Returning: " + rtn);
        return rtn;
    }

    private void LoadAudio(string filename)
    {
        song = Resources.Load("Sound FX/Music/" + filename) as AudioClip;
        if (song == null)
        {
            // Debug.Log("Loading Failed!");
            return;
        }

        // Debug.Log("Load Successful");
    }
}
