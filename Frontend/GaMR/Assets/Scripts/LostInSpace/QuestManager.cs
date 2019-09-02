using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager {

    // Singleton instance
    private static QuestManager instance;

    public static readonly string GAME_ID = "LOST_IN_SPACE";

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

    private bool StoreGamificationData = false;

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

        if (StoreGamificationData) {
            CreateGame();       // Create Game in Gamification Database
        }
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

    public float getInGameTime() {
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
        CreateQuest("BROKEN_FUSE", "Broken Fuse", "The player has to fix the broken fuse by replacing it with a new fuse. The maximum current needs to be calculated.");
    }

    /**
     * Should only be called if the electric circuit quest is completed
     */
    public void CircuitFixed() {
        questsSolved[Quest.ElectricCircuit] = true;
        Debug.Log("Quest solved: Electric Circuit");
        CheckComputerBoot();
        CreateQuest("ELECTRIC_CIRCUIT", "Electric Circuit", "The player hast to recreate an electric circuit to match the labeled outputs.");
    }

    public void ComputerLogin() {
        questsSolved[Quest.ComputerPassword] = true;
        Debug.Log("Quest solved: Computer Password");
        CreateQuest("COMPUTER_LOGIN", "Computer Login", "The player hast to find out the password und log in.");
    }

    /**
     * Should only be called if the charge engine quest is completed
     */
    public void EngineStarted() {
        questsSolved[Quest.Voltage] = true;
        Debug.Log("Congratulations, you won the game!");
        GameObject.FindGameObjectWithTag("GameDoneSplash").GetComponent<GameDone>().Gamedone();
        CreateQuest("ENGINE_START", "Engine start", "The player has to calculate the voltage needed to charge the capacitor to the right charge amount.");
        PlayerPrefs.SetInt("LOST_IN_SPACE_BADGE", 1);
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
        GameObject.FindGameObjectWithTag("GameOverSplash").GetComponent<GameOver>().Gameover();
    }

    public bool getIsGameOver() {
        return gameIsOver;
    }

    public void startGame() {
        gameStartTime = Time.time;
    }

    public void CreateGame() {
        Game game = new Game(GAME_ID, "Lost in Space, Educational Mixed Reality Escape Game. For learning of physics of electricity");
        GamificationFramework.Instance.CreateGame(game, (game1, code) => {
            if (game1 != null) {
                Debug.Log("Game with ID " + game1.ID + " was setup in Gamification Framework");
            }
        });
        GamificationFramework.Instance.AddUserToGame(GAME_ID, code => {
            Debug.Log("Response code for adding user to game: " + code);
        });
    }

    public void CreateQuest(string ID, string name, string description) {
        if (StoreGamificationData) {
            global::Quest quest = new global::Quest(ID, name, QuestStatus.REVEALED, ID, false, false, 1, description);
            GamificationFramework.Instance.CreateQuest(GAME_ID, quest, (quest1, code) => { });
        }
    }

    public void CreateAchievement() {
        Achievement achievement = new Achievement(GAME_ID + "_ACHIEVEMENT", "Escape from Lost in Space", "The player completed the Lost in Space Escape Game.", 10, GAME_ID + "_BADGE");
        GamificationFramework.Instance.CreateAchievement(GAME_ID, achievement, (achievement1, code) => {
        });
    }
 }
