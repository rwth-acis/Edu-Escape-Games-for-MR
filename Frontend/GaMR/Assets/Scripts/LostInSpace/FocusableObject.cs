using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

/**
 * A simple script that makes any object focusable. All clickable objects
 * should apply this to ensure user feedback. The script easily changes
 * the shader of the focused object to a diffuse shader.
 * 
 * IMPORTANT: make sure the game object uses a collider component
 */
public class FocusableObject : MonoBehaviour, IFocusable {

    private Renderer render;
    private Shader standard;
    private Shader focused;

    /**
     * On start the renderer and the needed shaders are loaded.
     */
    void Start () {
        render = GetComponent<Renderer>();
        standard = render.material.shader;  // Load the shader applied in the inspector
        focused = Shader.Find("Diffuse");   // Load the diffuse shader

        if (standard.Equals(focused))
        {
            // Check if the diffuse shader is already in use. Log a warning that the focus will have no effect
            Debug.LogWarning("Focusable is used on a diffuse object! Focusable will have no effect.");
        }
    }

    /**
     * The method is called everytime the HoloLens cursor enters focus. 
     * Change the shader to focused shader if that happen.
     */
    public void OnFocusEnter()
    {
        render.material.shader = focused;
    }

    /**
     * The method is called everytime the HoloLens cursor leaves focus. 
     * Change the shader to original shader if that happen.
     */
    public void OnFocusExit()
    {
        render.material.shader = standard;
    }
}
