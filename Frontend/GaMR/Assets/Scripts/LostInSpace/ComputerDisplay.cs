using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ComputerDisplay : MonoBehaviour {
    
    public string requieredPin;
    public string correctChargeTime;
    
    private int underscoreBlink = 0;
    private bool DisplayEnabled;
    private Text text;
    private bool passwordEnteringMode = true;
    private string currentPin = "";
    private string currentTime = "";

    private List<string> lines;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        AddLine("Booting...");
        EnterPassword();
	}
	
	// Update is called once per frame
	void Update () {
        if (lines == null) {
            return;
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < lines.Count; i++) {
            sb.Append(lines[i] + (i == lines.Count - 1 ? GetBlinkingUnderscore() : "") + "\n");
        }
        text.text = sb.ToString();
	}

    public void SetIsDisplayEnabled(bool DisplayEnabled) {
        this.DisplayEnabled = DisplayEnabled;
    }

    public void Typing(string input) {
        if (input.Equals("enter")) {
            if (passwordEnteringMode) {
                AddLine("");
                if (currentPin.Equals(requieredPin)) {
                    AddLine("*** ACCESS GRANTED ***");
                    AddLine("Welcome! You are now logged in.");
                    EnterTime();
                } else {
                    AddLine("*** ACCESS DENIED ***");
                    EnterPassword();
                }
            } else {
                AddLine("Charge engine " + currentTime + "s...");
                AddLine("");
                if (currentTime.Equals(correctChargeTime)) {
                    AddLine("Engine 1 started!");
                    AddLine("Engine 2 started!");
                    AddLine("");
                    AddLine("Launching...");
                } else {
                    AddLine("Something went wrong... Try with another charge time!");
                    EnterTime();
                }
            }
        } else {
            if (passwordEnteringMode) {
                lines[lines.Count - 1] = lines[lines.Count - 1] + "*";
                currentPin = currentPin + input;
            } else {
                lines[lines.Count - 1] = lines[lines.Count - 1] + input;
                currentTime = currentTime + input;
            }
        }
    }

    private void AddLine(string line) {
        if (lines == null) {
            lines = new List<string>();
        }
        if (lines.Count >= 10) {
            lines.RemoveAt(0);
        }
        lines.Add(line);
    }

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

    private void EnterPassword() {
        passwordEnteringMode = true;
        currentPin = "";
        AddLine("Please enter your password to continue:");
        AddLine("");
    }

    private void EnterTime() {
        passwordEnteringMode = false;
        currentTime = "";
        AddLine("Specify a charge time to start the engines:");
        AddLine("Charge time in seconds: ");
    }
}
