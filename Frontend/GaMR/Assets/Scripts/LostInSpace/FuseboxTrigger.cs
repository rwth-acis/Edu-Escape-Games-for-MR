using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseboxTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider collider) {
        string objectTag = collider.gameObject.tag;

        Debug.Log("Was trigger by " + objectTag);

        switch (objectTag) {
            case "Fuse10":
                Debug.Log("Fuse 10");
                break;
            case "Fuse15":
                Debug.Log("Fuse 15");
                break;
            case "Fuse20":
                Debug.Log("Fuse 20");
                break;
            case "FuseBroken":
                Debug.Log("Fuse broken");
                break;
            default:
                Debug.Log("You can't put that here");
                break;
        }
    }
}
