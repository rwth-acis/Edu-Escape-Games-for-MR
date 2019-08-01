using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintsManager : MonoBehaviour {

    public Text hintText;
    public Image hintImage;

    void Start() {
        setHint(null, null);
        QuestManager.GetInstance().setHintsManager(this);
    }
    
    public void setHint(string hintText, Sprite hintImage) {
        if (hintText == null) {
            this.hintText.gameObject.SetActive(false);
        } else {
            this.hintText.gameObject.SetActive(true);
            this.hintText.text = hintText;
        }

        if (hintImage == null) {
            this.hintImage.gameObject.SetActive(false);
        }
        else {
            this.hintImage.gameObject.SetActive(true);

            this.hintImage.sprite = hintImage;
        }

        if (hintImage == null && hintText == null) {
            gameObject.SetActive(false);
        } else {
            gameObject.SetActive(true);
        }
    }
}
