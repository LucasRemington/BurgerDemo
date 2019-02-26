using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeManager : MonoBehaviour {

    public OverworldMovement owm;
    public BattleTransitions bt;
    public NarrativeScript1 ns1;
    public GameObject player;
    public GameObject BurgerSpawner;
    public BurgerComponentInstantiator bci;

    public int ev; //integer that determines what can happen. Each 'event' to occur should increase it by one.
    public bool reading; //true when text is onscreen

    public int area; //used to identify area
    public int room; //used to identify room

    //used for dialogue box
    public Text textTS; // text box text
    public Text nameTS; //name of talksprite
    public GameObject textBox; //textbox gameobject
    public RectTransform tbTrans; //textbox transform
    public Text[] choiceText = new Text[2]; //choice text - this limits choices to binary FYI
    public DBAnimEvents DBAE; //script that calls animation events
    public Image db; //textbox component
    public Image imageTS; //talksprite image component
    public Animator dbAnim;//animator for dialog box
    public GameObject canvas;

    //used for combat dialogue box
    public GameObject combatUI;
    public Text textTSCombat; // text box text
    public Text nameTSCombat; //name of talksprite
    public Image imageTSCombat; //talksprite image component

    public bool choice = false; // true when making a choice
    public bool combatText; //true when combat text can be activated
    public bool dbStartStop; //triggered briefly to start and stop text
    public bool dbChoiceSS; //start stop but choice specific
    public bool canAdvance = true; //true when you can advance text
    public bool autoAdvance; //autoadvances dialogue

    private bool spaceHeld;
    private float textTime = 0.01f;

    public void PseudoStart ()
    {
        ns1 = GetComponent<NarrativeScript1>();
        player = GameObject.FindWithTag("Player");
        player = player.transform.Find("OverworldPlayer").gameObject;
        owm = player.GetComponent<OverworldMovement>();
    }

    public void Start () //note that the Narrative Script and each sub-script are intended to be on the same object. Other scripts currently reference MainCamera, so use that.
    {
        CheckEvent();
        ns1.blackScreen.gameObject.SetActive(true);
        StartCoroutine(combatUIOn());
        ClearText();
    }
    
    IEnumerator eventZero() //'event zero' occurs right at the beginning of the game. Might also be phased out.
    {
        //yield return new WaitUntil(() => room == 1);
        yield return new WaitForSeconds(0.5f);
        ev++;
        CheckEvent();
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

    public IEnumerator dialogueBox (bool talkSprite) //other scripts pass to this functon - enables appropriate dialog box
    {
        owm.canMove = false;
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
        Debug.Log("dialogueend");
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
        owm.canMove = true;
    }

    public IEnumerator combatUIOn () //activates UI for combat. Might be moved to a different script.
    {
        yield return new WaitUntil(() => bt.battling == true);
        combatUI.SetActive(true);
        yield return new WaitUntil(() => dbStartStop == true);
        dbStartStop = false;
        textTSCombat.enabled = true;
        nameTSCombat.enabled = true;
        imageTSCombat.enabled = true;
        combatText = true;
    }

    public void setTalksprite (Dialogue dia, int convoNumber) //sets the appropriate talksprite of the image when called.
    {
        string name = dia.DialogItems[convoNumber].CharacterName;
        Sprite sprite = dia.DialogItems[convoNumber].CharacterPic;
        if (bt.battling == false)
        {
            nameTS.text = name;
            imageTS.sprite = sprite;
        }
        else
        {
            nameTSCombat.text = name;
            imageTSCombat.sprite = sprite;
        }
    }

    public IEnumerator AnimateText(Dialogue dia, int convoNumber) //makes text scroll retro-style instead of coming in all at once.
    {

        string strComplete = dia.DialogItems[convoNumber].DialogueText;
        if (dia.DialogItems[convoNumber].ChoiceText.Length > 0)
        {
            StartCoroutine(choiceAnimator(dia, convoNumber, 0));
            StartCoroutine(choiceAnimator(dia, convoNumber, 1));
        }
        int i = 0;
        canAdvance = false;
        bool isComplete = false;




        if (bt.battling == false && !isComplete)
        {
            textTS.text = "";
            while (i < strComplete.Length)
            {
                textTS.text += strComplete[i++];

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    textTS.text = strComplete;
                    i = strComplete.Length;
                }
                yield return new WaitForSeconds(0.01f);

            }
        }
        else if (bt.battling && !isComplete)
        {
            //Debug.Log(strComplete);
            textTSCombat.text = "";
            while (i < strComplete.Length)
            {
                textTSCombat.text += strComplete[i++];

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    textTSCombat.text = strComplete;
                    i = strComplete.Length;
                }
                yield return new WaitForSeconds(0.01f);
            }
        }


        /*while (spaceHeld && !Input.GetKey(KeyCode.Space))
        {
            spaceHeld = false;
            yield return null;
        }*/


        if (i == strComplete.Length)
        {
            canAdvance = true;
            textTime = 0.01f;
            spaceHeld = false;
        }
        
    }

    public IEnumerator choiceAnimator(Dialogue dia, int convoNumber, int text) //called from AnimateText to animate text of choices
    {
        yield return new WaitUntil(() => dbChoiceSS == true);
        string cho = dia.DialogItems[convoNumber].ChoiceText[text];
        int i = 0;
        while (i < cho.Length)
        {
            choiceText[text].text += cho[i++];
            yield return new WaitForSeconds(0.01f);
        }
    }

    public IEnumerator isQuestion (Dialogue dia, int convoNumber) //called whenever a choice needs to be made -- messy and hardcoded FYI
    {
        choiceText[0].enabled = true;
        choiceText[1].enabled = true;
        choice = true;
        dbAnim.SetBool("Choice", true);
        choiceText[0].transform.SetParent(canvas.transform);
        choiceText[1].transform.SetParent(canvas.transform);
        imageTS.transform.SetParent(canvas.transform);
        textTS.transform.SetParent(canvas.transform);
        tbTrans.localPosition = new Vector3(0, 87, 0);
        tbTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 320);
        yield return new WaitUntil(() => dbChoiceSS == true);
        yield return new WaitForSeconds(0.05f);
        dbChoiceSS = false;
        AnimateText(dia, convoNumber);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || autoAdvance == true);
        autoAdvance = false;
        choice = false;
        choiceText[0].transform.SetParent(db.transform);
        choiceText[1].transform.SetParent(db.transform);
        choiceText[0].enabled = false;
        choiceText[1].enabled = false;
        dbAnim.SetBool("Choice", false);
        yield return new WaitUntil(() => dbChoiceSS == true);
        yield return new WaitForSeconds(0.05f);
        dbChoiceSS = false;
        tbTrans.localPosition = new Vector3(0, 167, 0);
        tbTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 160);
        imageTS.transform.SetParent(db.transform);
        textTS.transform.SetParent(db.transform);  

    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals(object other)
    {
        return base.Equals(other);
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
