using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputerInputButton : MonoBehaviour {

    // The key the button is representing
    public string key;

    public AudioSource clickAudio;

    private Button button;
    private GameObject computer;

	/**
     * On start attach the OnClickListener and search for the computer
     */
	void Start () {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        computer = GameObject.FindGameObjectWithTag("ComputerDisplay");
	}

    /**
     * Tell the computer that the button was clicked
     */
    private void OnClick() {
        computer.GetComponent<ComputerDisplay>().Typing(key);
        clickAudio.Play();
    }
}
