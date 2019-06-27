using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class CableModification : MonoBehaviour, IFocusable, IInputHandler, IInputClickHandler {
    

    // Use this for initialization
    void Start () {
        Debug.Log("Started script");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnFocusEnter()
    {
        Debug.Log("Entered Focus");
    }

    public void OnFocusExit()
    {
        Debug.Log("Left Focus");
    }

    public void OnInputDown(InputEventData eventData)
    {
        Debug.Log("Input is down");
    }

    public void OnInputUp(InputEventData eventData)
    {
        Debug.Log("Input is up");
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log("Input was clicked");
    }
}
