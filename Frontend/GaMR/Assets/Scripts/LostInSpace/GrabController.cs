using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class GrabController : MonoBehaviour, IFocusable, IInputClickHandler
{
    public GameObject userCamera;
    public bool isGrabable;

    private bool isGrabed;


    // Use this for initialization
    void Start () {
        isGrabed = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnFocusEnter()
    {
        throw new System.NotImplementedException();
    }

    public void OnFocusExit()
    {
        throw new System.NotImplementedException();
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
