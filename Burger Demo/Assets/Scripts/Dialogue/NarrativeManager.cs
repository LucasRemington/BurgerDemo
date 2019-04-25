using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NarrativeManager : MonoBehaviour {

    [Header("Misc")]
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

    [Header("Dialogue Box")]
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
    private bool dbActive = false; // Set at the beginning of dialogueBox and at the end of dialogueEnd; if this is active, we don't actually go through dialogueBox.

    [Header("Text Positions")]
    public Vector2 textPosYesTalksp;
    public Vector2 textPosNoTalksp;
    public float widthYesTalksp = 590;
    public float widthNoTalksp = 805;

    [Header("Combat Dia Box")]
    public GameObject combatUI;
    public Text textTSCombat; // text box text
    public Text nameTSCombat; //name of talksprite
    public Image imageTSCombat; //talksprite image component

    [Header("Choices and Text Bools")]
    public bool choice = false; // true when making a choice
    public bool combatText; //true when combat text can be activated
    public bool dbStartStop; //triggered briefly to start and stop text
    public bool dbChoiceSS; //start stop but choice specific
    public bool canAdvance = true; //true when you can advance text
    public bool autoAdvance; //autoadvances dialogue

    public int choiceHover; // 1 for left choice 2 for right choice
    public int choiceSelected; //final choice
    public bool choiceMade; //
    private bool isChoice = false; // If the choiceanimator is currently in a choice, helps prevents the choice boxes from moving out of place by being called too quickly
    private bool canSkip = true; // if you can currently skip through text
    [HideInInspector] public bool choiceFilling = false; // Whether choice text is still filling out: you can't skip this, so we wait until it fills out. Also helps with back to back choices.

    [HideInInspector] public bool gameStarted; // If the player has watched the cutscene at the very beginning of the game already, don't play it whenever they enter the freezer.

    [HideInInspector] public bool BattleDone = false; // active if you have finished a battle, for narrative purposes.
    [HideInInspector] public bool BattleWon = false; // active if you have killed the enemy, for narrative purposes.

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
        //StartCoroutine(combatUIOn());
        ClearText();
       
    }


    IEnumerator eventZero() //'event zero' occurs right at the beginning of the game. Might also be phased out.
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name != "MainMenu");
        if (!gameStarted)
        {
            ns1.blackScreen = GameObject.FindGameObjectWithTag("BlackScreen").GetComponent<Image>();
            //yield return new WaitUntil(() => ns1.blackScreen.gameObject != null);
            ns1.blackScreen.gameObject.SetActive(true);
            //yield return new WaitUntil(() => room == 1);
            yield return new WaitForSeconds(0.5f);
            ev++;
        }       
        
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
        Debug.Log("Clear text.");
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
            case 2:
                StartCoroutine(ns1.eventTwo());
                break;
            case 3:
                StartCoroutine(ns1.eventThree());
                break;
            case 4:
                StartCoroutine(ns1.eventFour());
                break;
            case 5:
                StartCoroutine(ns1.eventFive());
                break;
            case 6:
                StartCoroutine(ns1.eventSix());
                break;
        }
    }

    public IEnumerator dialogueBox (bool talkSprite) //other scripts pass to this functon - enables appropriate dialog box
    {
        if (!dbActive)
        {
            dbActive = true;

            Debug.Log("dialogueBox start");
            owm.canMove = false;
            db.enabled = true;
            if (talkSprite == true)
            {
                dbAnim.SetBool("Talksprite", true);
                textTS.rectTransform.localPosition = textPosYesTalksp;
                textTS.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, widthYesTalksp);
                //textTS.alignment = TextAnchor.MiddleLeft;

            }
            else
            {
                dbAnim.SetBool("Talksprite", false);
                textTS.rectTransform.localPosition = textPosNoTalksp;
                textTS.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, widthNoTalksp);
                //textTS.alignment = TextAnchor.MiddleCenter;
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
            Debug.Log("dialogueBox end");
        }
        
    }

    public IEnumerator dialogueEnd () //called when dialog box is no longer needed
    {
        Debug.Log("dialogueend start");
        dbActive = false;
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
        Debug.Log("dialogueend end");
    }

    public IEnumerator combatUIOn () //activates UI for combat. Might be moved to a different script.
    {
        yield return new WaitUntil(() => bt.battling == true);
        combatUI.SetActive(true);
        /*yield return new WaitUntil(() => dbStartStop == true);
        dbStartStop = false;
        textTSCombat.enabled = true;
        nameTSCombat.enabled = true;
        imageTSCombat.enabled = true;
        combatText = true;*/
    }

    public void setTalksprite (Dialogue dia, int convoNumber) //sets the appropriate talksprite of the image when called.
    {
        string name = dia.DialogItems[convoNumber].CharacterName;
        Sprite sprite = dia.DialogItems[convoNumber].CharacterPic;
        if (/*bt.battling == false*/ true) // We're getting rid of our old combat UI, but just in case...
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
            choiceFilling = true;
            StopCoroutine(choiceAnimator(dia, convoNumber, 0));
            StopCoroutine(choiceAnimator(dia, convoNumber, 1));
            StartCoroutine(choiceAnimator(dia, convoNumber, 0));
            StartCoroutine(choiceAnimator(dia, convoNumber, 1));
            StopCoroutine(isQuestion(dia, convoNumber));
            StartCoroutine(isQuestion(dia, convoNumber));
            canSkip = false;
        }
        else
            canSkip = true;
        int i = 0;
        canAdvance = false;
        bool isComplete = false;


        if (/*bt.battling == false &&*/ !isComplete)
        {
            textTS.text = "";
            while (i < strComplete.Length)
            {
                textTS.text += strComplete[i++];

                if (Input.GetKeyDown(KeyCode.Space) && canSkip)
                {
                    textTS.text = strComplete;
                    i = strComplete.Length;
                }
                if (dia.DialogItems[convoNumber].TextPlayBackSpeed == 0)
                    yield return new WaitForSeconds(0.01f);
                else
                    yield return new WaitForSeconds(dia.DialogItems[convoNumber].TextPlayBackSpeed);
            }
        }
        /*else if (bt.battling && !isComplete) // Again, this is combatUI specific, and as we're unifying combat and out of combat UI...leaving the skellington, just in case.
        {
            //Debug.Log(strComplete);
            textTSCombat.text = "";
            while (i < strComplete.Length)
            {
                textTSCombat.text += strComplete[i++];

                if (Input.GetKeyDown(KeyCode.Space) && canSkip)
                {
                    textTSCombat.text = strComplete;
                    i = strComplete.Length;
                }
                if (dia.DialogItems[convoNumber].TextPlayBackSpeed == 0)
                    yield return new WaitForSeconds(0.01f);
                else
                    yield return new WaitForSeconds(dia.DialogItems[convoNumber].TextPlayBackSpeed);
            }
        }*/


        /*while (spaceHeld && !Input.GetKey(KeyCode.Space))
        {
            spaceHeld = false;
            yield return null;
        }*/

        if (i == strComplete.Length && choiceFilling)
            yield return new WaitUntil(() => !choiceFilling);

        if (i == strComplete.Length)
        {
            canAdvance = true;
            textTime = 0.01f;
            spaceHeld = false;
        }


        
    }

    public IEnumerator choiceAnimator(Dialogue dia, int convoNumber, int text) //called from AnimateText to animate text of choices
    {
        choiceText[text].text = "";
        //Debug.Log("choiceAnimator called. Convonumber " + convoNumber + ", choice text = " + dia.DialogItems[convoNumber].ChoiceText[text]);
        yield return new WaitUntil(() => dbChoiceSS == true);
       // yield return new WaitUntil(() => dbChoiceSS == true);
        string cho = dia.DialogItems[convoNumber].ChoiceText[text];
        int i = 0;

        while (i < cho.Length)
        {
            choiceFilling = true;
            choiceText[text].text += cho[i++];
            yield return new WaitForSeconds(0.01f);

            if (!isChoice)
            {
                choiceText[text].text = "";
                i = cho.Length;
            }
        }
        choiceFilling = false;
    }

    public IEnumerator isQuestion (Dialogue dia, int convoNumber) //called whenever a choice needs to be made -- messy and hardcoded FYI
    {
        StopCoroutine(choiceChecker());
        StartCoroutine(choiceChecker());

        isChoice = true;

        choiceText[0].enabled = true;
        choiceText[1].enabled = true;

        choice = true;
        dbChoiceSS = false;

        dbAnim.SetBool("Choice", true);
        choiceText[0].transform.SetParent(db.transform);
        choiceText[1].transform.SetParent(db.transform);
        imageTS.transform.SetParent(canvas.transform);
        textTS.transform.SetParent(canvas.transform);

        if (isChoice)
        {
            tbTrans.localPosition = new Vector3(0, 87, 0);
            tbTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 320);
        }

        yield return new WaitUntil(() => dbChoiceSS == true);
        yield return new WaitForSeconds(0.01f);

        dbChoiceSS = false;
        AnimateText(dia, convoNumber);

        //yield return new WaitUntil(() => textTS.text == dia.DialogItems[convoNumber].DialogueText);
        yield return new WaitUntil(() => canAdvance && !choiceFilling);
        yield return new WaitUntil(() => (Input.GetKeyDown(KeyCode.Space) || autoAdvance == true) && !choiceFilling);
        choiceText[0].text = "";
        choiceText[1].text = "";
        isChoice = false;
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

        if (!isChoice)
        {
            tbTrans.localPosition = new Vector3(0, 167, 0);
            tbTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 160);
        }

        imageTS.transform.SetParent(db.transform);
        textTS.transform.SetParent(db.transform);  

    }

    public IEnumerator choiceChecker()
    {
        yield return new WaitUntil(() => (choiceText[0].enabled == true));
        choiceText[0].color = Color.red;
        choiceHover = 1;
        StartCoroutine(arrowColors());
        yield return new WaitUntil(() => (Input.GetKeyDown(KeyCode.Space) || autoAdvance == true) && !choiceFilling);
        choiceSelected = choiceHover;
        Debug.Log("Choice selected: choice " + choiceHover);
        choiceMade = true;
        yield return new WaitForSeconds(0.05f);
        choiceSelected = 0;
        choiceMade = false;
    }

    public IEnumerator arrowColors()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.RightArrow) || choiceMade == true);
        choiceText[0].color = Color.white;
        choiceText[1].color = Color.red;
        choiceHover = 2;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.LeftArrow) || choiceMade == true);
        choiceText[1].color = Color.white;
        choiceText[0].color = Color.red;
        choiceHover = 1;
        if (choiceMade == false)
        {
            StartCoroutine(arrowColors());
        }
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
