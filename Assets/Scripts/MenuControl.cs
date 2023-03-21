using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine.UI;
using TMPro;
using static Globals;

public class MenuControl : MonoBehaviour
{
    private int numButtons = 20;

    public GameObject button, grid, scroll;
    private GridObjectCollection gc;
    private ScrollingObjectCollection so;
    private EMGReader emgReader;
    private string control = "";
    private double debounceTime;
    private float speed;
    private int count = 0;
    private bool toggleState = true;


    void Awake()
    {
        gc = FindObjectOfType<GridObjectCollection>();
        so = FindObjectOfType<ScrollingObjectCollection>();

        scroll.SetActive(false);

        // Add the number of buttons desired
        for(int i=0; i<numButtons; i++) {
            var nButton = Instantiate(button, grid.transform);
            nButton.SetActive(true);
            nButton.name = "Button_" + (i+1);
            nButton.transform.GetChild(3).gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Button " + (i+1);
        }

        scroll.SetActive(true);
    }

    void Start() {
        // Do nothing for now - but leave this.
        Globals.logger.writeDebug("Here!");
    }

    void Update()
    {
      if (Input.inputString != "") {
        Globals.logger.writeDebug("Key Pressed: " + Input.inputString);
      }
      if (Input.GetKeyDown("space"))
      {
          print("space key was pressed");
      }
    }

    void UpdateMenu()
    {
        gc.UpdateCollection();
        so.UpdateContent();
        if (Input.GetKeyUp(KeyCode.T)) {
          Globals.logger.writeDebug("Hit " + "T");
          toggleState = true;
        }

    }

    void FixedUpdate()
    {
        UpdateMenu();

        if (Input.GetKeyUp(KeyCode.O)) {
          Globals.logger.writeDebug("Hit " + "O");
            DownScroll(speed);
        } else if (Input.GetKeyUp(KeyCode.L))  {
          Globals.logger.writeDebug("Hit " + "L");
            UpScroll(speed);
        } else if(control == "1") {

        }
    }

    void DownScroll(float speed) {
        so.MoveByTiers(Mathf.RoundToInt(10 * speed));
        UpdateMenu();
    }

    void UpScroll(float speed) {
        so.MoveByTiers(Mathf.RoundToInt(-10 * speed));
        UpdateMenu();
    }
}
