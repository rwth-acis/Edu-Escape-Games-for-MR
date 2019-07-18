using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/**
 * Script that is applied to the main monitor of the board computer.
 * It manages the content and the input of the touch pad.
 */
public class ComputerDisplay : MonoBehaviour {
    
    // Configure the pin and the right charge
    public string requieredPin;
    public string requiredChargeVoltage;
    public GameObject keyboard;
    
    // Interal variables
    private int underscoreBlink = 0;
    private bool DisplayEnabled = false;
    private Text text;
    private bool passwordEnteringMode = true;
    private string currentPin = "";
    private string currentVoltage = "";

    // Currently displayed content (lines)
    private List<string> lines;

	/**
     * On start the computer is turned of. So don't show something 
     * on the screen and disable the touch pad
     */
	void Start () {
        text = GetComponent<Text>();
        text.text = "";
        keyboard.SetActive(false);
	}
	
	/**
     * Update the monitor content
     */
	void Update () {
        if (lines == null) {    // If there is no content return
            return;
        }

        if (!DisplayEnabled) {  // If the display isn't enabled return
            text.text = "";
            return;
        }

        StringBuilder sb = new StringBuilder(); // Build the display string together
        for (int i = 0; i < lines.Count; i++) {
            sb.Append(lines[i] + (i == lines.Count - 1 ? GetBlinkingUnderscore() : "") + "\n");
        }
        text.text = sb.ToString();
	}

    /**
     * Toggle display active
     */
    public void SetIsDisplayEnabled(bool DisplayEnabled) {
        this.DisplayEnabled = DisplayEnabled;
    }

    /**
     * This method should be called whenever a key is pressed.
     * The new letter is then added to the displayed content
     */
    public void Typing(string input) {
        if (input.Equals("enter")) {    // Enter button checks the current entry
            if (passwordEnteringMode) { // Check for the correct password
                AddLine("");
                if (currentPin.Equals(requieredPin)) {
                    AddLine("*** ACCESS GRANTED ***");
                    AddLine("Welcome! You are now logged in.");
                    EnterTime();
                } else {
                    AddLine("*** ACCESS DENIED ***");
                    EnterPassword();
                }
            } else {                    // Check for the correct charge
                AddLine("Charge engine with " + currentVoltage + "V");
                AddLine("");
                if (currentVoltage.Equals(requiredChargeVoltage)) {
                    AddLine("Engine 1 started!");
                    AddLine("Engine 2 started!");
                    AddLine("");
                    AddLine("Launching...");
                    QuestManager.GetInstance().EngineStarted();
                } else {
                    AddLine("Something went wrong... Try with another charge voltage!");
                    EnterTime();
                }
            }
        } else {    // Added a letter
            if (passwordEnteringMode) { // Only add a star in password mode
                lines[lines.Count - 1] = lines[lines.Count - 1] + "*";
                currentPin = currentPin + input;
            } else {
                lines[lines.Count - 1] = lines[lines.Count - 1] + input;
                currentVoltage = currentVoltage + input;
            }
        }
    }

    /**
     * Adds a new line to the display content. If there are
     * already 10 line on the screen the oldes lines is
     * deleted automatically
     */
    private void AddLine(string line) {
        if (lines == null) {
            lines = new List<string>();
        }
        if (lines.Count >= 10) {
            lines.RemoveAt(0);
        }
        lines.Add(line);
    }

    /**
     * Return 20 times an underscore and the next 20 times
     * an empty string. Calling this in the Update() method
     * achieves a blinking underscore
     */
    private string GetBlinkingUnderscore() {
        underscoreBlink++;
        if (underscoreBlink > 20) {
            if (underscoreBlink > 40) {
                underscoreBlink = 0;
            }
            return "_";
        } else {
            return "";
        }
    }

    /**
     * Adds the desired lines to the display and configures the mode
     */
    private void EnterPassword() {
        passwordEnteringMode = true;
        currentPin = "";
        AddLine("Please enter your personal pincode to continue:");
        AddLine("");
    }

    /**
     * Adds the desired lines to the display and configures the mode
     */
    private void EnterTime() {
        passwordEnteringMode = false;
        currentVoltage = "";
        AddLine("The engine capacitor need to be loaded before launching!");
        AddLine("Specify charging tension in volts: ");
    }

    /**
     * Turns on the computer
     */
    public void StartComputer() {
        StartCoroutine(BootComputer());
    }

    /**
     * A nested method to get a delayed boot and password request
     */
    private IEnumerator BootComputer() {
        DisplayEnabled = true;
        AddLine("Booting...");
        yield return new WaitForSeconds(10);
        keyboard.SetActive(true);
        EnterPassword();
    }
}
