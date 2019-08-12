using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnteringButton : MonoBehaviour {

    public GameObject roomLoader;
    public string scene;
    private Button button;

	// Use this for initialization
	void Start () {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
	}
	
	
	void OnClick () {
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        roomLoader.GetComponent<RoomLoader>().EnterRoom(this.gameObject);
	}
}
