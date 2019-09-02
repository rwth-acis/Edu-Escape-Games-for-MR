using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject membrane;
    public GameObject mitochondria;
    public GameObject er;
    public GameObject nucleus;
    private Vector3 nucleusStartScale;
    private Vector3 nucleusStartPosition;
    public GameObject golgi;

    public bool isEditMode;

    private CellMode currentMode;
    private enum CellMode {
        Nucleus, Membrane, Other
    }

    private float gameSartTime = 0f;
    public Text display;
    public float timeLimit;

	// Use this for initialization
	void Start () {
		if (!isEditMode) {
            membrane.SetActive(false);
            mitochondria.SetActive(false);
            er.SetActive(false);
            golgi.SetActive(false);

            nucleusStartScale = nucleus.transform.localScale;
            nucleusStartPosition = nucleus.transform.position;
            nucleus.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
            nucleus.transform.position = new Vector3(13.337f, 12.374f, -33.347f);

            currentMode = CellMode.Nucleus;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (currentMode != CellMode.Nucleus) {
            if (Vector3.Distance(nucleusStartPosition, nucleus.transform.position) < 0.1f) {
                nucleus.transform.localScale = nucleusStartScale;
                nucleus.transform.position = nucleusStartPosition;

                membrane.SetActive(true);
                mitochondria.SetActive(true);
                er.SetActive(true);
                golgi.SetActive(true);
            } else {
                nucleus.transform.localScale = Vector3.Lerp(nucleus.transform.localScale, nucleusStartScale, Time.deltaTime);
                nucleus.transform.position = Vector3.Lerp(nucleus.transform.position, nucleusStartPosition, Time.deltaTime);
            }
        }

        updateTimeDisplay();
	}

    public void nucleusDone() {
        currentMode = CellMode.Other;
    }

    public void cellOrganesDone() {
        currentMode = CellMode.Membrane;
    }

    public void setStartTime() {
        gameSartTime = Time.time;
    }

    private void updateTimeDisplay() {
        float inGameTime = Time.time - gameSartTime;
        float timeLeft = timeLimit - inGameTime;
        int minutes = (int) timeLeft / 60;
        int seconds = (int) timeLeft % 60;

        display.text = (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds;

        if (timeLeft <= 0) {
            GameOver();
        }
    }

    private void GameOver() {
        Debug.Log("GameOver! You ran out of time.");
        GameObject.FindGameObjectWithTag("GameOverSplash").GetComponent<GameOver>().Gameover();
    }

    private void GameDone() {
        Debug.Log("Congratulations, you won the game!");
        GameObject.FindGameObjectWithTag("GameDoneSplash").GetComponent<GameDone>().Gamedone();
        PlayerPrefs.SetInt("THE_CELL_BADGE", 1);
    }
}
