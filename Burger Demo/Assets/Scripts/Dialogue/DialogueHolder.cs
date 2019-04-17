using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHolder : MonoBehaviour {

    public GameObject MainCamera;
    public NarrativeManager nm;
    public NarrativeScript1 ns1; //remember to update this every time a new narrative script is created.
    public GameObject voices;
    public AudioSource gcAS;

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
    public bool ongoingEvent; //true when a dialogue event is ongoingconvoChecker
    public int choiceHover; // 1 for left choice 2 for right choice
    public int choiceSelected; //final choice
    public bool choiceMade; //
    public bool autoAdvance; //when true automatically advances dialogue. Set from outside, if at all

    private int i = 0; // this is for iterating in the interactable

    public void PseudoStart() //sets certain array lengths equal to scripted or interactable
    {
        MainCamera = GameObject.FindWithTag("MainCamera");
        nm = MainCamera.GetComponent<NarrativeManager>();
        gcAS = voices.GetComponent<AudioSource>();
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
                    //Debug.Log("DH Event 1: Calling ns1.convoChecker with scripted conversation  " + scriptedConversation);
                }
                break;

            case 2:
                if (ns1 != null)
                {
                    ns1.convoChecker(scriptedConversation, scriptedConvo[scriptedConversation]); 
                }
                break;

            case 3:
                if (ns1 != null)
                {
                    ns1.convoChecker(scriptedConversation, scriptedConvo[scriptedConversation]);
                }
                break;
            case 4:
                if (ns1 != null)
                {
                    ns1.convoChecker(scriptedConversation, scriptedConvo[scriptedConversation]);
                }
                break;
        }
    }

    public IEnumerator GenericFirstConvo(int scriptedConversation, bool inCombat) //whenever a conversation is going to start, pass it to this, indicating the number from the dialog array the conversation is placed into, as well as whether or not it takes place during combat.
    {
        
        if (scriptedConvoStart[scriptedConversation] == false)
        {
            scriptedConvoStart[scriptedConversation] = true;
            if (/*inCombat == false*/ true)
            {
                StartCoroutine(nm.dialogueBox(true));
            }
            sizeOfList = Scripted[scriptedConversation].DialogItems.Count;
        }
        nm.setTalksprite(Scripted[scriptedConversation], scriptedConvo[scriptedConversation]);
        gcAS.clip = Scripted[scriptedConversation].DialogItems[scriptedConvo[scriptedConversation]].PlayBackSoundFile;
        gcAS.Play();
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(nm.AnimateText(Scripted[scriptedConversation], scriptedConvo[scriptedConversation]));
        specifyScript(scriptedConversation); //here is where we put a switch(?) statement calling other functions when appropriate
        Debug.Log("DH: Calling specify script at stage " + scriptedConversation + " with dialogue text '" + Scripted[scriptedConversation].DialogItems[scriptedConvo[scriptedConversation]].DialogueText + "'");
        scriptedConvo[scriptedConversation]++;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) && nm.canAdvance == true && ongoingEvent == false || autoAdvance == true && nm.canAdvance == true);
        autoAdvance = false;
        //Debug.Log("scripted convo = " + scriptedConvo[scriptedConversation] + "  &   Size of List = " + sizeOfList);
        if (scriptedConvo[scriptedConversation] >= sizeOfList)
        {
            StartCoroutine(nm.dialogueEnd());
            //Debug.Log("its here in dialogue holder");
            scriptedConvoDone[scriptedConversation] = true;
        }
        else
        {
            StartCoroutine(GenericFirstConvo(scriptedConversation, inCombat));
        }
    }

    /*public IEnumerator GenericInteractable(int interactableIdentity) //same as conversation except for interactable objects. These probably could be one function, but it was a little more readable to split them.
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
    }*/

    public IEnumerator GenericInteractableNew(Dialogue dia, GameObject theObject, bool isNPC) //Trying a new spin on the generic interactable script, this time not requiring you to link indexes. In this case, 0 in the array should be left empty of any sort of dialogue and will be a default index, but won't read from there. Instead, only dialogues that want to be checked for "has been used" need be added, and are found dynamically.
    {
        int interactableIdentity = 0;
        foreach (Dialogue dialogue in Interactable)
        {
            if (dialogue == dia)
                interactableIdentity = System.Array.IndexOf(Interactable, dialogue);
            else
                interactableIdentity = 0;
        }

        if (interactConvoStart[interactableIdentity] == false)
        {
            interactConvoStart[interactableIdentity] = true;
            if (!isNPC)
            {
                StartCoroutine(nm.dialogueBox(false));
            }
            else
            {
                StartCoroutine(nm.dialogueBox(true));                
            }
            sizeOfListInteractable = dia.DialogItems.Count;
        }
        if (isNPC)
        {
            float dir = nm.player.transform.position.x - theObject.transform.position.x;
            if (dir > 0) // If the player is to the right of the NPC.
            {
                theObject.GetComponent<SpriteRenderer>().flipX = false;
            }
            else // If the player is directly on top of or to the left of the NPC.
            {
                theObject.GetComponent<SpriteRenderer>().flipX = true;
            }

            nm.imageTS.sprite = dia.DialogItems[interactConvo[interactableIdentity]].CharacterPic;
            nm.nameTS.text = dia.DialogItems[interactConvo[interactableIdentity]].CharacterName;
        }
        //Debug.Log(i);
        if(theObject.GetComponent<Animator>())
            theObject.GetComponent<Animator>().SetInteger("AnimInt", i);
        i++;
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(nm.AnimateText(dia, interactConvo[interactableIdentity]));
        interactConvo[interactableIdentity]++;
        //here is where we put a switch(?) statement calling other functions when appropriate
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) && nm.canAdvance == true && !nm.choiceFilling);
        //Debug.Log("interact convo = " + interactConvo[interactableIdentity] + "  &   Size of List = " + sizeOfListInteractable);
        if (interactConvo[interactableIdentity] >= sizeOfListInteractable)
        {
            StartCoroutine(nm.dialogueEnd());
            interactConvoDone[interactableIdentity] = true;
            interactConvo[interactableIdentity] = 0;
            interactConvoStart[interactableIdentity] = false;
            i = 0;

            if (theObject.GetComponent<Animator>())
            {
                theObject.GetComponent<Animator>().SetInteger("AnimInt", i); yield return new WaitForEndOfFrame();
                theObject.GetComponent<Animator>().SetInteger("AnimInt", -1);
            }
        }
        else
        {
            StartCoroutine(GenericInteractableNew(dia, theObject, isNPC));
        }
    }

    public void CancelDialogue(bool endNow)
    {
        StopAllCoroutines();

        for (int i = 0; i < interactConvo.Length; i++)
        {
            interactConvo[i] = 0;
            interactConvoStart[i] = false;
        }

        for (int i = 0; i < scriptedConvo.Length; i++)
        {
            scriptedConvo[i] = 0;
            scriptedConvoStart[i] = false;
        }

        if (endNow)
        {
            nm.StartCoroutine(nm.dialogueEnd());
            //Debug.Log("its here in CancelDialogue");
        }


    }
    

}
