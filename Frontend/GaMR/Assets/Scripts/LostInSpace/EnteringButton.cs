using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnteringButton : MonoBehaviour {

    private Button button;

	// Use this for initialization
	void Start () {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
	}
	
	
	void OnClick () {
        SceneManager.LoadScene("Lost_in_Space", LoadSceneMode.Additive);
	}
}
