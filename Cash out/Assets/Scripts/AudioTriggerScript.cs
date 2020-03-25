using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTriggerScript : MonoBehaviour
{

    GameControllerScript game;

    [System.Serializable]
    public struct AudioInfo {
        public AudioClip clip;        
        public TextAsset subtitle;

        public AudioInfo(AudioClip clip, TextAsset subs) {
            this.clip = clip;
            this.subtitle = subs;
        }
    };

    public static Queue<int> playOrder = new Queue<int>();

    AudioSource source;
    public List<AudioInfo> audioInfos;
    public List<AudioInfo> audioQueue;

    public bool autoPlayQueue = true;
    public bool loopLast = true;
    public bool mustPlayIndependent = false;
    public bool finishedQueue = false;
    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        source = GetComponent<AudioSource>();
        if (autoPlayQueue)
            StartCoroutine(PlayNextInQueue(0, mustPlayIndependent));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool isPlaying;
    public IEnumerator PlayNextInQueue(float delay, bool mustPlayIndependent) {        
        yield return new WaitForSecondsRealtime(delay);

        if (audioQueue.Count > 0) {
            isPlaying = true;
            if (mustPlayIndependent) {
                int myID = Random.Range(int.MinValue, int.MaxValue);
                playOrder.Enqueue(myID);
                //wait until its my turn to play
                yield return new WaitUntil(() => playOrder.Peek() == myID);
            }

            //last in queue
            if (audioQueue.Count == 1) {                
                source.loop = loopLast;
            }

            source.clip = audioQueue[0].clip;
            if(audioQueue[0].subtitle != null)
                game.StartCoroutine(game.DisplaySubtitles(audioQueue[0].subtitle.text, audioQueue[0].clip.length));
            source.Play();

            yield return new WaitForSecondsRealtime(0.1f);


            yield return new WaitWhile(() => source.isPlaying);

            if(audioQueue.Count > 0)
                audioQueue.RemoveAt(0);

            //finished queue
            finishedQueue = audioQueue.Count == 0; 

            if (mustPlayIndependent) {
                playOrder.Dequeue();
            }

            isPlaying = false;

            if (autoPlayQueue) {
                StartCoroutine(PlayNextInQueue(0, mustPlayIndependent));
            }
            
        }        
    }

    public void PlayRandomFromList(float delay, bool independent){
        audioQueue.Add(audioInfos[Random.Range(0, audioInfos.Count)]);
        StartCoroutine(PlayNextInQueue(delay, independent));
    }
}
