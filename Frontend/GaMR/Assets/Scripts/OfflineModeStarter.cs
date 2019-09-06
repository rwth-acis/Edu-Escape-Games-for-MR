using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OfflineModeStarter : MonoBehaviour {

    private Button button;

	void Start () {
        button = GetComponent<Button>();
        button.onClick.AddListener(playOffline);
	}

    void playOffline() {
        Debug.Log("Play offline");
        AuthorizationManager.Instance.playOffline();
        SceneManager.LoadScene("Scene", LoadSceneMode.Single);
    }
}
