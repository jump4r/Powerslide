using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderBot : MonoBehaviour {


    public GameObject Slider;

    public NoteDrag activeNoteDrag = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (activeNoteDrag != null)
        {
            float relPos = activeNoteDrag.GetXRelPos();
            Slider.transform.position = new Vector3(relPos, Slider.transform.position.y, Slider.transform.position.z);
        }
	}
}
