using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine.UI;
using TMPro;
using static Globals;
using UnityEngine;

public class MenuControl : MonoBehaviour
{
    private int numButtons = 10;

    public GameObject grid, scroll, button;
    private GridObjectCollection gc;
    private ScrollingObjectCollection so;
    private EMGReader emgReader;
    private string control = "";
    private double debounceTime;
    private float speed;
    private bool toggleState = true;
    private int selectedButton;
    private List<GameObject> buttons;


    void Awake()
    {
        gc = FindObjectOfType<GridObjectCollection>();
        so = FindObjectOfType<ScrollingObjectCollection>();

        buttons = new List<GameObject>();

        scroll.SetActive(false);

        for(int i=0; i<numButtons; i++) {
            var nButton = Instantiate(button, grid.transform);
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

        scroll.SetActive(true);
    }

    void Start() {
        Globals.logger.writeDebug("Menu Control Started!");
        selectedButton = 3;

    }

    void Update()
    {
      UpdateMenu();
      if (Input.GetKeyDown(KeyCode.O)) {
        UpScroll(1);
      } else if (Input.GetKeyDown(KeyCode.L))  {
        DownScroll(1);
      } else if (Input.GetKeyDown(KeyCode.P))  {
        ButtonClicked(selectedButton - 1);
      }
    }

    void UpdateMenu()
    {
        gc.UpdateCollection();
        so.UpdateContent();
    }

    void Test() {
      Debug.Log("Clicked.");
    }

    void DownScroll(int speed) {
        Globals.logger.writeDebug("DownScroll");

        unselect(buttons[selectedButton - 1]);

        if (selectedButton < buttons.Count) {
          selectedButton += 1;
        }
        if (selectedButton > 3) {
          so.MoveByTiers(Mathf.RoundToInt(1 * speed));
        }

        select(buttons[selectedButton - 1]);

        Globals.logger.writeDebug("New Selected button: " + selectedButton);
        UpdateMenu();
    }

    void UpScroll(int speed) {
        Globals.logger.writeDebug("UpScroll");

        unselect(buttons[selectedButton - 1]);

        if (selectedButton > 1) {
          selectedButton -= 1;
        }
        if (selectedButton < buttons.Count - 2) {
          so.MoveByTiers(Mathf.RoundToInt(-1 * speed));
        }

        select(buttons[selectedButton - 1]);


        Globals.logger.writeDebug("New Selected button: " + selectedButton);
        UpdateMenu();
    }
    void select(GameObject btn) {
      // Add Arrows
      btn.transform.GetChild(2).gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = true;
      btn.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = true;

      // Change Text
      btn.transform.GetChild(3).gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().fontStyle = (FontStyles) FontStyle.Bold;
      btn.transform.GetChild(3).gameObject.transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3((float) -0.0025, 0, 0);
    }
    void unselect(GameObject btn) {
      // Remove Arrows
      btn.transform.GetChild(2).gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = false;
      btn.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;

      // Change Text
      btn.transform.GetChild(3).gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().fontStyle = (FontStyles) FontStyle.Normal;
      btn.transform.GetChild(3).gameObject.transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(0, (float) -0.00199, 0);
    }
    public void ButtonClicked(int buttonNumber){
      Globals.logger.writeDebug("Button " + buttonNumber + " got clicked!");
      Debug.Log("Button " + buttonNumber + " got clicked!");
    }
}
