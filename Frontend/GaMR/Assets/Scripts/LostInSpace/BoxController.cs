using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;


/**
 * Should only be apply to the replacement part box. Handles the open/close
 * animations of the box.
 */
public class BoxController : MonoBehaviour, IInputClickHandler {
    
    private bool isOpened;
    private Animator anim;
    
    /**
     * On start get the animator and set the box opening configuration to closed (false)
     */
    void Awake () {
        anim = gameObject.GetComponent<Animator>();
        isOpened = false;
    }

    /**
     * Change the state evertime the box is clicked. Use crossfade for 
     * a smooth transition if the box is clicked fast.
     */
    public void OnInputClicked(InputClickedEventData eventData) {
        if (isOpened) { // If it's open close it
            anim.CrossFade("CloseBox", 0.5f);
            isOpened = false;
            Debug.Log("Close box");
        } else {        // If it's closed open it
            anim.CrossFade("OpenBox", 0.5f);
            isOpened = true;
            Debug.Log("Open box");
        }
    }
}
