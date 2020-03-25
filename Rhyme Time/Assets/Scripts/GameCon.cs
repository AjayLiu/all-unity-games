using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

class Word {
    public string word;
    public bool isCommon;
    public int syllableCount;
    public bool isPerfect = true;
}
class CommonWord {
    public string word;
}



public class GameCon : MonoBehaviour {

    string StripHTML(string input) {
        //return Regex.Replace(input, "<.*?>", "");
        var remainTag = "b";
        var pattern = string.Format("(</?(?!{0})[^<>]*(?<!{0})>)", remainTag);
        var result = Regex.Replace(input, pattern, "");
        return result;
    }

    // Use this for initialization
    string url = "https://www.rhymezone.com/r/rhyme.cgi?Word=hate&typeofrhyme=perfect&org1=syl&org2=l&org3=y";
    string commonWordsURL = "https://www.ef.com/english-resources/english-vocabulary/top-3000-words/";



    void Awake() {
        //StartCoroutine("Go");
        StartCoroutine("CommonWords");
    }

    void Start () {
        timeLeft = startTimeAmount + 1;

        msgOriginPos = msgOriginTransform.position;
    }





    #region html stuff

    //stores the www information
    string s;

    IEnumerator Go() {
        using (WWW www = new WWW(url)) {
            yield return www;
            s = www.text;
            Decode(s, false);
            //Decode(s, true);
        }
    }

    IEnumerator CommonWords() {
        using (WWW www = new WWW(commonWordsURL)) {
            yield return www;
            s = www.text;
            GetCommonWords(s);           
        }
    }

    List<CommonWord> commonWords = new List<CommonWord>();


    string commonCue = "functional.</p>";

    void GetCommonWords(string s) {
        string commonList;
        commonList = s.Substring(s.IndexOf(commonCue)+19, s.IndexOf("zone</p>") - s.IndexOf(commonCue)+8);

        while (commonList.Contains("<br />")) {
            string word, temp;
            word = commonList.Substring(0, commonList.IndexOf("<br />"));

            CommonWord cw = new CommonWord();
            cw.word = word;            
            commonWords.Add(cw);

            temp = commonList.Remove(0, commonList.IndexOf("<br />")+6);
            commonList = temp;
        }

        CommonWord zonecw = new CommonWord();
        zonecw.word = "zone";

        commonWords.Add(zonecw);

        NewSurvivalWord();

    }

    string CUE = "results)";

    List<Word> words = new List<Word>();
    List<Word> nearRhymes = new List<Word>();


