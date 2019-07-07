using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordInput : MonoBehaviour {

    public string welcomeText;
    public string requieredPin;

    private string pin;
    private int underscoreBlink = 0;
    private bool isShowing;
    private Text text;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(welcomeText);
        sb.Append("\n");
        sb.Append("***");
        if (underscoreBlink > 20) {
            sb.Append("_");
            if (underscoreBlink > 40) {
                underscoreBlink = 0;
            }
        }
        underscoreBlink++;

        text.text = sb.ToString();
	}

    public void SetIsShowing(bool isShowing) {
        this.isShowing = isShowing;
    }
}
