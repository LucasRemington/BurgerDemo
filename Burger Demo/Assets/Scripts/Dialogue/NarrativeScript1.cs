using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeScript1 : MonoBehaviour {

    //critical game objects
    public GameObject MainCamera;
    public NarrativeManager nm;
    public DialogHolder dh;
    public GameObject player;

    public Dialogue dennis1; // specific dialogue modified by player choice
    //public string[] choiceFor_dennis1; //string holding the dialogue for choice changes. Kept in for template only
    public bool animationFlag; //set from animation events to get timing right
    public bool timerFlag; //set by timer to ger timing right
    public Animator playerAnim;
    public Animator dennisAnim;

    public GameObject masterHologram;

    private void Start()
    {
        MainCamera = GameObject.FindWithTag("MainCamera");
        nm = MainCamera.GetComponent<NarrativeManager>();
        dh = GetComponent<DialogHolder>();
        StartCoroutine(battleEarly());
    }

    public IEnumerator battleEarly ()
    {
        Debug.Log("coroutine");
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.J));
        Debug.Log("j");
        StartCoroutine(nm.bt.StartBattle(masterHologram));
        yield return new WaitUntil(() => nm.bt.battling == true);
        StartCoroutine(dh.GenericFirstConvo(2, true));
    }

    public IEnumerator eventOne() //first event. Eventually, this will be the 'master function' calling shit in order via coroutines.
    {
        yield return new WaitUntil(() => player.activeInHierarchy == true);
        yield return new WaitUntil(() => nm.room == 1 && animationFlag == true); //change this to pull from gameManager + flag set from animation event 
        animationFlag = false;
        StartCoroutine(dh.GenericFirstConvo(0, false));
        yield return new WaitUntil(() => dh.scriptedConvoDone[0] == true);
        yield return new WaitUntil(() => nm.room == 2 && animationFlag == true); //change this to pull from gameManager + flag set from animation event 
        animationFlag = false;
        StartCoroutine(dh.GenericFirstConvo(1, false));
        nm.bt.StartBattle(masterHologram);
        yield return new WaitUntil(() => dh.scriptedConvoDone[1] == true && nm.bt.battling == true);
        StartCoroutine(dh.GenericFirstConvo(2, true));
        yield return new WaitUntil(() => nm.room == 2 && animationFlag == true); //change this to wait until combat finishes + flag set from animation event 
        animationFlag = false;
        StartCoroutine(dh.GenericFirstConvo(3, false));
        nm.ev++;
        nm.CheckEvent();
    }

    public void convoChecker(int dia, int scriptedConvo) //if the conversation has events, they're called from here. If the conversation has no events, this should immediately break.
    {
        switch (dia)
        {
            case 0:
                StartCoroutine(convo0Events(dia, scriptedConvo));
                break;
            case 1:
                StartCoroutine(convo1Events(dia, scriptedConvo));
                break;
            case 2:
                StartCoroutine(convo2Events(dia, scriptedConvo));
                break;
            case 3:
                StartCoroutine(convo2Events(dia, scriptedConvo));
                break;
        }
    }

    IEnumerator convo0Events(int dia, int scriptedConvo) //called from convochecker. These are where 'events' throughout conversations like people turning around or walking should be called.
    {
        switch (scriptedConvo) //requires setting a lot of animation bools for Dennis and Player.
        {
            case 0: //the convo starts with Dennis looking up.
                dh.ongoingEvent = true;
                yield return new WaitForSeconds(0.1f); //there just to make this compile

                dh.ongoingEvent = false;
                break;
            case 1:
                dh.ongoingEvent = true;


                dh.ongoingEvent = false;
                break;
            case 2:
                dh.ongoingEvent = true;


                dh.ongoingEvent = false;
                break;
            case 3:
                dh.ongoingEvent = true;

                dh.ongoingEvent = false;
                break;
        }
    }

    IEnumerator convo1Events(int dia, int scriptedConvo) //called from convochecker. These are where 'events' throughout conversations like people turning around or walking should be called.
    {
        switch (scriptedConvo)
        {
            case 0:
                break;
            case 1:
                dh.ongoingEvent = true;
                StartCoroutine(nm.isQuestion(dh.Scripted[dia], dh.scriptedConvo[scriptedConvo]));
                yield return new WaitUntil(() => nm.dbChoiceSS == true);
                StartCoroutine(dh.choiceChecker());
                StartCoroutine(SecondTimer(1.25f));
                yield return new WaitUntil(() => timerFlag == true); //normally is dh.choiceMade == true
                Debug.Log("timer true");
                if (dh.choiceSelected == 1)
                {
                    Debug.Log("choice 1 made");
                    //dennis1.DialogItems[2].DialogueText = choiceFor_dennis1[0];
                }
                else if (dh.choiceSelected == 2)
                {
                    Debug.Log("choice 2 made");
                    //dennis1.DialogItems[2].DialogueText = choiceFor_dennis1[1];
                } else if (dh.choiceSelected == 2)
                {
                    Debug.Log("no choice made");
                }
                dh.ongoingEvent = false;
                dh.autoAdvance = true; //both should be flagged - ineffcient, might streamline later
                nm.autoAdvance = true;
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }

    IEnumerator convo2Events(int dia, int scriptedConvo) //called from convochecker. These are where 'events' throughout conversations like people turning around or walking should be called.
    {
        // this is a weird one, since it's integrated with combat. It's also really long. Will take time, will likely have bugs. Will possibly require a TutorialCombat script extension or something.
        switch (scriptedConvo)
        {
            case 0:
                dh.ongoingEvent = true;
                yield return new WaitForSeconds(0.1f); //there just to make this compile
                dh.ongoingEvent = false;
                break;
            case 1:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 2:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 3:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 4:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 5:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 6:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 7:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 8:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 9:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 10:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 11:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 12:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 13:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 14:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 15:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 16:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 17:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 18:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 19:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 20:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 21:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 22:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 23:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 24:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 25:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 26:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 27:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
        }
    }

    IEnumerator convo3Events(int dia, int scriptedConvo) //called from convochecker. These are where 'events' throughout conversations like people turning around or walking should be called.
    {
        switch (scriptedConvo) //requires setting a lot of animation bools for Dennis and Player.
        {
            case 0: //the convo starts with Dennis looking up.
                dh.ongoingEvent = true;
                yield return new WaitForSeconds(0.1f); //there just to make this compile

                dh.ongoingEvent = false;
                break;
            case 1:
                dh.ongoingEvent = true;


                dh.ongoingEvent = false;
                break;
            case 2:
                dh.ongoingEvent = true;


                dh.ongoingEvent = false;
                break;
        }
    }

    IEnumerator SecondTimer (float time)
    {
        timerFlag = false;
        yield return new WaitForSeconds(time);
        timerFlag = true;
    }

}
