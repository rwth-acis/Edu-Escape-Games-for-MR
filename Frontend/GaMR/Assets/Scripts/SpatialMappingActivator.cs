using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialMappingActivator : MonoBehaviour {

    public GameObject[] spatialMappingObjects;

    public void startSpatialMapping() {
        foreach (GameObject gameObject in spatialMappingObjects) {
            gameObject.SetActive(true);
        }
    }
}
