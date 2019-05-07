using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbar : Tappable {

    public static Hitbar MyHitbar; // Horrible name


    // Use this for initialization
    void Start()
    {
        if (!MyHitbar)
        {
            MyHitbar = this;
        }

        else
        {
            Destroy(this);
        }
    }

    public void Tapped()
    {
        SetRenderMaterial(pathColorPressed);

        if (ActiveNotes.Count <= 0) // no active notes
        {
            return;
        }
        
        // To do, is tap position close enough to call it a successfull tap?

        ActiveNotes[0].Tapped();
    }

    public void Held()
    {
        if (ActiveNotes.Count <= 0) // no active notes
        {
            return;
        }

        // Debug.Log("Android Debug: Held (NotePath");
        ActiveNotes[0].Held();
    }

    // Finger lifted from the screen
    public void Lifted()
    {
        SetRenderMaterial(pathColorDefault);

        if (ActiveNotes.Count <= 0)
        {
            return;
        }

        ActiveNotes[0].Lift();
    }
}
