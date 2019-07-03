using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class BoxController : MonoBehaviour, IFocusable, IInputClickHandler {

    private Renderer render;
    private Shader standard;
    private Shader focused;
    private bool isOpened;

    private Animation anim;

    // Use this for initialization
    void Awake () {
        render = GetComponentInChildren<Renderer>();
        standard = Shader.Find("Standard");
        focused = Shader.Find("Diffuse");

        anim = gameObject.GetComponent<Animation>();
        if (anim != null)
        {
            anim["close_animation"].wrapMode = WrapMode.Once;
            anim.Play("close_animation");
            isOpened = false;
        } else
        {
            Debug.Log("Animator is null!");
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnFocusEnter()
    {
        render.material.shader = focused;
    }

    public void OnFocusExit()
    {
        render.material.shader = standard;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (isOpened)
        {
            anim["close_animation"].wrapMode = WrapMode.Once;
            anim.Play("close_animation");
            isOpened = false;
        } else {
            anim["open_animation"].wrapMode = WrapMode.Once;
            anim.Play("open_animation");
            isOpened = true;
        }
    }
}
