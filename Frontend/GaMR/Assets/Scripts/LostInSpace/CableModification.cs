using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

/**
 * This script is used for the electric circuit quest. It handle click
 * events of the cables and turns them. 
 */
public class CableModification : MonoBehaviour, IInputClickHandler {
      
    /**
     * Is called when this tile is clicked. The tile then turns clockwise by 90 degrees.
     */
    public void OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log("Cable was clicked. Turn 90 degrees");
        this.transform.Rotate(0, 0, 90, Space.Self);    // 90 degrees in z axis and local space!
    }
}
