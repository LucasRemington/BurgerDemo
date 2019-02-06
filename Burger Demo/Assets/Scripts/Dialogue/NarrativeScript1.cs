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
    public int numberOfScripted; //used to control number of arrays
    public int numberOfInteract; //used to control number of arrays

    private void Start()
    {
        nm = GetComponent<NarrativeManager>();
        scriptedConvo = new int[numberOfScripted];
        scriptedConvoStart = new bool [numberOfScripted];
        scriptedConvoDone = new bool [numberOfScripted];
        interactConvo = new int[numberOfInteract];
        interactConvoStart = new bool[numberOfInteract];
        interactConvoDone = new bool[numberOfInteract];
        StartCoroutine(interactableTester());
    }

    public IEnumerator eventOne()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.I));

        StartCoroutine(GenericFirstConvo(0, false));
        StartCoroutine(masterCombatStarter());
        yield return new WaitUntil(() => nm.room == 1);
        nm.ev++;
        nm.CheckEvent();
    }

    IEnumerator GenericFirstConvo(int scriptedConversation, bool inCombat)
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
        scriptedConvo[scriptedConversation]++;
        //here is where we put a switch(?) statement calling other functions when appropriate
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) && nm.canAdvance == true);
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
    }

    IEnumerator masterCombatStarter ()
    {
        yield return new WaitUntil(() => nm.combat == true && nm.combatText == true);
        Debug.Log("master start");
        StartCoroutine(GenericFirstConvo(2, true));
    }

    IEnumerator interactableTester ()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.O));
        StartCoroutine(GenericInteractable(0));
    }

}
