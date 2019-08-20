using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

    public Canvas gameoverCanvas;
    public float splashScreenShowTime;

    public void Gameover() {
        gameoverCanvas.transform.gameObject.SetActive(true);
    }

    private IEnumerator deloadScene() {
        yield return new WaitForSeconds(splashScreenShowTime);
        SceneManager.UnloadSceneAsync(2);
    }
}
