using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;

public class SceneManager : MonoBehaviour
{
    public GameObject template;
    public GameObject plant, painting, book, pillow;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Here!");
    }

    public void HousePlantClicked() {
        Debug.Log("House Plant");
        CreateObject(plant, new Vector3(0.1f, 0.1f, 0.1f));
    }

    public void HousePaintingClicked() {
        Debug.Log("Painting");
        CreateObject(painting, new Vector3(1,1,1));
    }

    public void BooksClicked() {
        Debug.Log("Books");
        CreateObject(book, new Vector3(1,1,1));
    }

    public void PillowClicked() {
        Debug.Log("Pillow");
        CreateObject(pillow, new Vector3(1,1,1));
    }

    void CreateObject(GameObject obj, Vector3 vec) {
        GameObject nObj = GameObject.Instantiate(obj,new Vector3(0,0.2f,0),Quaternion.identity);
        nObj.transform.localScale = vec;
        nObj.AddComponent<BoxCollider>();
        nObj.AddComponent<ObjectManipulator>();
    }
}
