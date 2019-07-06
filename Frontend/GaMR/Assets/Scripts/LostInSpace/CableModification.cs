using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

/**
 * This script is used for the electric circuit quest. It handle click
 * events of the cables and turns them. 
 */
public class CableModification : MonoBehaviour, IInputClickHandler {

    private readonly bool[,] STRAIGHT_CON = new bool[4, 4] { { false, false, false, false }, { false, false, false, true }, { false, false, false, false }, { false, true, false, false } };
    private readonly bool[,] CURVE_CON = new bool[4, 4] { { false, true, false, false }, { true, false, false, false }, { false, false, false, false }, { false, false, false, false } };
    private readonly bool[,] TCURVE_CON = new bool[4, 4] { { false, false, false, false }, { false, false, true, true }, { false, true, false, true }, { false, true, true, false } };
    private readonly bool[,] CROSS_CON = new bool[4, 4] { { false, false, true, false }, { false, false, false, true }, { true, false, false, false }, { false, true, false, false } };
    private readonly bool[,] INVERTEDX_CON = new bool[4, 4] { { false, true, false, false }, { true, false, false, false }, { false, false, false, true }, { false, false, true, false } };

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
        breadboard.GetComponent<CircuitController>().registerTile(x, y, getConnections(tileType), rotation);
    }

    /**
     * Is called when this tile is clicked. The tile then turns clockwise by 90 degrees.
     */
    public void OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log("Cable " + x + ", " + y + " was clicked. Turn 90 degrees");
        this.transform.Rotate(0, 0, 90, Space.Self);    // 90 degrees in z axis and local space!
        rotation = (rotation + 90) % 360;
        breadboard.GetComponent<CircuitController>().TurnTile(x, y, rotation);
    }

    private bool[,] getConnections(int tileType) {
        switch (tileType) {
            case 0:
                return STRAIGHT_CON;
            case 1:
                return CURVE_CON;
            case 2:
                return TCURVE_CON;
            case 3:
                return CROSS_CON;
            case 4:
                return INVERTEDX_CON;
            default:
                Debug.LogWarning("Undefined tile type!");
                break;
        }
        return null;
    }
}
