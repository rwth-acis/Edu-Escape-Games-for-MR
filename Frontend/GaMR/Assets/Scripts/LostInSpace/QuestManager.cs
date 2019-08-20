using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager {

    // Singleton instance
    private static QuestManager instance;

    private float gameStartTime = 0f;

    // HintsManager for pushing hints to the user
    private HintsManager hintsManager;
    private Dictionary<Quest, Hints.Hint[]> hints;
    private float lastDisplayedHintTime = 20f;  // Initialize with 20 to display hints from sec 30

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

    // Markes showing which quest are completed
    private Dictionary<Quest, bool> questsSolved;
    private bool gameIsOver = false;

    /**
     * Creates a new QuestManager. Therefore the HintsManager (attached to the HintDisplay)
     * is needed.
     */
    private QuestManager() {
        questInformation = new Dictionary<Quest, QuestInfo>();
        questInformation.Add(Quest.BrokenFuse, new QuestInfo());
        questInformation.Add(Quest.ElectricCircuit, new QuestInfo());
        questInformation.Add(Quest.ComputerPassword, new QuestInfo());
        questInformation.Add(Quest.Voltage, new QuestInfo());
        questInformation.Add(Quest.Document, new QuestInfo());
        questInformation.Add(Quest.None, new QuestInfo());

        questsSolved = new Dictionary<Quest, bool>();
        questsSolved.Add(Quest.BrokenFuse, false);
        questsSolved.Add(Quest.ElectricCircuit, false);
        questsSolved.Add(Quest.ComputerPassword, false);
        questsSolved.Add(Quest.Voltage, false);
        questsSolved.Add(Quest.Document, false);
        questsSolved.Add(Quest.None, false);

        hints = Hints.getHints();
    }

    public void currentlyWorkingOn(Quest quest) {
        if (currentQuest != quest) {
            QuestInfo currentQuestInfo = questInformation[currentQuest];

            if (getInGameTime() > 30) {
                currentQuestInfo.timeSpent += getInGameTime() - timeStartedQuest;
            }

            Debug.Log("Stopped working on quest " + currentQuest + ". Total time: " + currentQuestInfo.timeSpent + ", hints shown: " + currentQuestInfo.hintsShown);
            Debug.Log("Now working on quest " + quest);

            currentQuest = quest;
            timeStartedQuest = getInGameTime();
        }
    }

    private float getInGameTime() {
        return Time.time - gameStartTime;
    }

    public void checkDisplayHints() {
        if (getInGameTime() - lastDisplayedHintTime < 10f || !hints.ContainsKey(currentQuest)) {
            // Do not show a hint within the next 10 seconds
            return;
        }
        Debug.Log("Check for displaying hint");

        Quest checkQuest = currentQuest;
        QuestInfo checkInfo = questInformation[checkQuest];
        float currentTime = getInGameTime() - timeStartedQuest;

        float timeWorkedOn = checkInfo.timeSpent + currentTime;
        int hintLevel = (int)(timeWorkedOn / 60);

        if (hintLevel > checkInfo.hintsShown && hintLevel <= hints[checkQuest].Length && !questsSolved[checkQuest]) {
            hintsManager.setHint(hints[checkQuest][hintLevel - 1].hintText, hints[checkQuest][hintLevel - 1].hintImage);
            checkInfo.hintsShown++;
            Debug.Log("Displayed hint");
        }
    }

    public void setHintsManager(HintsManager hintsManager) {
        this.hintsManager = hintsManager;
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
        questsSolved[Quest.BrokenFuse] = true;
        Debug.Log("Quest solved: Broken fuse");
        CheckComputerBoot();
    }

    /**
     * Should only be called if the electric circuit quest is completed
     */
    public void CircuitFixed() {
        questsSolved[Quest.ElectricCircuit] = true;
        Debug.Log("Quest solved: Electric Circuit");
        CheckComputerBoot();
    }

    public void ComputerLogin() {
        questsSolved[Quest.ComputerPassword] = true;
        Debug.Log("Quest solved: Computer Password");
    }

    /**
     * Should only be called if the charge engine quest is completed
     */
    public void EngineStarted() {
        questsSolved[Quest.Voltage] = true;
        Debug.Log("Congratulations, you won the game!");
    }

    /**
     * Checks whether both quests (fuse and circuit) are completed
     * and boots the computer if yes
     */
    private void CheckComputerBoot() {
        if (questsSolved[Quest.BrokenFuse] && questsSolved[Quest.ElectricCircuit]) {
            GameObject.FindGameObjectWithTag("ComputerDisplay").GetComponent<ComputerDisplay>().StartComputer();
        }
    }
    
    /**
     * Simple getter method for the FuseBox Quest completed
     */
    public bool IsFuseFixed() {
        return questsSolved[Quest.BrokenFuse];
    }

    /**
     * Simple getter method for the ElectricCircuit Quest completed
     */
    public bool IsCircuitFixed() {
        return questsSolved[Quest.ElectricCircuit];
    }

    public void GameOver() {
        Debug.Log("GameOver! You ran out of time.");
    }

    public bool getIsGameOver() {
        return gameIsOver;
    }
 }
