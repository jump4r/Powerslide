using UnityEngine;
using System.Collections;

public class NoteFlick : NoteBase {

    public char direction; // Swipe left or right

	// Use this for initialization
	void OnEnable () {
        // TODO: Not really sure yet, but make this work with object pooling
        type = NoteType.Flick;
	}

    public override void ChangeMaterial()
    {
        if (noteValue > 50)
        {
            gameObject.GetComponent<Renderer>().material = Score100;
        }

        else if (noteValue > 0)
        {
            gameObject.GetComponent<Renderer>().material = Score50;
        }
    }
}
