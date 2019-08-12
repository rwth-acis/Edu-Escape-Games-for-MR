using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLoader : MonoBehaviour {

    public GameObject[] canavasForEachRoom;

	public void EnterRoom(GameObject gameObject) {
        DisableRoomCanavas();
    }

    private void DisableRoomCanavas() {
        if (canavasForEachRoom != null) {
            for (int i = 0; i < canavasForEachRoom.Length; i++) {
                canavasForEachRoom[i].SetActive(false);
            }
        }
    }
}
