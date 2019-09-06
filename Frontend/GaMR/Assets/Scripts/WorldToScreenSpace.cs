using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldToScreenSpace : MonoBehaviour {

    private static readonly float canvasDistance = 2;

	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position = Camera.main.transform.position + Camera.main.transform.forward * canvasDistance;
        this.transform.LookAt(Camera.main.transform);
	}
}
