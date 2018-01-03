using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatmapSelectable : MonoBehaviour {

    public string Artist { get; private set; }
    public string Name { get; private set; }

    public string path { get; private set; }

    [SerializeField]
    private Text artistText;

    [SerializeField]
    private Text songNameText;

    public RectTransform rTransform { get; set; }


    // Change Materials for Selected
    public Material BeatmapSelectedMat;
    public Material BeatmapSelectedBackgroundMat;
    public Material BeatmapDefaultMat;
    public Material BeatmapDefaultBackgroundMat;

    public Image BeatmapImage;
    public Image BeatmapBGImage;

    
	// Use this for initialization
	void Start () {
        
	}

    public void Initialize(string raw_input, string path,  Vector3 spawnPosition)
    {
        rTransform = GetComponent<RectTransform>();

        ParseRawInput(raw_input);

        artistText.text = Artist;
        songNameText.text = Name;
        this.path = path;

        rTransform.anchoredPosition = spawnPosition;
    }

    private void ParseRawInput(string raw_input)
    {
        string[] elements = raw_input.Split('-');
        
        if (elements.Length != 2)
        {
            return; 
        }

        Name = elements[0].Trim();
        Artist = elements[1].Trim();
    }

    public void SetEnabled(bool enabled)
    {
        this.gameObject.SetActive(enabled);
    }

    public void Pressed()
    {
        if (MenuManager.instance.passedScrollThreshold)
        {
            Debug.Log("Button presed, but passed scroll threshold");
            return;
        }

        if (BeatmapSelectMenu.instance.selectedBeatmap.Equals(this))
        {
            MenuManager.instance.EnableDifficultyModal(path);
        }

        else
        {
            BeatmapSelectMenu.instance.selectedBeatmap.SetUnselected();
            BeatmapSelectMenu.instance.selectedBeatmap = this;
            SetSelected();
            StartCoroutine(BeatmapSelectMenu.instance.AutoScroll(rTransform.anchoredPosition.y, 1f));
        }
    }

    public void Scroll(float distance)
    {
        rTransform.anchoredPosition = new Vector2(rTransform.anchoredPosition.x, rTransform.anchoredPosition.y + distance);
    }

    public void SetSelected()
    {
        BeatmapImage.color = BeatmapSelectedMat.color;
        BeatmapBGImage.color = BeatmapSelectedBackgroundMat.color;
    }

    public void SetUnselected()
    {
        BeatmapImage.color = BeatmapDefaultMat.color;
        BeatmapBGImage.color = BeatmapDefaultBackgroundMat.color;
    }


    public override string ToString()
    {
        return Artist + "  - " + Name;
    }

    // To do: Put a primary key in for beatmaps
    public bool Equals(BeatmapSelectable b)
    {
        if (b.Name == this.Name && b.Artist == this.Artist)
        {
            return true;
        }

        return false;
    }
}
