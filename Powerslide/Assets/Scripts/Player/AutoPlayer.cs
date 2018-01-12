using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPlayer : MonoBehaviour {

    public static AutoPlayer instance = null;

    public List<AutoFinger> aFingers;

    public SliderBot sBot;

	// Use this for initialization
	void Start () {
        if (instance == null)
        {
            instance = this;
        }

        aFingers = new List<AutoFinger>();

        aFingers.Add(new AutoFinger(0));
        aFingers.Add(new AutoFinger(1));
        aFingers.Add(new AutoFinger(2));
        aFingers.Add(new AutoFinger(3));

        sBot = GetComponent<SliderBot>();
        sBot.enabled = true;
    }
	
	// Update is called once per frame
	void Update () {
		foreach (AutoFinger f in aFingers)
        {
            if (f.activeNote == null)
            {
                f.CheckToAddNote();
            }

            else
            {
                // The player takes longer to activate flick notes, that should be reflected in the bot
                /*
                if (f.activeNote.rTP > 0.93f && !f.activated && f.activeNote.type == NoteType.Flick)
                {
                    StartCoroutine(f.noteAction());
                }*/

                // Debug.Log("Note in lane " + f.notePathID + " has rTP " + f.activeNote.rTP);
                if (f.activeNote.rTP > 0.98f && !f.activated)
                {
                    StartCoroutine(f.noteAction());
                }
            }
        }
	}
}

public class AutoFinger
{
    public int notePathID;

    public NoteBase activeNote = null;

    public bool activated = false; // Is the AutoFinger current engaged in an action?

    public delegate IEnumerator NoteAction();
    public NoteAction noteAction;

    public AutoFinger(int notePathID)
    {
        this.notePathID = notePathID;
    }

    public void CheckToAddNote()
    {
        if (NotePath.NotePaths[notePathID].ActiveNotes.Count > 0 && activeNote == null)
        {
            
            if (NotePath.NotePaths[notePathID].ActiveNotes[0].type == NoteType.Drag)
            {
                AutoPlayer.instance.sBot.activeNoteDrag = NotePath.NotePaths[notePathID].ActiveNotes[0] as NoteDrag;
                return;
            }

            activeNote = NotePath.NotePaths[notePathID].ActiveNotes[0];
            
            if (activeNote.type == NoteType.Regular)
            {
                noteAction = TapNote;
            }

            else if (activeNote.type == NoteType.Hold)
            {
                 noteAction = HoldNote;
            }

            else if (activeNote.type == NoteType.Flick)
            {
                noteAction = FlickNote;
            }
        } 
    }

    // Tap the note, return is so 'hold' can be used.
    public IEnumerator TapNote()
    {
        NotePath.NotePaths[notePathID].Tapped();

        activeNote = null;

        yield return new WaitForSeconds(0.08f);

        NotePath.NotePaths[notePathID].Lifted();
    }

    public IEnumerator HoldNote()
    {
        activated = true;
        // activeNote.IsTapped = true;
        NotePath.NotePaths[notePathID].Tapped();

        while (activeNote.isReadyToHit && activeNote != null)
        {
            NotePath.NotePaths[notePathID].Held();
            yield return true;
        }

        activeNote = null;
        activated = false;

        NotePath.NotePaths[notePathID].Lifted();
        yield return false;
    }

    public IEnumerator FlickNote()
    {
        int endPath = activeNote.endPath;
        NotePath.NotePaths[notePathID].Transitioned(endPath);
        activeNote = null;
        yield return false;
    }
}