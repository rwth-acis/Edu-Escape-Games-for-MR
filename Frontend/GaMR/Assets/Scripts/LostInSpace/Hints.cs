using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hints {

    public static readonly Dictionary<QuestManager.Quest, Hints[]> hints = new Dictionary<QuestManager.Quest, Hints[]> {
        {QuestManager.Quest.BrokenFuse, {"Penis", "Vagina"} }
    };

    public class Hint {

        public string hintText;
        public Sprite hintImage;

        public Hint(string hintText, string image) {
            this.hintText = hintText;
            
            if (image == null) {
                this.hintImage = null;
            } else {
                this.hintImage = Resources.Load(image, typeof(Sprite)) as Sprite;
            }
        }
    }
}