    void Decode(string s, bool isDecodeNearRhymes) {   
        
        if(!isDecodeNearRhymes) {     
            //TRANSFORM FROM HTML TO NORMAL TEXT
            int index = s.IndexOf(CUE);
            string bloob = s.Remove(0, index + 27);
            string blob = "";
            if (bloob.Contains("var rz_snippets")) {
                blob = bloob.Remove(bloob.IndexOf("var rz_snippets"), bloob.Length - bloob.IndexOf("var rz_snippets"));
            } else {
                StartCoroutine(DisplayMessage("No rhymes"));
                NewSurvivalWord();
                return;
            }
            string blb = StripHTML(blob);

            string _list = blb.Replace("&nbsp;"," ");
            string list = _list.Replace("<br>", "*");

            //TRANSFORM INTO A LIST OF WORDS        
            int sCount = 0;
            string w ="";

            int syllableChangeCount = 0;

            bool isTransferCase = false;

            while (list.Contains(",")) {

                isTransferCase = false;

                Word ins = new Word();
                w = list.Substring(0, list.IndexOf(','));

                //On syllable count change            
                if (w.Contains(":*")) {

                    string t = w.Substring(0, w.IndexOf("*"));
                    string missWord = w.Substring(w.IndexOf(":*") + 2, w.Length - w.IndexOf(":*") - 2);

                    syllableChangeCount++;

                    string syllableString = Regex.Match(w, @"\d+").Value;
                    sCount = int.Parse(syllableString);

                    if (syllableChangeCount == 1) {
                        w = missWord;
                    } else {
                        w = t;
                        isTransferCase = true;
                    }                
                }
            
                if(isTransferCase)
                    ins.syllableCount = sCount-1;
                else
                    ins.syllableCount = sCount;



                //Check if common or not
                string word1 = "";
                string word2 = "";
                string word3 = "";

                if (w.Contains("<b>")) {
                    ins.isCommon = false;
                    word1 = w.Replace("<b>", "");
                    word2 = word1.Replace("</b>", "");
                    word3 = word2.Replace("\n", "");
                } else {
                    ins.isCommon = true;
                    word3 = w.Replace("\n", "");
                }



                ins.word = word3;

                words.Add(ins);


                //Cut off the registered word from the list
                if(isTransferCase) {
                    string l = list.Remove(0, list.IndexOf(":*")+2);
                    list = l;

                    isTransferCase = false;
                } else {
                    string temp = list.Remove(0, list.IndexOf(',')+1);
                    list = temp;
                }


            
            
            }



            //Register the last rhyme (not caught due to lack of comma)
            Word w_ins = new Word();

            if (list.Contains(":*")) {
                string t = list.Substring(0, list.IndexOf("*"));
                string missWord = list.Substring(list.IndexOf(":*") + 2, list.Length - list.IndexOf(":*")-2);


                string mw = missWord.Replace("*", "");
                missWord = mw;

                syllableChangeCount++;

                string syllableString = Regex.Match(list, @"\d+").Value;
                sCount = int.Parse(syllableString);


                //WORD BEFORE THE "n syllables:*"
                w = t.Trim();

                string l1 = "";
                string l2 = "";
                string l3 = "";

                if (w.Contains("<b>")) {
                    w_ins.isCommon = false;
                    l1 = w.Replace("<b>", "");
                    l2 = l1.Replace("</b>", "");
                    l3 = l2.Replace("\n", "");
                } else {
                    w_ins.isCommon = true;
                    l3 = w.Replace("\n", "");
                }

                w_ins.word = l3;
                w_ins.syllableCount = sCount - 1;
                words.Add(w_ins);


                //LAST WORD
                Word lastWord = new Word();

                w = missWord.Trim();
                l1 = "";
                l2 = "";
                l3 = "";

                if (w.Contains("<b>")) {
                    lastWord.isCommon = false;
                    l1 = w.Replace("<b>", "");
                    l2 = l1.Replace("</b>", "");
                    l3 = l2.Replace("\n", "");
                } else {
                    lastWord.isCommon = true;
                    l3 = w.Replace("\n", "");
                }

                lastWord.word = l3;
                lastWord.syllableCount = sCount;
                words.Add(lastWord);
            } else {

                string list1 = "";
                string list2 = "";
                string list3 = "";
                string listz = "";
                if (w.Contains("<b>")) {
                    w_ins.isCommon = false;
                    list1 = list.Replace("<b>", "");
                    list2 = list1.Replace("</b>", "");
                    listz = list2.Replace("*", "");
                    list3 = listz.Replace("\n", "");
                } else {
                    w_ins.isCommon = true;
                    list2 = list.Replace("\n", "");
                    list3 = list.Replace("*", "");
                }

                w_ins.word = list3;

                words.Add(w_ins);
            }

            int rhymesExisting = 0;

            
            foreach (Word i in words) {
                //print(i.word);
                rhymesExisting++;
            }
            

            //Check if there's enough existing rhymes
            if (rhymesExisting < availableRhymesMinimum) {
                print("NOT ENOUGH RHYMES");
                NewSurvivalWord();
            }
        } else {
            // FIND NEAR RHYMES
            /*
            print("PANIC");
            //TRANSFORM FROM HTML TO NORMAL TEXT
            int index = s.IndexOf(CUE);
            string bloob = s.Remove(0, index + 27);
            string blob = "";
            if (bloob.Contains("var rz_snippets")) {
                blob = bloob.Remove(bloob.IndexOf("var rz_snippets"), bloob.Length - bloob.IndexOf("var rz_snippets"));
            } else {
                StartCoroutine(DisplayMessage("No rhymes"));
                NewSurvivalWord();
                return;
            }
            string blb = StripHTML(blob);

            string _list = blb.Replace("&nbsp;", " ");
            string list = _list.Replace("<br>", "*");

            //TRANSFORM INTO A LIST OF WORDS        
            int sCount = 0;
            string w = "";

            int syllableChangeCount = 0;

            bool isTransferCase = false;

            while (list.Contains(",")) {

                isTransferCase = false;

                Word ins = new Word();
                w = list.Substring(0, list.IndexOf(','));

                //On syllable count change            
                if (w.Contains(":*")) {

                    string t = w.Substring(0, w.IndexOf("*"));
                    string missWord = w.Substring(w.IndexOf(":*") + 2, w.Length - w.IndexOf(":*") - 2);

                    syllableChangeCount++;

                    string syllableString = Regex.Match(w, @"\d+").Value;
                    sCount = int.Parse(syllableString);

                    if (syllableChangeCount == 1) {
                        w = missWord;
                    } else {
                        w = t;
                        isTransferCase = true;
                    }
                }

                if (isTransferCase)
                    ins.syllableCount = sCount - 1;
                else
                    ins.syllableCount = sCount;



                //Check if common or not
                string word1 = "";
                string word2 = "";
                string word3 = "";

                if (w.Contains("<b>")) {
                    ins.isCommon = false;
                    word1 = w.Replace("<b>", "");
                    word2 = word1.Replace("</b>", "");
                    word3 = word2.Replace("\n", "");
                } else {
                    ins.isCommon = true;
                    word3 = w.Replace("\n", "");
                }



                ins.word = word3;

                words.Add(ins);


                //Cut off the registered word from the list
                if (isTransferCase) {
                    string l = list.Remove(0, list.IndexOf(":*") + 2);
                    list = l;

                    isTransferCase = false;
                } else {
                    string temp = list.Remove(0, list.IndexOf(',') + 1);
                    list = temp;
                }
                

        

            }



            //Register the last rhyme (not caught due to lack of comma)
            Word w_ins = new Word();

            if (list.Contains(":*")) {
                string t = list.Substring(0, list.IndexOf("*"));
                string missWord = list.Substring(list.IndexOf(":*") + 2, list.Length - list.IndexOf(":*") - 2);


                string mw = missWord.Replace("*", "");
                missWord = mw;

                syllableChangeCount++;

                string syllableString = Regex.Match(list, @"\d+").Value;
                sCount = int.Parse(syllableString);


                //WORD BEFORE THE "n syllables:*"
                w = t.Trim();

                string l1 = "";
                string l2 = "";
                string l3 = "";

                if (w.Contains("<b>")) {
                    w_ins.isCommon = false;
                    l1 = w.Replace("<b>", "");
                    l2 = l1.Replace("</b>", "");
                    l3 = l2.Replace("\n", "");
                } else {
                    w_ins.isCommon = true;
                    l3 = w.Replace("\n", "");
                }

                w_ins.word = l3;
                w_ins.syllableCount = sCount - 1;
                words.Add(w_ins);


                //LAST WORD
                Word lastWord = new Word();

                w = missWord.Trim();
                l1 = "";
                l2 = "";
                l3 = "";

                if (w.Contains("<b>")) {
                    lastWord.isCommon = false;
                    l1 = w.Replace("<b>", "");
                    l2 = l1.Replace("</b>", "");
                    l3 = l2.Replace("\n", "");
                } else {
                    lastWord.isCommon = true;
                    l3 = w.Replace("\n", "");
                }

                lastWord.word = l3;
                lastWord.syllableCount = sCount;
                words.Add(lastWord);
            } else {

                string list1 = "";
                string list2 = "";
                string list3 = "";
                string listz = "";
                if (w.Contains("<b>")) {
                    w_ins.isCommon = false;
                    list1 = list.Replace("<b>", "");
                    list2 = list1.Replace("</b>", "");
                    listz = list2.Replace("*", "");
                    list3 = listz.Replace("\n", "");
                } else {
                    w_ins.isCommon = true;
                    list2 = list.Replace("\n", "");
                    list3 = list.Replace("*", "");
                }

                w_ins.word = list3;

                words.Add(w_ins);
            }
            */
            int rhymesExisting = 0;

            foreach (Word i in words) {
                print(i.word);
                rhymesExisting++;
            }

            //Check if there's enough existing rhymes
            if (rhymesExisting < availableRhymesMinimum) {
                print("NOT ENOUGH RHYMES");
                NewSurvivalWord();
            }


        }
    }




