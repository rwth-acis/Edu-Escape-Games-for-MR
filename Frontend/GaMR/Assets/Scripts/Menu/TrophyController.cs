using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;

public class TrophyController : MonoBehaviour, IFocusable, IInputClickHandler {

    // Plug in the heads up UI tag here
    public Canvas headsUpTag;

    public GameObject lostInSpaceBadge;
    public GameObject theCellBadge;

    // Camera object the UI tag should follow
    private Camera mrCamera;
    private bool trophyRoomOpened;
    
    void Start () {
        headsUpTag.transform.gameObject.SetActive(false);   // Disabled at the beginning
        mrCamera = Camera.main;                             // Set look at object to main camera
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
        if (trophyRoomOpened) {
            lostInSpaceBadge.SetActive(false);
            theCellBadge.SetActive(false);
            trophyRoomOpened = false;
        } else {
            if (PlayerPrefs.GetInt("LOST_IN_SPACE_BADGE", 0) == 1) {
                lostInSpaceBadge.SetActive(true);
            }
            if (PlayerPrefs.GetInt("THE_CELL_BADGE", 0) == 1) {
                theCellBadge.SetActive(true);
            }
            trophyRoomOpened = true;
        }
    }

    public void CloseTrophyRoom() {
        if (trophyRoomOpened) {
            lostInSpaceBadge.SetActive(false);
            theCellBadge.SetActive(false);
            trophyRoomOpened = false;
        }
    }
}
