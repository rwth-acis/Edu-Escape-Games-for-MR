using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTransfrom : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        Vector3 fwd = transform.rotation * Vector3.forward;
        Debug.DrawRay(transform.position, fwd, Color.red, 0f, true);
    }
}
