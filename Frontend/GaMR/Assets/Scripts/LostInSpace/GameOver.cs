using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

    public GameObject[] subElements;
    public float splashScreenShowTime;

    void Start() {
        for (int i = 0; i < subElements.Length; i++) {
            subElements[i].SetActive(false);
        }
    }

    public void Gameover() {
        for (int i = 0; i < subElements.Length; i++) {
            subElements[i].SetActive(true);
        }
    }

    private IEnumerator deloadScene() {
        yield return new WaitForSeconds(splashScreenShowTime);
        SceneManager.UnloadSceneAsync(2);
    }
}
