using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;

public class TrophyController : MonoBehaviour, IFocusable, IInputClickHandler {

    // Plug in the heads up UI tag here
    public Canvas headsUpTag;

    // Camera object the UI tag should follow
    private Camera mrCamera;
    
    void Start () {
        headsUpTag.transform.gameObject.SetActive(false);   // Disabled at the beginning
        mrCamera = Camera.main;                             // Look at main camera
	}
	
    /**
     * When the game object is focused look at main camera continuously
     */
	void Update () {
		if (headsUpTag.transform.gameObject.activeSelf) {
            headsUpTag.transform.LookAt(mrCamera.transform);
        }
	}

    /**
     * When the game object gets focused display the heads up tag and correct
     * the viewing angle
     */
    public void OnFocusEnter() {
        headsUpTag.transform.LookAt(mrCamera.transform);
        headsUpTag.transform.gameObject.SetActive(true);
    }

    /**
     * When the game object lost focus just disable the heads up tag
     */
    public void OnFocusExit() {
        headsUpTag.transform.gameObject.SetActive(false);
    }

    public void OnInputClicked(InputClickedEventData eventData) {
        throw new System.NotImplementedException();
    }
}
