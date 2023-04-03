using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine.UI;
using TMPro;
using static Globals;
using Microsoft.MixedReality.Toolkit.Input;

public class MenuControl : MonoBehaviour
{
    // Setup
    private int numButtons = 0;
    private int numTargets = 10;
    private int controlScheme = -1; // 1, 2, or 3

    private int count = 0;
    public GameObject grid, scroll, button;
    private EMGReader emgReader;
    private ScrollingObjectCollection so;
    private string control = "";
    private double debounceTime;
    private float speed;
    private bool toggleState = true;
    private int selectedButton;
    private List<GameObject> buttons;
    private int currentButton;
    private System.Random rand = new System.Random(0);
    public Material baseMaterial;
    public TextMeshPro label;
    private string activeButtons = "";
    public GameObject mainMenu;
    private float startTime = 0;

    void Awake()
    {
      buttons = new List<GameObject>();
      selectedButton = 3;
  }

    void UpdateMenu()
    {
      FindObjectOfType<GridObjectCollection>().UpdateCollection();
      FindObjectOfType<ScrollingObjectCollection>().UpdateContent();
    }

    void Update()
    {
      if(controlScheme != -1 && numButtons != 0) {
        if(startTime == 0) {
          startTime = Time.time;
        }
        mainMenu.SetActive(false);
        if(Time.time - startTime < 3) {
          if(Time.time - startTime < 1) {
            label.text = "Starting in... 3";
          } else if (Time.time - startTime < 2) {
            label.text = "Starting in... 2";
          } else {
            label.text = "Starting in... 1";
          }
        } else if(count == numTargets) {
          label.text = "Done";
          Destroy(FindObjectOfType<GridObjectCollection>().gameObject);
          //Reset
          Reset();
        } else {
          if(buttons.Count == 0) {
            so = FindObjectOfType<ScrollingObjectCollection>();
            Globals.logger.writeDebug("Menu Control Started" + ",Control Scheme: " + controlScheme + ",Num Buttons: " + numButtons);
            ButtonSetup();
            RandomButtonHighlight();
            if(controlScheme == 1 && buttons.Count > 0) {
              select(buttons[selectedButton - 1]);
            } else {
              selectedButton = 0;
              so.MoveByTiers(Mathf.RoundToInt(-10));
            }
          }
          UpdateMenu();
          if (Input.GetKeyDown(KeyCode.O)) {
            UpScroll(1);
          } else if (Input.GetKeyDown(KeyCode.L))  {
            DownScroll(1);
          } else if (Input.GetKeyDown(KeyCode.P))  {
            if(count < 20){
              ButtonClicked(selectedButton - 1);
            }   
          }
          label.text = "Button " + (currentButton + 1); 
          Log();
        } 
      } else {
          mainMenu.SetActive(true);
      }
    }

    void Reset() 
    {
      numButtons = 0;
      controlScheme = -1;
      startTime = 0;
      selectedButton = 3;
      buttons = new List<GameObject>();
      count=0;
      activeButtons="";
    }

    void ButtonSetup() 
    {
      GameObject gridNew = Instantiate(grid, so.transform.GetChild(0));
      foreach (Transform child in grid.transform) {
        GameObject.Destroy(child.gameObject);
      }
      for(int i=0; i<numButtons; i++) {
        var nButton = Instantiate(button, gridNew.transform);
        nButton.SetActive(true);
        nButton.name = "Button_" + (i+1);
        nButton.transform.GetChild(3).gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Button " + (i+1);
        nButton.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
        nButton.transform.GetChild(2).gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = false;

        buttons.Add(nButton);
      }

      // Add listeners for each button.
      int x = 0;
      foreach (GameObject btn in buttons) {
        int j = x;
        btn.GetComponent<Interactable>().OnClick.AddListener(() => { ButtonClicked(j); });
        x++;
      }

      UpdateMenu();
    }

    void DownScroll(int speed) {
        unselect(buttons[selectedButton - 1]);
        if (selectedButton < buttons.Count) {
          selectedButton += 1;
        }
        if (selectedButton > 3) {
          so.MoveByTiers(Mathf.RoundToInt(1 * speed));
        }
        select(buttons[selectedButton - 1]);
        UpdateMenu();
    }

    void UpScroll(int speed) {
        unselect(buttons[selectedButton - 1]);
        if (selectedButton > 1) {
          selectedButton -= 1;
        }
        if (selectedButton < buttons.Count - 2) {
          so.MoveByTiers(Mathf.RoundToInt(-1 * speed));
        }
        select(buttons[selectedButton - 1]);
        UpdateMenu();
    }
    void select(GameObject btn) {
      // Add Arrows
      btn.transform.GetChild(2).gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = true;
      btn.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = true;

      // Change Text
      btn.transform.GetChild(3).gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().fontStyle = (FontStyles) FontStyle.Bold;
      // btn.transform.GetChild(3).gameObject.transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3((float) 0, 0, 0);
    }
    void unselect(GameObject btn) {
      // Remove Arrows
      btn.transform.GetChild(2).gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = false;
      btn.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;

      // Change Text
      btn.transform.GetChild(3).gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().fontStyle = (FontStyles) FontStyle.Normal;
      // btn.transform.GetChild(3).gameObject.transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
    }
    void ButtonClicked(int buttonNumber){
      Globals.logger.writeDebug("Button Clicked: " + buttonNumber);
      if(currentButton == buttonNumber){
        count = count + 1;
        RandomButtonHighlight();
      }
    }

    public void RandomButtonHighlight(){
      
      if (currentButton < buttons.Count) {
        Material a = new Material(baseMaterial);
        a.SetInt("_Iridescence", 1);
        a.SetInt("_EnvironmentColoring", 0);
        buttons[currentButton].transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = a;
      }
      
      currentButton = rand.Next(0,numButtons);
      if (currentButton < buttons.Count) {
        Material b = new Material(baseMaterial);
        b.SetInt("_Iridescence", 0);
        b.SetInt("_EnvironmentColoring", 1);
        b.SetColor("_EnvironmentColorZ", Color.red);
        buttons[currentButton].transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = b;
      }
    }

    public void GetActiveButtons() {
      string activeButtonsTemp = "";
      for(int i=0; i<buttons.Count; i++) {
        if(buttons[i].transform.GetChild(3).gameObject.transform.GetChild(0).gameObject.activeSelf) {
          activeButtonsTemp += i + ",";
        }
      }
      activeButtons = activeButtonsTemp;
    }

    void Log() {
      Globals.logger.writeDebug("Selected Button: " + selectedButton + ",Current Button: " + currentButton + ",Active Buttons:" + activeButtons);
    }


    // Button Controllers 
    public void Control1() {controlScheme=1;}

    public void Control2() {controlScheme=2;}

    public void Control3() {controlScheme=3;}

    public void FiveButtons() {numButtons=5;}

    public void TenButtons() {numButtons=10;}

    public void TwentyButtons() {numButtons=20;}
}
