using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnnotationManagement : MonoBehaviour {

    // General Attributes
    public GameObject membrane;
    public GameObject mitochondria;
    public GameObject er;
    public GameObject nucleus;
    public GameObject other;

    public GameObject nucleusMenu;
    public GameObject[] nucleusAnnotations;
    public GameObject erMenu;
    public GameObject[] erAnnotations;
    public GameObject mitochondriumMenu;
    public GameObject[] mitochondriumAnnotations;
    public GameObject otherMenu;
    public GameObject[] otherAnnotations;
    public GameObject membraneMenu;
    public GameObject[] membraneAnnotations;

    public Material AnnotationMaterial;


    // Quiz specific attributes
    private GameObject[] buttonObjects;
    private FocusableCheckButton[] currentButtons;
    private GameObject[] currentAnnotations;
    private TapNotifier[] currentNotifiers;
    private GameObject currentMenu;

    private int lastClickedAnnotationIndex = -1;
    private AnnotationMode currentAnnotationMode = AnnotationMode.None;
    private int remainingQuestions;

    private Action callback;


    private enum AnnotationMode {
        Text, AnnotationObject, None
    }

    public void StartQuiz(GameManager.CellMode cellMode, Action callback) {
        Debug.Log("Start quiz " + cellMode);
        switch (cellMode) {
            case GameManager.CellMode.Nucleus:
                StartQuiz(nucleus, nucleusMenu, nucleusAnnotations);
                break;
            case GameManager.CellMode.ER:
                StartQuiz(er, erMenu, erAnnotations);
                break;
            case GameManager.CellMode.Mitochondrium:
                StartQuiz(mitochondria, mitochondriumMenu, mitochondriumAnnotations);
                break;
            case GameManager.CellMode.Other:
                StartQuiz(other, otherMenu, otherAnnotations);
                break;
            case GameManager.CellMode.Membrane:
                StartQuiz(membrane, membraneMenu, membraneAnnotations);
                break;
        }
        this.callback = null;
        this.callback = callback;
    }

    private void StartQuiz(GameObject gameobject, GameObject menu, GameObject[] annotations) {
        Debug.Log("Start quiz for object " + gameobject.name);
        menu.SetActive(true);
        currentMenu = menu;

        currentButtons = new FocusableCheckButton[annotations.Length];
        buttonObjects = new GameObject[annotations.Length];
        currentNotifiers = new TapNotifier[annotations.Length];

        for (int i = 0; i < annotations.Length; i++) {
            buttonObjects[i] = menu.transform.Find("Item" + (i + 1) + " Button").gameObject;
            currentButtons[i] = buttonObjects[i].GetComponent<FocusableCheckButton>();

            annotations[i].SetActive(true);
            currentNotifiers[i] = annotations[i].AddComponent<TapNotifier>();
        }

        try {
            currentButtons[0].OnButtonPressed = ClickText0;
            currentButtons[1].OnButtonPressed = ClickText1;
            currentButtons[2].OnButtonPressed = ClickText2;
            currentButtons[3].OnButtonPressed = ClickText3;
            currentButtons[4].OnButtonPressed = ClickText4;
        } catch (IndexOutOfRangeException e) {
            Debug.Log(e.Message);
        }

        try {
            currentNotifiers[0].RegisterListenerOnInputUp(Annotation0Clicked);
            currentNotifiers[1].RegisterListenerOnInputUp(Annotation1Clicked);
            currentNotifiers[2].RegisterListenerOnInputUp(Annotation2Clicked);
            currentNotifiers[3].RegisterListenerOnInputUp(Annotation3Clicked);
            currentNotifiers[4].RegisterListenerOnInputUp(Annotation4Clicked);
        } catch (IndexOutOfRangeException e) {
            Debug.Log(e.Message);
        }

        currentAnnotations = annotations;
        remainingQuestions = annotations.Length;
    }

    private void FinishedQuiz() {
        Debug.Log("Finished Quiz successfully");
        currentMenu.SetActive(false);
        currentMenu = null;
        buttonObjects = null;
        currentButtons = null;
        currentAnnotations = null;
        lastClickedAnnotationIndex = -1;
        currentAnnotationMode = AnnotationMode.None;

        if (callback != null) {
            callback();
        } else {
            Debug.Log("Callback is null");
        }
    }

    private void ClickedText(int index) {
        Debug.Log("Clicked Button " + index);
        if (lastClickedAnnotationIndex == -1 || currentAnnotationMode == AnnotationMode.None || currentAnnotationMode == AnnotationMode.Text) {
            Debug.Log("First button clicked");
            lastClickedAnnotationIndex = index;
            currentAnnotationMode = AnnotationMode.Text;
            CheckButton(index);
        } else {
            Debug.Log("Second Button clicked");
            CheckClicked(index, AnnotationMode.Text);
        }
    }

    private void ClickedAnnotation(int index) {
        if (lastClickedAnnotationIndex == -1 || currentAnnotationMode == AnnotationMode.None || currentAnnotationMode == AnnotationMode.AnnotationObject) {
            Debug.Log("First annotation clicked");
            lastClickedAnnotationIndex = index;
            currentAnnotationMode = AnnotationMode.AnnotationObject;
            MarkAnnotationObjects(index);
        }
        else {
            Debug.Log("Second Annotation clicked");
            CheckClicked(index, AnnotationMode.AnnotationObject);
        }
    }

    private void CheckClicked(int index, AnnotationMode mode) {
        Debug.Log("Verfy clicked annotation " + index);
        if (index == lastClickedAnnotationIndex && mode != currentAnnotationMode) {
            buttonObjects[index].SetActive(false);
            currentAnnotations[index].SetActive(false);

            remainingQuestions--;
            Debug.Log("Correct answer! Remaining questions: " + remainingQuestions);

            if (remainingQuestions == 0) {
                FinishedQuiz();
            }
        } else {
            CheckButton(-1);
            MarkAnnotationObjects(-1);
            MessageBox.Show("That's not correct!", MessageBoxType.ERROR);
            Debug.Log("Clicked annotation " + index + " and lastClickedAnnotation " + lastClickedAnnotationIndex + " does not match");
        }
        lastClickedAnnotationIndex = -1;
        currentAnnotationMode = AnnotationMode.None;
    }

    private void CheckButton(int index){
        if (currentButtons != null && currentButtons.Length > index) {
            for (int i = 0; i < currentButtons.Length; i++) {
                currentButtons[i].ButtonChecked = i == index;
            }
        } else {
            Debug.Log("Tried to click unavailable button");
        }
    }

    private void MarkAnnotationObjects(int index) {
        if (currentAnnotations != null && currentAnnotations.Length > index) {
            for (int i = 0; i < currentAnnotations.Length; i++) {
                if (i == index) {
                    currentAnnotations[i].GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                } else {
                    currentAnnotations[i].GetComponent<Renderer>().material = AnnotationMaterial;
                }
            }
        } else {
            Debug.Log("Tried to click unavailable annotation");
        }
    }

    private void ClickText0(GaMRButton sender) {
        ClickedText(0);
    }

    private void ClickText1(GaMRButton sender) {
        ClickedText(1);
    }

    private void ClickText2(GaMRButton sender) {
        ClickedText(2);
    }

    private void ClickText3(GaMRButton sender) {
        ClickedText(3);
    }

    private void ClickText4(GaMRButton sender) {
        ClickedText(4);
    }

    private void Annotation0Clicked() {
        ClickedAnnotation(0);
    }

    private void Annotation1Clicked() {
        ClickedAnnotation(1);
    }

    private void Annotation2Clicked() {
        ClickedAnnotation(2);
    }

    private void Annotation3Clicked() {
        ClickedAnnotation(3);
    }

    private void Annotation4Clicked() {
        ClickedAnnotation(4);
    }
}
