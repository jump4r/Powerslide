using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;

public class DifficultyModal : MonoBehaviour {


    // Total Height Formula
    // 250 (Height of TitleBar) + (150 * numButtons) + (50 * (numButtons))

    private RectTransform rTransform;

    [SerializeField]
    private GameObject BeatmapButton;

    private List<GameObject> BeatmapButtons;

    // Use this for initialization
    private void OnEnable()
    {
        BeatmapButtons = new List<GameObject>();
        rTransform = GetComponent<RectTransform>();
    }

    private void OnDisable()
    {
        foreach (GameObject go in BeatmapButtons)
        {
            Destroy(go);
        }

        BeatmapButtons.Clear();
    }

    public void Initialize(FileInfo[] beatmapFiles)
    {
        rTransform.sizeDelta = new Vector2(rTransform.rect.width, 250 + (200 * beatmapFiles.Length) + 50);

        for (int i = 0; i < beatmapFiles.Length; i++)
        {
            GameObject button = Instantiate(BeatmapButton);
            button.transform.SetParent(this.gameObject.transform);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -350 + (i * -200), 0); // Anchored top
            button.GetComponentInChildren<Beatmap>().SetFilename(beatmapFiles[i].FullName);
            button.GetComponentInChildren<Beatmap>().SetDirectoryName(beatmapFiles[i].Directory.FullName);
            button.transform.Find("DifficultyText").GetComponent<Text>().text = Regex.Match(beatmapFiles[i].Name, @"\(([^)]*)\)").Groups[1].Value;
            BeatmapButtons.Add(button);
        }
    }

    private string ParseDifficultyFromFile(string file)
    {
        return file;
    }
}
