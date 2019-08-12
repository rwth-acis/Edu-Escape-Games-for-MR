using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

/**
 * Makes a game object moveable by a click on it. If the object is clicked onces 
 * the game object stays in front of the camera. Another click removes the grabbed
 * property and the object stays at the current position.
 */
public class GrabController : MonoBehaviour, IInputClickHandler
{
    public bool isMoveable;         // Configures if the object is currently moveable
    public float grabbedDistance;   // Configures the distance of the object to the camera when grabbed

    public AudioSource takeAudio;
    public AudioSource placeAudio;

    private bool isGrabbed;         // Is the object currently grabbed
    private readonly float speed = 2.5f;
    
    /**
     * On start the grabbed property is set to false.
     */
    void Start () {
        isGrabbed = false;
    }
	
    /**
     * The position of the grabbed object is updated every frame.
     */
    void Update () {
        if (isGrabbed && isMoveable) {  // Checks whether the object is grabbed and moveable
            Vector3 endPosition = Camera.main.transform.position + Camera.main.transform.forward * grabbedDistance;
            Quaternion endRotation = Quaternion.Euler(new Vector3(-90, 0, 0));

            // Animate a tranistion if the object is grabbed
            this.transform.position = Vector3.Lerp(transform.position, endPosition, speed * Time.deltaTime);
            this.transform.rotation = Quaternion.Lerp(transform.rotation, endRotation, speed * Time.deltaTime);
        }
    }

    /**
     * When the object is clicked the grabbed property is toggled. If the object
     * is not moveable toggle isGrabbed to false.
     * 
     * TODO maybe change to long press action
     */
    public void OnInputClicked(InputClickedEventData eventData) {
        if (isGrabbed || !isMoveable) {
            if (isGrabbed) {
                placeAudio.Play();
            }
            isGrabbed = false;
        } else {
            QuestManager.GetInstance().currentlyWorkingOn(QuestManager.Quest.BrokenFuse);
            isGrabbed = true;
            takeAudio.Play();
        }
    }

    /**
     * Releases the grabbed object
     */
    public void StopGrabbing() {
        isGrabbed = false;
        if (placeAudio != null) {
            placeAudio.Play();
        }
    }

    /**
     * Changes the isMoveable state
     */
    public void SetMoveable(bool isMoveable) {
        this.isMoveable = isMoveable;
    }
}
