using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject membrane;
    public GameObject mitochondria;
    public GameObject er;
    public GameObject nucleus;
    public GameObject other;

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
        membrane.GetComponent<AttachementManager>().Init();
        mitochondria.GetComponent<AttachementManager>().Init();
        er.GetComponent<AttachementManager>().Init();
        nucleus.GetComponent<AttachementManager>().Init();
        other.GetComponent<AttachementManager>().Init();
    }
	
	// Update is called once per frame
	void Update () {
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
