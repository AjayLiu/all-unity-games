using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class CustomEditor : MonoBehaviour {

    const int MAX_CUSTOM_COUNT = 1000;

    List<GameObject> elements = new List<GameObject>();

    [SerializeField]
    InputField customTitle;

    public GameObject elementPrefab;   

	// Use this for initialization
	void Start () {
    }

    void Update() {
    }

    public void AddElement() {

        print("ADD ELEMENT");

        GameObject instance = Instantiate(elementPrefab, GameObject.Find("GridWithOurElements").transform);

        Button[] buttons = new Button[2];
        buttons = instance.GetComponentsInChildren<Button>();
        if(buttons[0].gameObject.name == "DeleteButton") {
            buttons[0].onClick.AddListener(delegate { RemoveElement(instance); });
        } else {
            buttons[1].onClick.AddListener(delegate { RemoveElement(instance); });
        }

    }

    public void RemoveElement(GameObject caller) {
        print("REMOVE");

        elements.Remove(caller);
        Destroy(caller);
    }

    public void Submit() {

        GameObject[] temp = GameObject.FindGameObjectsWithTag("CUSTOM_ELEMENT");

        for(int i = 0; i < temp.Length; i++) {
            elements.Add(temp[i]);
        }

        string package;

        package = customTitle.text + "[title]";

        print("ELEMENT COUNT: " + elements.Count);

        for(int i = 0; i < elements.Count; i++) {
            package += elements[i].GetComponent<InputField>().text;
            if(i != elements.Count-1)
                package += "[word]";
        }

        
        int existingCustomCount = 0;

        for(int i = 0; i < MAX_CUSTOM_COUNT; i++) {
            if(!PlayerPrefs.HasKey("CUSTOM " + i)) {
                existingCustomCount = i;
                break;
            }
        }

        print(package);
        print(existingCustomCount);

        PlayerPrefs.SetString("CUSTOM " + existingCustomCount, package);
        PlayerPrefs.SetInt("customDeckCount", existingCustomCount+1);

    }

    public void ReturnToMenu() {
        SceneManager.LoadScene("MenuScene");
    }
}
