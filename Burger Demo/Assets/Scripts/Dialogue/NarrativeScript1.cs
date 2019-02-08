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
    public string[] choiceFor_dennis1; //string holding the dialogue

    private void Start()
    {
        MainCamera = GameObject.FindWithTag("MainCamera");
        nm = MainCamera.GetComponent<NarrativeManager>();
        dh = GetComponent<DialogHolder>();
        StartCoroutine(tempCombatCaller());
    }

    public IEnumerator eventOne() //first event. Eventually, this will be the 'master function' calling shit in order via coroutines.
    {
        yield return new WaitUntil(() => player.activeInHierarchy == true);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.I));
        StartCoroutine(dh.GenericFirstConvo(0, false));
        yield return new WaitUntil(() => nm.room == 1);
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
        }
    }

    IEnumerator convo0Events(int dia, int scriptedConvo) //called from convochecker. These are where 'events' throughout conversations like people turning around or walking should be called.
    {
        switch (scriptedConvo)
        {
            case 0:
                dh.ongoingEvent = true;
                Debug.Log("press l");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.L));
                dh.ongoingEvent = false;
                break;
            case 1:
                dh.ongoingEvent = true;
                StartCoroutine(nm.isQuestion(dh.Scripted[dia], dh.scriptedConvo[scriptedConvo]));
                yield return new WaitUntil(() => nm.dbChoiceSS == true);
                StartCoroutine(dh.choiceChecker());
                yield return new WaitUntil(() => dh.choiceMade == true);
                if (dh.choiceSelected == 1)
                {
                    Debug.Log("choice 1 made");
                    dennis1.DialogItems[2].DialogueText = choiceFor_dennis1[0];
                }
                if (dh.choiceSelected == 2)
                {
                    Debug.Log("choice 2 made");
                    dennis1.DialogItems[2].DialogueText = choiceFor_dennis1[1];
                }
                dh.ongoingEvent = false;
                break;
            case 2:
                break;
            case 3:
                dh.ongoingEvent = true;
                Debug.Log("press c");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.C));
                dh.ongoingEvent = false;
                break;
        }
    }

    IEnumerator tempCombatCaller() //temp script, won't be in final
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.P));
        nm.combat = true;
        //yield return new WaitUntil(() => combat == true && combatText == true);
        Debug.Log("master start");
        StartCoroutine(dh.GenericFirstConvo(2, true));
    }

}
