using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager {

    // Singleton instance
    private static QuestManager instance;

    // HintsManager for pushing hints to the user
    private HintsManager hintsManager;

    public enum Quest {
        BrokenFuse, ElectricCircuit, ComputerPassword, Voltage, Document, None
    }

    private class QuestInfo {
        public float timeSpent = 0;
        public int hintsShown = 0;
    }

    private Dictionary<Quest, QuestInfo> questInformation;
    private Quest currentQuest = Quest.None;
    private float timeStartedQuest = 0f;

    // Markes showing quich quest are completed
    private bool fusefixed = false;
    private bool circuitfixed = false;
    private bool engineStarted = false;

    /**
     * Creates a new QuestManager. Therefore the HintsManager (attached to the HintDisplay)
     * is needed.
     */
    private QuestManager() {
        questInformation = new Dictionary<Quest, QuestInfo>();
    }

    public void currentlyWorkingOn(Quest quest) {
        if (currentQuest != quest) {
            QuestInfo currentQuestInfo = questInformation.ContainsKey(currentQuest) ?  questInformation[currentQuest] : null;

            if (currentQuestInfo == null) {
                currentQuestInfo = new QuestInfo();
                questInformation.Add(currentQuest, currentQuestInfo);
            }

            if (Time.time > 30) {
                currentQuestInfo.timeSpent += Time.time - timeStartedQuest;
            }

            Debug.Log("Stopped working on quest " + currentQuest + ". Total time: " + currentQuestInfo.timeSpent + ", hints shown: " + currentQuestInfo.hintsShown);
            Debug.Log("Now working on quest " + quest);

            currentQuest = quest;
            timeStartedQuest = Time.time;
        }
    }

    /**
     * Return the instance of the QuestManager
     */
    public static QuestManager GetInstance() {
        if (instance == null) {
            instance = new QuestManager();
        }
        return instance;
    }

    /**
     * Should only be called if the fuse quest is completed
     */
    public void FuseFixed() {
        fusefixed = true;
        Debug.Log("Quest solved: Broken fuse");
        CheckComputerBoot();
    }

    /**
     * Should only be called if the electric circuit quest is completed
     */
    public void CircuitFixed() {
        circuitfixed = true;
        Debug.Log("Quest solved: Electric Circuit");
        CheckComputerBoot();
    }

    /**
     * Should only be called if the charge engine quest is completed
     */
    public void EngineStarted() {
        engineStarted = true;
        Debug.Log("Congratulations, you won the game!");
    }

    /**
     * Checks whether both quests (fuse and circuit) are completed
     * and boots the computer if yes
     */
    private void CheckComputerBoot() {
        if (fusefixed && circuitfixed) {
            GameObject.FindGameObjectWithTag("ComputerDisplay").GetComponent<ComputerDisplay>().StartComputer();
        }
    }
    
    /**
     * Simple getter method for the FuseBox Quest completed
     */
    public bool IsFuseFixed() {
        return fusefixed;
    }

    /**
     * Simple getter method for the ElectricCircuit Quest completed
     */
    public bool IsCircuitFixed() {
        return circuitfixed;
    }
}
