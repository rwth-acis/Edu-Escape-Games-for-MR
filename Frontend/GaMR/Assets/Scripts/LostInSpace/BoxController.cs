using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using HoloToolkit.Unity.InputModule;


/**
 * Should only be apply to the replacement part box. Handles the open/close
 * animations of the box.
 */
public class BoxController : MonoBehaviour, IInputClickHandler {

    public AudioSource boxOpenAudioSource;
    public AudioSource boxCloseAudioSource;

    private bool isOpened;
    private Animator anim;

    // Variable for changing the collider when the box opens/closes
    private BoxCollider mCollider;
    private Vector3 closeCenter;
    private Vector3 openCenter;
    private Vector3 closeScale;
    private Vector3 openScale;
    
    /**
     * On start get the animator and set the box opening configuration to closed (false)
     */
    void Awake () {
        anim = gameObject.GetComponent<Animator>();
        isOpened = false;

        mCollider = GetComponent<BoxCollider>();
        closeScale = mCollider.size;            // Save the original size and center
        closeCenter = mCollider.center;
        openScale = new Vector3(closeScale.x, closeScale.y * 0.5f, closeScale.z);   // Calculate the size and center for opened box
        openCenter = new Vector3(closeCenter.x, closeCenter.y - 0.25f * closeScale.y, closeCenter.z);

        Debug.Log("Box Object started");
    }

    /**
     * Change the state evertime the box is clicked. Use crossfade for 
     * a smooth transition if the box is clicked fast.
     */
    public void OnInputClicked(InputClickedEventData eventData) {
        if (isOpened) { // If it's open close it
            // Start close animation
            anim.CrossFade("CloseBox", 0.5f);

            // Play Audio feedback
            boxCloseAudioSource.Play();
            
            // Update the collider to closed size
            mCollider.size = closeScale;
            mCollider.center = closeCenter;

            isOpened = false;
            Debug.Log("Close box");
        } else {        // If it's closed open it
            // Start open animation
            anim.CrossFade("OpenBox", 0.5f);

            // Play Audio feedback
            boxOpenAudioSource.Play();
            
            // Update the collider to opened size
            mCollider.size = openScale;
            mCollider.center = openCenter;

            isOpened = true;
            Debug.Log("Open box");
        }

        QuestManager.GetInstance().currentlyWorkingOn(QuestManager.Quest.BrokenFuse);
    }
}
