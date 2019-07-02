using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class CableModification : MonoBehaviour, IFocusable, IInputClickHandler {

    private Renderer rend;
    private Shader standard;
    private Shader focused;
    

    // Use this for initialization
    void Awake () {
        rend = GetComponentInChildren<Renderer>();
        standard = Shader.Find("Standard");
        focused = Shader.Find("Diffuse");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /**
     * Is called when the cursor points onto this tile. The shader is changed
     * to give a visual feedback to the user.
     */
    public void OnFocusEnter()
    {
        Debug.Log("Entered Focus");
        rend.material.shader = focused;
    }

    /**
     * Is called when the cursor does not point onto this tile anymore. The shader
     * is changed back to the original one.
     */
    public void OnFocusExit()
    {
        Debug.Log("Left Focus");
        rend.material.shader = standard;
    }

    /**
     * Is called when this tile is clicked. The tile then turns clockwise by 90 degrees.
     */
    public void OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log("Input was clicked");
        this.transform.Rotate(0, 0, 90, Space.Self);
    }
}
