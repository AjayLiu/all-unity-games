using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public class DialogueScript : MonoBehaviour {
    GameControl game;
    DialogueCommands comm;
    Text display, skipTextBox;
    public TextAsset rawText;
    string[] lines;
    public char newLineCharacter, commandCharacter;
    int lineIndex = 0;
    public float lineDisplayInterval;

	// Use this for initialization
	void Awake () {
        game = GameObject.Find("Game Controller").GetComponent<GameControl>();
        comm = game.GetComponent<DialogueCommands>();
		display = GetComponent<Text>();
        skipTextBox = transform.GetChild(0).GetComponentInChildren<Text>();
        skipTextBox.gameObject.SetActive(false);
        lines = rawText.text.Split(newLineCharacter);
        StartCoroutine(DisplayText(lines[lineIndex], lineDisplayInterval));
    }

    // Update is called once per frame
    void Update () {
        if(Input.GetKeyDown(KeyCode.Space)){
            SpacePressed();
        }
        
	}

    bool isWaitingForSkip = false;
    bool skipText = false;
    bool lineFinished = false;
    bool allTextFinished = false;

    IEnumerator DisplayText(string text, float intervalTime){

        //if is a command
        string command = text.Trim();
        if (command[0] == commandCharacter && command != null) {
            command = text.Trim().Remove(0, 1);

            string s_parameters;
            s_parameters = command.Substring(command.IndexOf('(') + 1, command.IndexOf(')') - command.IndexOf('(') - 1);

            command = command.Remove(command.IndexOf('('), command.IndexOf(')') - command.IndexOf('(') + 1);
            
            object[] parems = s_parameters.Split(',');
            comm.StartCoroutine(command, parems);

            SwitchNextLine();
        } else {
            skipText = false;
            lineFinished = false;

            string textDisplayed = "";
            for (int i = 0; i < text.Length; i++){
                textDisplayed += text[i];
                display.text = textDisplayed;
                if(!skipText)
                    yield return new WaitForSecondsRealtime(intervalTime);
            }
        }
        lineFinished = true;
        OnLineFinish();
    }

    void SpacePressed(){
        if(lineFinished){
            SwitchNextLine();
        } else {
            skipText = true;
        }

        if(isWaitingForSkip){
            isWaitingForSkip = false;
        }
    }

    void SwitchNextLine() {
        
        //if is last line of entire text
        if (lineIndex == lines.Length - 1) {
            OnAllTextFinish();
        } else {
            skipTextBox.gameObject.SetActive(false);
            lineIndex++;
            StartCoroutine(DisplayText(lines[lineIndex], lineDisplayInterval));
        }
    }

    public float blinkingSkipMessageInterval;

    void OnLineFinish() {
        StartCoroutine(BlinkingSkipMessage(blinkingSkipMessageInterval));
    }


    void OnAllTextFinish(){
        game.SetGameState(GameStates.game);
    }
    
    IEnumerator BlinkingSkipMessage(float interval){

        skipTextBox.gameObject.SetActive(true);

        isWaitingForSkip = true;
        bool isDisplay = true;

        while(isWaitingForSkip){
            yield return new WaitForSecondsRealtime(interval);
            isDisplay = !isDisplay;
            skipTextBox.gameObject.SetActive(isDisplay);
        }

        skipTextBox.gameObject.SetActive(false);
    }

    public void GoToLine(int line){
        DisplayText(lines[line], lineDisplayInterval);
    }
}
