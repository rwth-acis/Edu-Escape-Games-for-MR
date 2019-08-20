using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OxygenUpdater : MonoBehaviour {

    public Text oxygenText;
    public int timeLimitInMinutes;
    private int timeLimitInSeconds;

    int counter = 0;

	// Use this for initialization
	void Start () {
        timeLimitInSeconds = timeLimitInMinutes * 60;
    }
	
	// Update is called once per frame
	void Update () {
        if (counter > 150) {
            QuestManager.GetInstance().checkDisplayHints();
            setOxygenLevel();
            counter = 0;
        }
        counter++;
    }

    private void setOxygenLevel() {
        float percentage = (QuestManager.GetInstance().getInGameTime() / (float) timeLimitInSeconds);

        if (percentage >= 1) {
            QuestManager.GetInstance().GameOver();
        }

        oxygenText.text = (int)(100 - percentage*100) + "%";
    }
}
