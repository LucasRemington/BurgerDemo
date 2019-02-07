using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeScript1 : MonoBehaviour {

    public GameObject MainCamera;
    public NarrativeManager nm;
    public DialogHolder dh;

    private void Start()
    {
        MainCamera = GameObject.FindWithTag("MainCamera");
        nm = MainCamera.GetComponent<NarrativeManager>();
        dh = GetComponent<DialogHolder>();
        StartCoroutine(tempCombatCaller());
        StartCoroutine(interactableTester());
    }

    public IEnumerator eventOne() //first event. Eventually, this will be the 'master function' calling shit in order via coroutines.
    {
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
                StartCoroutine(convo0Events(scriptedConvo));
                break;
        }
    }

    IEnumerator convo0Events(int scriptedConvo) //called from convochecker. These are where 'events' throughout conversations like people turning around or walking should be called.
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
                Debug.Log("press u");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.U));
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

    IEnumerator interactableTester() //test function, won't be in final
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.O));
        StartCoroutine(dh.GenericInteractable(0));
    }

}
