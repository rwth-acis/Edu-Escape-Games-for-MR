using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalModelLoadManager : MonoBehaviour {

    // The cell prefabs
    public GameObject NucleusPrefab;
    public GameObject ErPrefab;
    public GameObject MitochondriumPrefab;
    public GameObject OtherPrefab;
    public GameObject MembranePrefab;
    
    public Transform globalSpawnPosition;

	void Start () {
        Debug.Log("Start initiating Models");

        // Nucleus
        BoundingBoxId nucBoundID = new BoundingBoxId();
        ModelLoadManager nucModelLoad = new ModelLoadManager(new Vector3(2, 2, 2), globalSpawnPosition, nucBoundID, false, NucleusPrefab).LoadLocal();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
