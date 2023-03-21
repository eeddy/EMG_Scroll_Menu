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
    private int numButtons = 10;

    public GameObject button, grid, scroll;
    private GridObjectCollection gc;
    private ScrollingObjectCollection so;
    private EMGReader emgReader;
    private string control = "";
    private double debounceTime;
    private float speed;
    private int count = 0;
    private bool toggleState = true;
    private int selectedButton;
    private List<GameObject> buttons;


    void Awake()
    {
        gc = FindObjectOfType<GridObjectCollection>();
        so = FindObjectOfType<ScrollingObjectCollection>();

        buttons = new List<GameObject>();

        scroll.SetActive(false);

        // Add the number of buttons desired
        for(int i=0; i<numButtons; i++) {
            var nButton = Instantiate(button, grid.transform);
            nButton.SetActive(true);
            nButton.name = "Button_" + (i+1);
            nButton.transform.GetChild(3).gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Button " + (i+1);
            nButton.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;
            buttons.Add(nButton);
        }

        // Add Selection
        // var rTriangle = Instantiate(TriangleGraphic, so.transform);
        // rTriangle.name = "Triangle_Right";

        scroll.SetActive(true);
    }

    void Start() {
        // Do nothing for now - but leave this.
        Globals.logger.writeDebug("Menu Control Started!");
        selectedButton = 3;
        // selectedMaterial = new Material(Shader.Find("Specular"));
        // Image1 =
    }

    void Update()
    {
      if (Input.GetKeyDown(KeyCode.O) || Input.GetKeyDown(KeyCode.L)) {
        Globals.logger.writeDebug("Key Pressed: " + Input.inputString);
        FixedUpdate();
      }

      // FixedUpdate();
    }

    void UpdateMenu()
    {
        gc.UpdateCollection();
        so.UpdateContent();
    }

    void FixedUpdate()
    {
        UpdateMenu();
        int speed = 1;

        if (Input.GetKeyDown(KeyCode.O)) {
            UpScroll(speed);
        } else if (Input.GetKeyDown(KeyCode.L))  {
            DownScroll(speed);
        }
    }

    void DownScroll(int speed) {
        Globals.logger.writeDebug("DownScroll");
        buttons[selectedButton - 1].transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;

        if (selectedButton < buttons.Count) {
          selectedButton += 1;
        }
        if (selectedButton > 3) {
          so.MoveByTiers(Mathf.RoundToInt(1 * speed));
        }

        buttons[selectedButton - 1].transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = true;

        Globals.logger.writeDebug("New Selected button: " + selectedButton);
        UpdateMenu();
    }

    void UpScroll(int speed) {
        Globals.logger.writeDebug("UpScroll");
        buttons[selectedButton - 1].transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;

        if (selectedButton > 1) {
          selectedButton -= 1;
        }
        if (selectedButton < buttons.Count - 2) {
          so.MoveByTiers(Mathf.RoundToInt(-1 * speed));
        }

        buttons[selectedButton - 1].transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = true;


        Globals.logger.writeDebug("New Selected button: " + selectedButton);
        UpdateMenu();
    }
}
