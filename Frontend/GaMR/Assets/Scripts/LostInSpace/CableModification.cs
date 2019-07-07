using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

/**
 * This script is used for the electric circuit quest. It handle click
 * events of the cables and turns them. 
 */
public class CableModification : MonoBehaviour, IInputClickHandler {

    public int tileType;
    public int x;
    public int y;
    public int rotation;

    private GameObject breadboard;

    /**
     * On awake the cable registers at the CircuitController of the breadboard and turns
     * itself to a random rotation.
     */
    void Awake() {
        breadboard = GameObject.FindGameObjectWithTag("Breadboard");    // Register cable
        breadboard.GetComponent<CircuitController>().registerTile(x, y, rotation);

        int rotations = Random.Range(0, 4);                             // Random rotation
        for (int i = 0; i < rotations; i++) {
            TurnCable();
        }
    }

    /**
     * Is called when this tile is clicked.
     */
    public void OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log("Cable " + x + ", " + y + " was clicked. Turn 90 degrees");
        TurnCable();
    }

    /**
     * Turns the cable 90 degrees clockwise
     */
    private void TurnCable() {
        this.transform.Rotate(0, 0, 90, Space.Self);    // 90 degrees in z axis and local space!
        rotation = (rotation + 90) % 360;
        breadboard.GetComponent<CircuitController>().TurnTile(x, y, rotation);
    }
}
