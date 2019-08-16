using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLoader : MonoBehaviour {

    public GameObject[] objectsToDisable;

	public void EnterRoom(GameObject gameObject) {
        DisableRoomCanavas();
    }

    private void DisableRoomCanavas() {
        if (objectsToDisable != null) {
            for (int i = 0; i < objectsToDisable.Length; i++) {
                objectsToDisable[i].SetActive(false);
            }
        }
    }
}
