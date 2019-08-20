using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDone : MonoBehaviour {

    public Canvas gamedoneCanvas;
    public float splashScreenShowTime;

    public void Gamedone() {
        gamedoneCanvas.transform.gameObject.SetActive(true);
    }

    private IEnumerator deloadScene() {
        yield return new WaitForSeconds(splashScreenShowTime);
        SceneManager.UnloadSceneAsync(2);
    }
}
