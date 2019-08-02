using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject membrane;
    public GameObject mitochondria;
    public GameObject er;
    public GameObject nucleus;
    private Vector3 nucleusStartScale;
    public GameObject golgi;

    public bool isEditMode;

    private int currentLevel = 1;

	// Use this for initialization
	void Start () {
		if (!isEditMode) {
            membrane.SetActive(false);
            mitochondria.SetActive(false);
            er.SetActive(false);
            golgi.SetActive(false);

            nucleusStartScale = nucleus.transform.localScale;
        }
	}
	
	// Update is called once per frame
	void Update () {
	}
}
