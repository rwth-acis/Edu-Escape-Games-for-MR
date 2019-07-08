using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager {

    // Singleton instance
    private static QuestManager instance = new QuestManager();

    // Markes showing quich quest are completed
    private bool fusefixed = false;
    private bool circuitfixed = false;
    private bool engineStarted = false;

    /**
     * Return the instance of the QuestManager
     */
    public static QuestManager GetInstance() {
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
