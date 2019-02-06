using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeManager : MonoBehaviour {

    public NarrativeScript1 ns1;

    public int ev; //integer that determines what can happen. Each 'event' to occur should increase it by one.
    public bool reading; //true when text is onscreen

    public int area; //used to identify area
    public int room; //used to identify room

    //used for dialogue box
    public Text textTS; // text box text
    public Text nameTS; //name of talksprite
    public GameObject textBox; //textbox gameobject
    public DBAnimEvents DBAE; //script that calls animation events
    public Image db; //textbox component
    public Image imageTS; //talksprite image component
    public Animator dbAnim;//animator for dialog box

    //used for combat dialogue box
    public GameObject combatUI;
    public Text textTSCombat; // text box text
    public Text nameTSCombat; //name of talksprite
    public Image imageTSCombat; //talksprite image component

    public bool combat = false; // true when in combat
    public bool combatText; //true when combat text can be activated
    public bool dbStartStop; //triggered briefly to start and stop text
    public bool canAdvance = true; //true when you can advance text
    public bool textActive; //wait for text

    void Start () //note that the Narrative Script and each sub-script are intended to be on the same object. Other scripts currently reference MainCamera, so use that.
    {
        ns1 = GetComponent<NarrativeScript1>();
        CheckEvent();
        StartCoroutine(waitForText());
        StartCoroutine(combatUIOn());
        StartCoroutine(tempCombatCaller());
        ClearText();
    }

    IEnumerator tempCombatCaller() //temp script, won't be in final
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.P));
        combat = true;
    }

    void ClearText () //clears all text and dialog boxes
    {
        imageTS.enabled = false;
        textTS.enabled = false;
        nameTS.enabled = false;
        db.enabled = false;
        textTSCombat.enabled = false;
        nameTSCombat.enabled = false;
        imageTSCombat.enabled = false;
    }

    public void CheckEvent () //master function used to begin and end the master functions of other scripts. 
    {
        switch (ev)
        {
            case 0:
                StartCoroutine(eventZero());
                break;
            case 1:
                StartCoroutine(ns1.eventOne());
                break;
        }
    }

    IEnumerator waitForText ()
    {
        yield return new WaitUntil(() => textActive == true);

    }//this currently doesn't do anything. It'll probably be phased out

	IEnumerator eventZero () //'event zero' occurs right at the beginning of the game. Might also be phased out.
    {
        //yield return new WaitUntil(() => room == 1);
        yield return new WaitForSeconds(0.5f);
        ev++;
        CheckEvent();
    }

    public IEnumerator dialogueBox (bool talkSprite) //other scripts pas to this functon - enables appropriate dialog box
    {
        db.enabled = true;
        if (talkSprite == true)
        {
            dbAnim.SetBool("Talksprite", true);
        } else
        {
            dbAnim.SetBool("Talksprite", false);
        }
        dbAnim.SetTrigger("Popup");
        yield return new WaitUntil(() => dbStartStop == true);
        dbStartStop = false;
        textTS.enabled = true;
        if (talkSprite == true)
        {
            imageTS.enabled = true;
            nameTS.enabled = true;
        }
    }

    public IEnumerator dialogueEnd () //called when dialog box is no longer needed
    {
        textTS.enabled = false;
        if (imageTS.isActiveAndEnabled == true)
        {
            imageTS.enabled = false;
            nameTS.enabled = false;
        }
        dbAnim.SetTrigger("Popdown");
        yield return new WaitUntil(() => dbStartStop == true);
        dbStartStop = false;
        db.enabled = false;
    }

    public IEnumerator combatUIOn () //activates UI for combat. Might be moved to a different script.
    {
        yield return new WaitUntil(() => combat == true);
        combatUI.SetActive(true);
        yield return new WaitUntil(() => dbStartStop == true);
        dbStartStop = false;
        textTSCombat.enabled = true;
        nameTSCombat.enabled = true;
        imageTSCombat.enabled = true;
        combatText = true;
    }

    public void setTalksprite (Dialogue dia, int convoNumber)
    {
        string name = dia.DialogItems[convoNumber].CharacterName;
        Sprite sprite = dia.DialogItems[convoNumber].CharacterPic;
        if (combat == false)
        {
            nameTS.text = name;
            imageTS.sprite = sprite;
        }
        else
        {
            nameTSCombat.text = name;
            imageTSCombat.sprite = sprite;
        }
    }//sets the appropriate talksprite of the image when called.

    public IEnumerator AnimateText(Dialogue dia, int convoNumber)
    {
        string strComplete = dia.DialogItems[convoNumber].DialogueText;
        int i = 0;
        canAdvance = false;
        if (combat == false)
        {
            textTS.text = "";
            while (i < strComplete.Length)
            {
                textTS.text += strComplete[i++];
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            textTSCombat.text = "";
            while (i < strComplete.Length)
            {
                textTSCombat.text += strComplete[i++];
                yield return new WaitForSeconds(0.01f);
            }
        }
        if (i == strComplete.Length)
        {
            canAdvance = true;
        }
    }//makes text scroll retro-style instead of coming in all at once.
}
