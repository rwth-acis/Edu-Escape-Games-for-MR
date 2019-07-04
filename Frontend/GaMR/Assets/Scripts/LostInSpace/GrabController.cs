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

    private bool isGrabbed;         // Is the object currently grabbed
    
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
            this.transform.position = Camera.main.transform.position + Camera.main.transform.forward * grabbedDistance;
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
            isGrabbed = false;
        } else {
            isGrabbed = true;
        }
    }
}
