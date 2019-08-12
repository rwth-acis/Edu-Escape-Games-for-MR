using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class LookAtController : MonoBehaviour, IInputClickHandler {

    // Configureable variables
    public float lookAtDistance;
    public float speed;

    public AudioSource takeAudio;
    public AudioSource putAudio;

    // Current state and orignal position and rotation
    private bool lookingAt = false;
    private Vector3 originalPosition;
    private Quaternion orignalRotation;

    //  For stopping animation after some frames
    private int flowAwayAnimationFrame = 300;
    private int duration = 300;

    /**
     * Save the original positon on start
     */
    void Start () {
        originalPosition = this.transform.position;
        orignalRotation = this.transform.rotation;
	}
	
	/**
     * If the user is looking at the object it always move in his
     * field of view. If not the docement move back to its original
     * place
     */
	void Update () {
		if (lookingAt) {
            // Calculate the desired position to look at
            Vector3 desiredPosition = Camera.main.transform.position + Camera.main.transform.forward * lookAtDistance;
            Quaternion desiredRotation = Camera.main.transform.rotation;

            // Move it to this position
            this.transform.position = Vector3.Lerp(transform.position, desiredPosition, speed * Time.deltaTime);
            this.transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, speed * Time.deltaTime);
        } else {
            if (flowAwayAnimationFrame < duration) {
                this.transform.position = Vector3.Lerp(transform.position, originalPosition, speed * Time.deltaTime);
                this.transform.rotation = Quaternion.Lerp(transform.rotation, orignalRotation, speed * Time.deltaTime);
                flowAwayAnimationFrame++;
            } else {
                this.transform.position = originalPosition;
                this.transform.rotation = orignalRotation;
            }
        }
	}

    /**
     * On click change the look at state
     */
    public void OnInputClicked(InputClickedEventData eventData) {
        if (lookingAt) {
            lookingAt = false;
            flowAwayAnimationFrame = 0;
            putAudio.Play();
            Debug.Log("Put look at object away");
        } else {
            QuestManager.GetInstance().currentlyWorkingOn(QuestManager.Quest.Document);
            lookingAt = true;
            takeAudio.Play();
            Debug.Log("Look at object");
        }
    }
}
