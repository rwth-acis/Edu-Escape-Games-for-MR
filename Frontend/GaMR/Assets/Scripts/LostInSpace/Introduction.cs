using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Introduction : MonoBehaviour {

    public Canvas introductionCanvas;
    public Text[] introText;

    public float textShowTime;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < introText.Length; i++) {
            StartCoroutine(DisplayText(introText[i], i * (0.5f + textShowTime), i == introText.Length - 1));
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator DisplayText(Text text, float delay, bool isLast) {
        yield return new WaitForSeconds(delay);
        text.transform.gameObject.SetActive(true);
        StartCoroutine(HideText(text, isLast));
    }

    private IEnumerator HideText(Text text, bool isLast) {
        yield return new WaitForSeconds(textShowTime);
        text.transform.gameObject.SetActive(false);
        if (isLast) {
            introductionCanvas.transform.gameObject.SetActive(false);
            QuestManager.GetInstance().startGame();
        }
    }
}
