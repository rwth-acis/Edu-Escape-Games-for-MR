using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameDone : MonoBehaviour {

    public GameObject[] subElements;
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
    }

    private IEnumerator deloadScene() {
        yield return new WaitForSeconds(splashScreenShowTime);
        SceneManager.UnloadSceneAsync(2);
    }
}