    #endregion

    #region scoring and input

    public InputField input;
    public Text timerText, completedText;
    public Text pointsMsgText;

    public float startTimeAmount;
    float timeLeft;

    List<string> blacklist = new List<string>();

    public void OnInputChange() {
        if (input.text.Contains("\n")) {
            OnSubmit();
        }
    }

    public void OnRhymeChangeInputChange() {
        if (changeInputField.text.Contains("\n")) {
            OnChangeRhyme();
        }
    }

    public void OnSubmit() {
        
        //REGISTER THE INPUT
        bool isValid = false;
        bool isDupe = false;

        for (int i = 0; i < words.Count; i++) {
            string word = input.text.Trim().ToLower();

            isDupe = false;


            if (word.Equals(words[i].word.Trim().ToLower())) {

                //CHECK IF IT IS IN BLACKLIST (to prevent duplicates)

                for (int j = 0; j < blacklist.Count; j++) {
                    if (word == blacklist[j])
                        isDupe = true;
                }
                if(!isDupe) {
                    blacklist.Add(word);
                    Scoring(words[i]);
                } else {
                    StartCoroutine(DisplayMessage("Duplicate!"));
                }

                isValid = true;
                break;

            }
        }

        if(!isValid) {
            StartCoroutine(DisplayMessage("Nope"));
        } else if(!isDupe) {
            wordsPassed++;
            wordSwitchCounter++;

            if(wordSwitchCounter >= wordsTilSwitch) {
                NewSurvivalWord();
                wordSwitchCounter = 0;
            }
            DisplayScore();

        }

        input.text = "";

    }

