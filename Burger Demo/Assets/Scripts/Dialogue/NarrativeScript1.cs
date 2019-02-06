using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeScript1 : MonoBehaviour {

    public NarrativeManager nm;

    public Dialogue[] Scripted; //each 'dialogue' is a different conversation, each element is a different dialog box. 1 indicates that this takes place during the first event only.
    public int[] scriptedConvo; //used to keep track of the point in conversation one would be at.
    public bool[] scriptedConvoStart; //true when the convo has started
    public bool[] scriptedConvoDone; //used to track which conversations have finished

    public Dialogue[] Interactable; //each 'interactable' is a different interactable 
    public int[] interactConvo; //used to keep track of the point in conversation one would be at.
    public bool[] interactConvoStart; //true when the convo has started
    public bool[] interactConvoDone; //used to track which conversations have finished

    public int sizeOfList; //set equal to the number of items in a dialogue element

    public bool ongoingEvent; //true when a dialogue event is ongoing

    private void Start() //sets certain array lengths equal to scripted or interactable
    {
        nm = GetComponent<NarrativeManager>();
        scriptedConvo = new int [Scripted.Length];
        scriptedConvoStart = new bool [Scripted.Length];
        scriptedConvoDone = new bool [Scripted.Length];
        interactConvo = new int[Interactable.Length];
        interactConvoStart = new bool[Interactable.Length];
        interactConvoDone = new bool[Interactable.Length];
        StartCoroutine(interactableTester()); //test function, won't be in final
    }

    IEnumerator masterCombatStarter() //test function, won't be in final
    {
        yield return new WaitUntil(() => nm.combat == true && nm.combatText == true);
        Debug.Log("master start");
        StartCoroutine(GenericFirstConvo(2, true));
    }

    IEnumerator interactableTester() //test function, won't be in final
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.O));
        StartCoroutine(GenericInteractable(0));
    }

    public IEnumerator eventOne() //first event. Eventually, this will be the 'master function' calling shit in order via coroutines.
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.I));

        StartCoroutine(GenericFirstConvo(0, false));
        StartCoroutine(masterCombatStarter());
        yield return new WaitUntil(() => nm.room == 1);
        nm.ev++;
        nm.CheckEvent();
    }

    IEnumerator GenericFirstConvo(int scriptedConversation, bool inCombat) //whenever a conversation is going to start, pass it to this, indicating the number from the dialog array the conversation is placed into, as well as whether or not it takes place during combat.
    {
        if (scriptedConvoStart[scriptedConversation] == false)
        {
            scriptedConvoStart[scriptedConversation] = true;
            if (inCombat == false)
            {
                StartCoroutine(nm.dialogueBox(true));
            }
            sizeOfList = Scripted[scriptedConversation].DialogItems.Count;
        }
        nm.setTalksprite(Scripted[scriptedConversation], scriptedConvo[scriptedConversation]);
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(nm.AnimateText(Scripted[scriptedConversation], scriptedConvo[scriptedConversation]));
        convoChecker(scriptedConversation, scriptedConvo[scriptedConversation]); //here is where we put a switch(?) statement calling other functions when appropriate
        scriptedConvo[scriptedConversation]++;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) && nm.canAdvance == true && ongoingEvent == false);
        if (scriptedConvo[scriptedConversation] >= sizeOfList)
        {
            StartCoroutine(nm.dialogueEnd());
            scriptedConvoDone[scriptedConversation] = true;
        }
        else
        {
            StartCoroutine(GenericFirstConvo(scriptedConversation, inCombat));
        }
    }

    IEnumerator GenericInteractable(int interactableIdentity)
    {
        if (interactConvoStart[interactableIdentity] == false)
        {
            interactConvoStart[interactableIdentity] = true;
            StartCoroutine(nm.dialogueBox(false));
            sizeOfList = Interactable[interactableIdentity].DialogItems.Count;
        }
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(nm.AnimateText(Interactable[interactableIdentity], interactConvo[interactableIdentity]));
        interactConvo[interactableIdentity]++;
        //here is where we put a switch(?) statement calling other functions when appropriate
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) && nm.canAdvance == true);
        if (interactConvo[interactableIdentity] >= sizeOfList)
        {
            StartCoroutine(nm.dialogueEnd());
            interactConvoDone[interactableIdentity] = true;
        }
        else
        {
            StartCoroutine(GenericInteractable(interactableIdentity));
        }
    } //same as conversation except for interactable objects. These probably could be one function, but it was a little more readable to split them/

    void convoChecker(int dia, int scriptedConvo) //nested switch statements, expanded to other functions 
    {
        switch (dia)
        {
            case 0:
                StartCoroutine(convo1Events(scriptedConvo));
                break;
        }
    }

    IEnumerator convo1Events (int scriptedConvo) //called from convochecker. These are where 'events' throughout conversations like people turning around or walking should be called.
    {
        switch (scriptedConvo)
        {
            case 0:
                ongoingEvent = true;
                Debug.Log("press l");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.L));
                ongoingEvent = false;
                break;
            case 1:
                ongoingEvent = true;
                Debug.Log("press u");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.U));
                ongoingEvent = false;
                break;
            case 2:
                break;
            case 3:
                ongoingEvent = true;
                Debug.Log("press c");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.C));
                ongoingEvent = false;
                break;
        }
    }

}
