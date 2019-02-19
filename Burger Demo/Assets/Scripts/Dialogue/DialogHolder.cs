using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogHolder : MonoBehaviour {

    public GameObject MainCamera;
    public NarrativeManager nm;
    public NarrativeScript1 ns1; //remember to update this every time a new narrative script is created.

    public Dialogue[] Scripted; //each 'dialogue' is a different conversation, each element is a different dialog box. 1 indicates that this takes place during the first event only.
    public int[] scriptedConvo; //used to keep track of the point in conversation one would be at.
    public bool[] scriptedConvoStart; //true when the convo has started
    public bool[] scriptedConvoDone; //used to track which conversations have finished

    public Dialogue[] Interactable; //each 'interactable' is a different interactable 
    public int[] interactConvo; //used to keep track of the point in conversation one would be at.
    public bool[] interactConvoStart; //true when the convo has started
    public bool[] interactConvoDone; //used to track which conversations have finished

    public int sizeOfList; //set equal to the number of items in a dialogue element
    public int sizeOfListInteractable;
    public bool ongoingEvent; //true when a dialogue event is ongoing
    public int choiceHover; // 1 for left choice 2 for right choice
    public int choiceSelected; //final choice
    public bool choiceMade; //
    public bool autoAdvance; //when true automatically advances dialogue. Set from outside, if at all

    public void PseudoStart() //sets certain array lengths equal to scripted or interactable
    {
        MainCamera = GameObject.FindWithTag("MainCamera");
        nm = MainCamera.GetComponent<NarrativeManager>();
        setScripts();
        scriptedConvo = new int[Scripted.Length];
        scriptedConvoStart = new bool[Scripted.Length];
        scriptedConvoDone = new bool[Scripted.Length];
        interactConvo = new int[Interactable.Length];
        interactConvoStart = new bool[Interactable.Length];
        interactConvoDone = new bool[Interactable.Length];
    }

    void setScripts () //remember to update this every time a new narrative script is created.
    {
        if (GetComponent<NarrativeScript1>() != null)
        {
            ns1 = GetComponent<NarrativeScript1>();
        }
    }

    void specifyScript (int scriptedConversation) //remember to update this every time a new narrative script is created.
    {
        switch (nm.ev)
        {
            case 0:
                break;

            case 1:
                if (ns1 != null)
                {
                    ns1.convoChecker(scriptedConversation, scriptedConvo[scriptedConversation]);
                }
                break;

            case 2:
                break;

            case 3:

                break;
        }
    }

    public IEnumerator GenericFirstConvo(int scriptedConversation, bool inCombat) //whenever a conversation is going to start, pass it to this, indicating the number from the dialog array the conversation is placed into, as well as whether or not it takes place during combat.
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
        specifyScript(scriptedConversation); //here is where we put a switch(?) statement calling other functions when appropriate
        scriptedConvo[scriptedConversation]++;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) && nm.canAdvance == true && ongoingEvent == false || autoAdvance == true && nm.canAdvance == true && ongoingEvent == false);
        autoAdvance = false;
        //Debug.Log("scripted convo = " + scriptedConvo[scriptedConversation] + "  &   Size of List = " + sizeOfList);
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

    public IEnumerator GenericInteractable(int interactableIdentity) //same as conversation except for interactable objects. These probably could be one function, but it was a little more readable to split them.
    {
        if (interactConvoStart[interactableIdentity] == false)
        {
            interactConvoStart[interactableIdentity] = true;
            StartCoroutine(nm.dialogueBox(false));
            sizeOfListInteractable = Interactable[interactableIdentity].DialogItems.Count;
        }
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(nm.AnimateText(Interactable[interactableIdentity], interactConvo[interactableIdentity]));
        interactConvo[interactableIdentity]++;
        //here is where we put a switch(?) statement calling other functions when appropriate
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) && nm.canAdvance == true);
        Debug.Log("interact convo = " + interactConvo[interactableIdentity] + "  &   Size of List = " + sizeOfListInteractable);
        if (interactConvo[interactableIdentity] >= sizeOfListInteractable)
        {
            StartCoroutine(nm.dialogueEnd());
            interactConvoDone[interactableIdentity] = true;
            interactConvo[interactableIdentity] = 0;
            interactConvoStart[interactableIdentity] = false;
        }
        else
        {
            StartCoroutine(GenericInteractable(interactableIdentity));
        }
    }

    public IEnumerator choiceChecker()
    {
        yield return new WaitUntil(() => (nm.choiceText[0].enabled == true));
        nm.choiceText[0].color = Color.red;
        choiceHover = 1;
        StartCoroutine(arrowColors());
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || autoAdvance == true);
        choiceSelected = choiceHover;
        choiceMade = true;
        yield return new WaitForSeconds(0.05f);
        choiceSelected = 0;
        choiceMade = false;
    }

    public IEnumerator arrowColors ()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.RightArrow) || choiceMade == true);
        nm.choiceText[0].color = Color.white;
        nm.choiceText[1].color = Color.red;
        choiceHover = 2;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.LeftArrow) || choiceMade == true);
        nm.choiceText[1].color = Color.white;
        nm.choiceText[0].color = Color.red;
        choiceHover = 1;
        if (choiceMade == false)
        {
            StartCoroutine(arrowColors());
        }
    }

}
