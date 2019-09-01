using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameDone : MonoBehaviour {

    public GameObject[] subElements;
    public GameObject[] toDestroyOnEnd;
    public bool isLostInSpace;
    public float splashScreenShowTime;

    void Start() {
        for (int i = 0; i < subElements.Length; i++) {
            subElements[i].SetActive(false);
        }
    }

    public void Gamedone() {
        for (int i = 0; i < subElements.Length; i++) {
            subElements[i].SetActive(true);
        }
        StartCoroutine(deloadScene());
    }

    private IEnumerator deloadScene() {
        yield return new WaitForSeconds(10);
        Debug.Log("Deload Scene");
        foreach (GameObject gameObject in toDestroyOnEnd) {
            Destroy(gameObject);
        }

        GameObject.FindGameObjectWithTag("Roomloader").GetComponent<RoomLoader>().EnableRoomCanvas();

        if (isLostInSpace) {
            GameObject.FindGameObjectWithTag("SpaceCollection").GetComponent<SpaceCollectionManager>().DisableChildren();

            try {
                Destroy(GameObject.FindGameObjectWithTag("Fuse10"));
            } catch(Exception e) {

            }

            try {
                Destroy(GameObject.FindGameObjectWithTag("Fuse15"));
            }
            catch (Exception e) {

            }

            try {
                Destroy(GameObject.FindGameObjectWithTag("Fuse20"));
            }
            catch (Exception e) {

            }
        }
    }
}
