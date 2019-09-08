using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public bool isEditMode;
    public GameObject AnnotationManagement;

    private CellMode currentMode;
    public enum CellMode {
        Nucleus, ER, Mitochondrium, Other, Membrane
    }

    private float gameSartTime = 0f;
    public Text display;
    public float timeLimit;

    // Use this for initialization
    void Start() {
        AnnotationManagement.GetComponent<AnnotationManagement>().StartQuiz(CellMode.Nucleus, () => {
            AnnotationManagement.GetComponent<AnnotationManagement>().StartQuiz(CellMode.ER, () => {
                AnnotationManagement.GetComponent<AnnotationManagement>().StartQuiz(CellMode.Mitochondrium, () => {
                    AnnotationManagement.GetComponent<AnnotationManagement>().StartQuiz(CellMode.Other, () => {
                        AnnotationManagement.GetComponent<AnnotationManagement>().StartQuiz(CellMode.Membrane, () => {
                            GameDone();
                        });
                    });
                });
            });
        });


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
        if (InformationManager.Instance.UserInfo != null) {
            PlayerPrefs.SetInt("THE_CELL_BADGE_" + InformationManager.Instance.UserInfo.preferred_username, 1);
        }
    }
}
