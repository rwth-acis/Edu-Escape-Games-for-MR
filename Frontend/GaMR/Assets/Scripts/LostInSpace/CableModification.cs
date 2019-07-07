using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

/**
 * This script is used for the electric circuit quest. It handle click
 * events of the cables and turns them. 
 */
public class CableModification : MonoBehaviour, IInputClickHandler {

    public static readonly int STRAIGHT_CON_INDEX = 0;
    public static readonly int CURVE_CON_INDEX = 1;
    public static readonly int TCURVE_CON_INDEX = 2;
    public static readonly int CROSS_CON_INDEX = 3;
    public static readonly int INVERTEDX_CON_INDEX = 4;

    public int tileType;
    public int x;
    public int y;
    public int rotation;

    GameObject breadboard;

    void Awake() {
        breadboard = GameObject.FindGameObjectWithTag("Breadboard");
        breadboard.GetComponent<CircuitController>().registerTile(x, y, rotation);

        int rotations = Random.Range(0, 4);
        for (int i = 0; i < rotations; i++) {
            TurnCable();
        }
    }

    /**
     * Is called when this tile is clicked. The tile then turns clockwise by 90 degrees.
     */
    public void OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log("Cable " + x + ", " + y + " was clicked. Turn 90 degrees");
        TurnCable();
    }

    private void TurnCable() {
        this.transform.Rotate(0, 0, 90, Space.Self);    // 90 degrees in z axis and local space!
        rotation = (rotation + 90) % 360;
        breadboard.GetComponent<CircuitController>().TurnTile(x, y, rotation);
    }
}
