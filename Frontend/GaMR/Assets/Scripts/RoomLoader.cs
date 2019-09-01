using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLoader : MonoBehaviour {

    public GameObject[] objectsToDisable;

    public enum Room {
        LostInSpace, TheCell
    }

    private void Start() {
        GameObject.FindGameObjectWithTag("Activator").GetComponent<SpatialMappingActivator>().startSpatialMapping();
    }

    public void EnterRoom(Room room) {
        DisableRoomCanvas();
        if (room == Room.LostInSpace) {
            Debug.Log("Enable Space collection");
            GameObject.FindGameObjectWithTag("SpaceCollection").GetComponent<SpaceCollectionManager>().EnableChildren();
        }
    }

    public void DisableRoomCanvas() {
        if (objectsToDisable != null) {
            for (int i = 0; i < objectsToDisable.Length; i++) {
                objectsToDisable[i].SetActive(false);
            }
        }
    }

    public void EnableRoomCanvas() {
        if(objectsToDisable != null) {
            for (int i = 0; i < objectsToDisable.Length; i++) {
                objectsToDisable[i].SetActive(true);
            }
        }
    }
    
}
