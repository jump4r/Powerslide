using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EditorBeatmap {

    // General
    public string SongName = "";
    public AudioClip song;
    public int OsuOffset;
    public int RawOffset;
    public int OffsetDifference; // Because we are using osu to generate maps, which will sometimes trim audio files, we must get the real offset from a program like audiacity, and then add the offset difference to each HitObject
    public float BPM;

    // Notes List
    public List<string> Notes;

    public EditorBeatmap(string beatmapText)
    {
        List<string> beatmapSplitText = beatmapText.Split('\n').ToList();
        Notes = new List<string>();

        for (int i = 0; i < beatmapSplitText.Count; i++)
        {
            if (string.Compare(beatmapSplitText[i].Trim(), "[General]") == 0)
            {
                SongName = GetInfoFromLine(beatmapSplitText[i + 1]).Trim();
                OsuOffset = int.Parse(GetInfoFromLine(beatmapSplitText[i + 2]).Trim());
                RawOffset = int.Parse(GetInfoFromLine(beatmapSplitText[i + 3]).Trim());
                OffsetDifference = RawOffset - OsuOffset;
                BPM = float.Parse(GetInfoFromLine(beatmapSplitText[i + 4]).Trim());
            }
            // Load up the Hit Objects in thew Notes list
            if (string.Compare(beatmapSplitText[i].Trim(), "[HitObjects]") == 0)
            {
                while (i + 1 < beatmapSplitText.Count)
                {
                    Notes.Add(beatmapSplitText[i + 1]);
                    i++;
                }
            }
        }

        LoadAudio(SongName);
    }

    private string GetInfoFromLine(string line)
    {
        return line.Split(':')[1].Trim();
    }

    private void LoadAudio(string filename)
    {
        song = Resources.Load("Sound FX/Music/" + filename) as AudioClip;
        Debug.Log(filename);
        if (song == null)
        {
            Debug.Log("Song Didn't Load Correctly");
            return;
        }
    }
}
