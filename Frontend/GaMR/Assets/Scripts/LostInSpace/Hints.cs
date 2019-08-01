using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hints {

    private static readonly string[] hintsFuse = {"Click and drag the right fuse. Move it into the fuse box.", "You can calculate the right fuse with Ohm's law.", "The Ohm's law is U = R I", "Rearrange the formula: I = U/R"};
    private static readonly string[] hintsFuseImg = { };

    private static readonly string[] hintsCircuit = { "Turn the cables by clicking. The resistance of all outputs is the same.", "Create an electric circuit that applies the right tension at the outputs. The resistance of all outputs is the same.", "Try to recreate this circuit:" };
    private static readonly string[] hintsCircuitImg = {"","","circuit_solution"};

    private static readonly string[] hintsDocument = {"This is the expedition data sheet. Maybe there are useful information on it."};
    private static readonly string[] hintsDocumentImg = { };

    private static readonly string[] hintsPassword = {"Unfortunally, you forget the password due to the hard crash. ", "You're the only crew member and you are not that creative in creating password.", "Take a look at the expedition data sheet. There are some information about yourself.","It's your birthday..."};
    private static readonly string[] hintsPasswordImg = { };

    private static readonly string[] hintsVoltage = {"The engines try to start if the capacitor is charged fully.", "The full charge depends on the connected voltage.", "The needed formula is Q_max = C U.", "Search for the missing components in the formula on the expedition data sheet.", "Rearrange the formula to U = Q_max/C and plug in the given values."};
    private static readonly string[] hintsVoltageImg = { };

    public class Hint {

        public string hintText;
        public Sprite hintImage;

        public Hint(string hintText, string image) {
            this.hintText = hintText;
            
            if (image == null || image.Equals("")) {
                this.hintImage = null;
            } else {
                this.hintImage = Resources.Load(image, typeof(Sprite)) as Sprite;
            }
        }
    }

    public static Dictionary<QuestManager.Quest,Hint[]> getHints() {
        Dictionary<QuestManager.Quest, Hint[]> hints = new Dictionary<QuestManager.Quest, Hint[]>();
        hints.Add(QuestManager.Quest.BrokenFuse, createHints(hintsFuse, hintsFuseImg));
        hints.Add(QuestManager.Quest.ElectricCircuit, createHints(hintsCircuit, hintsCircuitImg));
        hints.Add(QuestManager.Quest.Document, createHints(hintsDocument, hintsDocumentImg));
        hints.Add(QuestManager.Quest.ComputerPassword, createHints(hintsPassword, hintsPasswordImg));
        hints.Add(QuestManager.Quest.Voltage, createHints(hintsVoltage, hintsVoltageImg));
        return hints;
    }

    private static Hint[] createHints(string[] hintText, string[] hintImg) {
        int length = Mathf.Max(hintText.Length, hintImg.Length);

        string text;
        string img;
        Hint[] hints = new Hint[length];

        for (int i = 0; i < length; i++) {
            text = null;
            img = null;

            if (i < hintText.Length) {
                text = hintText[i];
            }
            if (i < hintImg.Length) {
                img = hintImg[i];
            }

            hints[i] = new Hint(text, img);
        }

        return hints;
    }
}