    void DisplayScore() {
        completedText.text = wordsPassed.ToString();
    }

    public InputField changeInputField;

    public void OnChangeRhyme() {
        url = "https://www.rhymezone.com/r/rhyme.cgi?Word=" + changeInputField.text.Trim().ToLower().Replace("\n","") + "&typeofrhyme=perfect&org1=syl&org2=l&org3=y";
        StartCoroutine("Go");
        words.Clear();
        changeInputField.text = "";
    }


    void Scoring(Word word) {
        string w = word.word.Trim().ToLower();
        float scoreToAdd;
        scoreToAdd = word.syllableCount * 2f;
        scoreToAdd += 0.25f * w.Length;
        if (word.isCommon)
            scoreToAdd *= 2;

        StartCoroutine(DisplayMessage(scoreToAdd));
    }

    public float bounceHeight;
    public float bounceSpeed;
    public float bounceTime;

    public Transform msgOriginTransform;
    Vector3 msgOriginPos;

    bool isBounce = false, isBounceUp = true;

    IEnumerator DisplayMessage(float score) {

        pointsMsgText.transform.position = msgOriginPos;

        pointsMsgText.text = "+" + score.ToString();
        timeLeft += score;

        //Bounce Up
        isBounce = true;
        isBounceUp = true;

        yield return new WaitForSeconds(bounceTime);

        isBounceUp = false;

        yield return new WaitForSeconds(bounceTime);
        isBounce = false;
        pointsMsgText.text = "";
    }

    IEnumerator DisplayMessage(string msg) {

        pointsMsgText.transform.position = msgOriginPos;


        pointsMsgText.text = msg;

        //Bounce Up
        isBounce = true;
        isBounceUp = true;

        yield return new WaitForSeconds(bounceTime);

        isBounceUp = false;

        yield return new WaitForSeconds(bounceTime);
        isBounce = false;
        pointsMsgText.text = "";
    }

    #endregion

    #region survival mode

    List<string> commonBlacklist = new List<string>();
    public Text targetRhymeText;

    int wordsPassed = 0, wordSwitchCounter = 0;
    public int wordsTilSwitch;

    public int availableRhymesMinimum;

    void NewSurvivalWord() {
        blacklist.Clear();

        int r = Random.Range(0, commonWords.Count);
        CommonWord com = commonWords[r];

        targetRhymeText.text = com.word.Trim().Replace("\n", "");
        url = "https://www.rhymezone.com/r/rhyme.cgi?Word=" + com.word.Trim().ToLower().Replace("\n", "") + "&typeofrhyme=perfect&org1=syl&org2=l&org3=y";

        words.Clear();

        StartCoroutine("Go");


    }

    void DrainTime() {
        timeLeft -= Time.deltaTime;
        int min, sec;
        min = (int)(timeLeft / 60);
        sec = (int)(timeLeft % 60);
        timerText.text = min + ":" + sec;

        if(timeLeft <= 10) {
            timerText.color = Color.red;
        } else {
            timerText.color = Color.yellow;
        }

        if(timeLeft <= 0) {

            if(wordsPassed > PlayerPrefs.GetInt("Highscore")) {
                PlayerPrefs.SetInt("Highscore", wordsPassed);
            }
            PlayerPrefs.SetInt("Score", wordsPassed);

            SceneManager.LoadScene("GameEnd");
            
        }
    }

    public float skipPenalty;

    public void SkipWord() {
        timeLeft -= skipPenalty;
        StartCoroutine(DisplayMessage("-" + skipPenalty));
        wordSwitchCounter = 0;
        NewSurvivalWord();
        
    }


    #endregion

    // Update is called once per frame
    void Update () {

        // DRAINING YOUR SOUUUUULLLL
        DrainTime();


        if (isBounce) {
            if(isBounceUp) {
                pointsMsgText.transform.position = Vector3.Lerp(pointsMsgText.transform.position,
                    new Vector3(pointsMsgText.transform.position.x, pointsMsgText.transform.position.y + bounceHeight), bounceSpeed);
            } else {
                pointsMsgText.transform.position = Vector3.Lerp(pointsMsgText.transform.position,
                    new Vector3(pointsMsgText.transform.position.x, pointsMsgText.transform.position.y - bounceHeight), bounceSpeed);
            }
            
        }
    }
}
