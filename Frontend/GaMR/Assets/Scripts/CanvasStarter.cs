using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasStarter : MonoBehaviour {

    // Use this for initialization
    void Start() {
        gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
    }
}