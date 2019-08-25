using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLoader : MonoBehaviour {

    public GameObject[] objectsToDisable;

    public enum Room {
        LostInSpace, TheCell
    }

	public void EnterRoom(Room room) {
        DisableRoomCanavas();
        if (room == Room.LostInSpace) {
            Debug.Log("Enable Space collection");
            GameObject.FindGameObjectWithTag("SpaceCollection").GetComponent<SpaceCollectionManager>().EnableChildren();
        }
    }

    private void DisableRoomCanavas() {
        if (objectsToDisable != null) {
            for (int i = 0; i < objectsToDisable.Length; i++) {
                objectsToDisable[i].SetActive(false);
            }
        }
    }
}
