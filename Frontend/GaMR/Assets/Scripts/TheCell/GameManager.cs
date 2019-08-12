using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject membrane;
    public GameObject mitochondria;
    public GameObject er;
    public GameObject nucleus;
    private Vector3 nucleusStartScale;
    private Vector3 nucleusStartPosition;
    public GameObject golgi;

    public bool isEditMode;

    private CellMode currentMode;
    private enum CellMode {
        Nucleus, Membrane, Other
    }

	// Use this for initialization
	void Start () {
		if (!isEditMode) {
            membrane.SetActive(false);
            mitochondria.SetActive(false);
            er.SetActive(false);
            golgi.SetActive(false);

            nucleusStartScale = nucleus.transform.localScale;
            nucleusStartPosition = nucleus.transform.position;
            nucleus.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
            nucleus.transform.position = new Vector3(13.337f, 12.374f, -33.347f);

            currentMode = CellMode.Nucleus;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (currentMode != CellMode.Nucleus) {
            if (Vector3.Distance(nucleusStartPosition, nucleus.transform.position) < 0.1f) {
                nucleus.transform.localScale = nucleusStartScale;
                nucleus.transform.position = nucleusStartPosition;

                membrane.SetActive(true);
                mitochondria.SetActive(true);
                er.SetActive(true);
                golgi.SetActive(true);
            } else {
                nucleus.transform.localScale = Vector3.Lerp(nucleus.transform.localScale, nucleusStartScale, Time.deltaTime);
                nucleus.transform.position = Vector3.Lerp(nucleus.transform.position, nucleusStartPosition, Time.deltaTime);
            }
        }
	}

    public void nucleusDone() {
        currentMode = CellMode.Other;
    }

    public void cellOrganesDone() {
        //currentMode
    }
}
