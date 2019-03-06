using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]

public class NarrativeScript1 : MonoBehaviour {

    //critical game objects
    public GameObject MainCamera;
    public NarrativeManager nm;
    public DialogueHolder dh;
    public ScriptedMovement sm;
    public GameObject tutEnemy;
    public TutorialEnemy te;
    public GameObject TV;
    private int loops;
    public Animator combatUIAnim;

    public GameObject playerHolder;
    public GameObject player;
    public SpriteRenderer playerSR;
    public Animator playerAnim;
    public GameObject dennis;
    public SpriteRenderer dennisSR;
    public Animator dennisAnim;
    public GameObject chair;
    public GameObject holoMaster;
    public SpriteRenderer holomSR;
    public Animator holomAnim;
    public Animator startAnim;
    public Image blackScreen;
    public bool waitForScript;
    public bool convoDone;
    public GameObject[] walls;
    public TransitionManager transitMan;

    public Dialogue dennis1; // specific dialogue modified by player choice
    //public string[] choiceFor_dennis1; //string holding the dialogue for choice changes. Kept in for template only
    public bool animationFlag; //set from animation events to get timing right
    public bool timerFlag; //set by timer to ger timing right

    public GameObject masterHologram;

    public void PseudoStart ()
    {
        playerHolder = GameObject.FindWithTag("Player");
        player = playerHolder.transform.Find("OverworldPlayer").gameObject;
        playerAnim = player.GetComponent<Animator>();
        playerSR = player.GetComponent<SpriteRenderer>();
        sm = player.GetComponent<ScriptedMovement>();
        MainCamera = GameObject.FindWithTag("MainCamera");
        nm = MainCamera.GetComponent<NarrativeManager>();
        dh = GetComponent<DialogueHolder>();
        if (transitMan == null) {
            transitMan = GameObject.FindGameObjectWithTag("Transition Manager").GetComponent<TransitionManager>();
        }
        if (nm.room == 3)
        {
            tutEnemy = GameObject.Find("HoloMaster");
            te = tutEnemy.GetComponent<TutorialEnemy>();
        }
    }

    public void Start()
    {
        StartCoroutine(battleEarly());
    }

    public IEnumerator battleEarly ()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.J));
        Debug.Log("j");
        StartCoroutine(nm.bt.StartBattle(masterHologram));
        yield return new WaitUntil(() => nm.bt.battling == true);
        StartCoroutine(dh.GenericFirstConvo(2, true));
    }

    public IEnumerator eventOne() //first event. Eventually, this will be the 'master function' calling shit in order via coroutines.
    {
        // Black screen on startup. Pressing anything fades it out and spawns in the player.
        if (!nm.gameStarted)
        {
            if (blackScreen == null)
                blackScreen = GameObject.FindGameObjectWithTag("BlackScreen").GetComponent<Image>();
            blackScreen.gameObject.SetActive(true);
            blackScreen.color = new Color(0, 0, 0, 1);
            yield return new WaitUntil(() => Input.anyKey == true);

            StartCoroutine(nm.bci.FadeImageToZeroAlpha(2, blackScreen));
            yield return new WaitForSeconds(4f);


            startAnim.SetTrigger("Start");
        }
        

        // Wait until we walk into Dennis' office for the first time. 0.75f waits until we load in before grabbing Dennis and his components.
        Debug.Log("event1");
        yield return new WaitUntil(() => nm.room == 1); //change this to pull from gameManager + flag set from animation event 
        yield return new WaitForSeconds(0.75f);
        dennis = GameObject.FindWithTag("Dennis");
        dennis = dennis.transform.Find("dennis").gameObject;
        sm = dennis.GetComponent<ScriptedMovement>();
        dennisAnim = dennis.GetComponent<Animator>();
        dennisSR = dennis.GetComponent<SpriteRenderer>();

        // We wait for an animation flag to be set...I think this is shaking your hand? And we get into the first conversation with Dennis!
        yield return new WaitUntil(() => animationFlag == true);
        dennis = GameObject.FindWithTag("Dennis");
        dennis = dennis.transform.Find("dennis").gameObject;
        sm = dennis.GetComponent<ScriptedMovement>();
        dennisAnim = dennis.GetComponent<Animator>();
        dennisSR = dennis.GetComponent<SpriteRenderer>();
        playerAnim.SetTrigger("ResetIdle");
        animationFlag = false;
        StartCoroutine(dh.GenericFirstConvo(0, false));
        nm.owm.canMove = false;

        // Once that convo is done and the player can move, we REALLY DON'T WANT THE PLAYER TO MOVE. PLEASE. STOP MOVING.
        yield return new WaitUntil(() => nm.owm.canMove == true);
        nm.owm.canMove = false;

        // After the conversation is done and we've loaded into the hallway, sliiiiiide the player toward the training room!
        yield return new WaitUntil(() => dh.scriptedConvoDone[0] == true && nm.room == 2);
        yield return new WaitUntil(() => nm.owm.canMove == true);
        nm.owm.canMove = false;
        sm = player.GetComponent<ScriptedMovement>();
        //yield return new WaitForSeconds(0.75f);
        StartCoroutine(sm.MoveTo(player, new Vector3(125f, 0, 0), 3f));

        // Training room now, bitches.
        yield return new WaitUntil(() => nm.room == 3);
        //StopCoroutine(sm.MoveTo(player, new Vector3(225f, 0, 0), 2f));
        //StartCoroutine(sm.MoveTo(player, new Vector3(0f, 0, 0), 0.25f));


        sm = player.GetComponent<ScriptedMovement>();
        
        nm.owm.canMove = false;
        Debug.Log("room 3 ");

        // Chair flips around, and we approach the Master.
        yield return new WaitForSeconds(1.5f);        
        StartCoroutine(sm.MoveTo(player, new Vector3(7.6f, 0, 0), 0.8f));
        yield return new WaitUntil(() => sm.finished == true);
        playerSR.flipX = false;
        playerAnim.SetTrigger("OfficeDennis");
        yield return new WaitUntil(() => animationFlag == true);
        animationFlag = false;
        holoMaster = GameObject.Find("TheMasterHolder");
        holoMaster = holoMaster.transform.Find("HoloMaster").gameObject;
        holomAnim = holoMaster.GetComponent<Animator>();
        holomAnim.SetTrigger("Awake");
        yield return new WaitUntil(() => animationFlag == true);
        animationFlag = false;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(dh.GenericFirstConvo(1, false));
        yield return new WaitUntil(() => dh.scriptedConvoDone[1] == true);
        dh.CancelDialogue(true); 
        yield return new WaitForSeconds(0.5f);
        Debug.Log("combat");
        StartCoroutine(nm.bt.StartBattle(masterHologram));
        yield return new WaitUntil(() => nm.bt.battling == true);
        StartCoroutine(dh.GenericFirstConvo(2, true));
        /*yield return new WaitUntil(() => animationFlag == true); //change this to wait until combat finishes + flag set from animation event   (!nm.bt.battling)
        animationFlag = false;*/
        //StartCoroutine(dh.GenericFirstConvo(2, true));
        yield return new WaitUntil(() => /*animationFlag == true && */nm.bt.battling == false); //change this to wait until combat finishes + flag set from animation event 
        Debug.Log("battle is over");
        holomAnim.SetTrigger("Sleep");
        animationFlag = false;
        yield return new WaitUntil(() => nm.room == 2);

        //StartCoroutine(dh.GenericFirstConvo(9, false));
        
        nm.ev++;
        
        nm.CheckEvent();
    }

    public IEnumerator eventTwo() {
        Debug.Log("event 2");
        dh.CancelDialogue(true);
        nm.combatText = false;
        nm.autoAdvance = false;
        if (nm.room == 2)
        {
            walls = GameObject.FindGameObjectsWithTag("ShudderWall");
            foreach (GameObject wall in walls)
            {
                wall.GetComponent<AudioSource>().mute = true;
                wall.GetComponent<Animator>().SetTrigger("Slam");
                wall.GetComponent<BoxCollider2D>().isTrigger = false;           // makes sure all the walls are closed and does so quietly
            }
            yield return new WaitForSeconds(3);
            foreach (GameObject wall in walls)
            {
                wall.GetComponent<AudioSource>().mute = false;
            }
        }
        yield return new WaitUntil(()=> nm.room == 1);
        Debug.Log("In Dennis' room");
        transitMan.readyForFade = false;
        yield return new WaitForSeconds(1);
        player.GetComponent<SpriteRenderer>().flipX = true;        
        dennis = GameObject.FindGameObjectWithTag("Dennis");
        dennis = dennis.transform.Find("dennis").gameObject;
        dennisAnim = dennis.GetComponent<Animator>();
        dennisAnim.SetBool("SitImmediate", true);
        dennis.GetComponent<SpriteRenderer>().flipX = true;        
        StartCoroutine(sm.MoveTo(dennis, new Vector3(2.88f, 0, 0), 0.01f));
        yield return new WaitForSeconds(0.02f);
        nm.owm.canMove = false;
        dennisAnim.SetBool("SitImmediate", false);
        Debug.Log("Can't move");
        transitMan.readyForFade = true;       
        yield return new WaitUntil(() => nm.owm.canMove == true);
        Debug.Log("Can Move but shouldn't");
        nm.owm.canMove = false;
        Debug.Log("Can't move again");
        convoDone = false;

        nm.dbAnim.ResetTrigger("Popdown");
        
        convoStartNS1(10);
        //nm.dbAnim.SetTrigger("Popup");
        yield return new WaitUntil(() => convoDone);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        yield return new WaitForSeconds(2);
        dennisAnim.SetInteger("Scene1", 0);
        StartCoroutine(sm.MoveTo(dennis, new Vector3(-4.71f, 0, 0), 1f));
        dennis.GetComponent<SpriteRenderer>().flipX = false;
        dennisAnim.SetInteger("Scene2", 6);
        yield return new WaitUntil(() => sm.finished);
        dennisAnim.SetTrigger("ResetSitting");
        yield return new WaitUntil(() => nm.room == 4);
    }



    public void convoChecker(int dia, int scriptedConvo) //if the conversation has events, they're called from here. If the conversation has no events, this should immediately break.
    {
        switch (dia)
        {
            case 0:
                StartCoroutine(convo0Events(dia, scriptedConvo));
                break;
            case 1:
                break;
            case 2:
                StartCoroutine(convo2Events(dia, scriptedConvo));
                break;
            case 3:
                StartCoroutine(convo2Events(dia, scriptedConvo));
                break;
            case 4:
                StartCoroutine(convo3Events(dia, scriptedConvo));
                break;
            case 5 :
                StartCoroutine(SingleLineConvos(dia, scriptedConvo));
                break;
            case 6:
                StartCoroutine(convo4Events(dia, scriptedConvo));
                break;
            case 7:
                StartCoroutine(SingleLineConvos(dia, scriptedConvo));
                break;
            case 8:
                StartCoroutine(SingleLineConvos(dia, scriptedConvo));
                break;
            case 9:
                StartCoroutine(convo9Events(dia, scriptedConvo));
                break;
            case 10:
                Debug.Log("Call thing");
                StartCoroutine(DennisConvo2(dia, scriptedConvo));
                break;
        }
    }

    IEnumerator convo0Events(int dia, int scriptedConvo) //called from convochecker. These are where 'events' throughout conversations like people turning around or walking should be called.
    {
        switch (scriptedConvo) //requires setting a lot of animation bools for Dennis and Player.
        {
            case 0: //the convo starts with Dennis looking up.
                dh.ongoingEvent = true;
                dennisAnim.SetInteger("Scene1", 1);
                dh.ongoingEvent = false;
                break;
            case 1:
                dh.ongoingEvent = true;
                dennisAnim.SetInteger("Scene1", 2);
                StartCoroutine(sm.MoveTo(dennis, new Vector3 (-2.85f, 0, 0), 0.8f));
                yield return new WaitForSeconds(0.5f);
                dennisSR.sortingOrder = 9;
                yield return new WaitUntil(() => sm.finished == true);
                dennisAnim.SetInteger("Scene1", 3);
                dh.ongoingEvent = false;
                break;
            case 2:
                dh.ongoingEvent = true;
                playerAnim.SetTrigger("OfficeDennis");
                yield return new WaitUntil(() => animationFlag == true);
                animationFlag = false;
                dennisAnim.SetInteger("Scene1", 5);
                dh.ongoingEvent = false;
                break;
            case 3:
                dh.ongoingEvent = true;
                yield return new WaitUntil(() => animationFlag == true);
                animationFlag = false;
                dennisAnim.SetInteger("Scene1", 6);
                player.transform.SetParent(dennis.transform);
                StartCoroutine(sm.MoveTo(dennis, new Vector3(6f, 0, 0), 0.8f));
                playerSR.sortingOrder = 9;
                playerAnim.SetTrigger("OfficeDennis");
                yield return new WaitUntil(() => sm.finished == true);
                dennisSR.sortingOrder = 4;
                dennisAnim.SetInteger("Scene1", 7);
                player.transform.SetParent(playerHolder.transform);
                chair = GameObject.Find("Chair");
                playerAnim.SetTrigger("OfficeDennis");
                playerSR.flipX = true;
                chair.SetActive(false);
                dennisSR.flipX = true;
                StartCoroutine(sm.MoveTo(dennis, new Vector3(-2.25f, 0, 0), 0.5f));
                yield return new WaitUntil(() => sm.finished == true);
                dennisAnim.SetInteger("Scene1", 8);
                dh.ongoingEvent = false;
                break;
            case 4:
                dh.ongoingEvent = true;
                dennisAnim.SetInteger("Scene1", 10);
                dh.ongoingEvent = false;
                break;
            case 5:
                break;
            case 6:
                dh.ongoingEvent = true;
                dennisAnim.SetInteger("Scene1", 11);
                dh.ongoingEvent = false;
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                dh.ongoingEvent = true;
                dennisAnim.SetInteger("Scene1", 12);
                dh.ongoingEvent = false;
                break;
            case 10:
                dh.ongoingEvent = true;
                dennisAnim.SetInteger("Scene1", 13);
                dh.ongoingEvent = false;
                break;
            case 11:
                dh.ongoingEvent = true;
                StartCoroutine(nm.isQuestion(dh.Scripted[dia], scriptedConvo + 1)); //dh.scriptedConvo[scriptedConvo]
                yield return new WaitUntil(() => nm.dbChoiceSS == true);
                StartCoroutine(nm.choiceChecker());
                StartCoroutine(SecondTimer(1f));
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
                }
                else if (dh.choiceSelected == 2)
                {
                    Debug.Log("no choice made");
                }
                dh.ongoingEvent = false;
                dh.autoAdvance = true; //both should be flagged - ineffcient, might streamline later
                nm.autoAdvance = true;
                break;
            case 12:
                break;
            case 13:
                dh.ongoingEvent = true;
                dennisAnim.SetInteger("Scene1", 14);
                yield return new WaitUntil(() => animationFlag == true);
                animationFlag = false;
                sm = player.GetComponent<ScriptedMovement>();
                StartCoroutine(sm.MoveTo(player, new Vector3(3.5f, 0, 0), 0.3f));                
                dennisAnim.SetInteger("Scene1", 15);
                playerAnim.SetTrigger("OfficeDennis");
                dh.ongoingEvent = false;
                dh.autoAdvance = true;
                nm.owm.canMove = false;
                break;
        }
    }

    IEnumerator convo2Events(int dia, int scriptedConvo) //called from convochecker. These are where 'events' throughout conversations like people turning around or walking should be called.
    {
        
        switch (scriptedConvo)
        {
            case 0:
                loops++;
                dh.ongoingEvent = true;
                waitForScript = true;
                nm.bt.gameObject.GetComponent<ActionSelector>().isReady = false;
                dh.ongoingEvent = false;
                break;
            case 1:
                dh.ongoingEvent = true;
                combatUIAnim = gameObject.transform.Find("Canvas").Find("CombatUI").GetComponent<Animator>();
                if (loops <= 1)
                {
                    combatUIAnim.SetBool("Looping", true);
                    combatUIAnim.SetTrigger("Options");
                }
                dh.ongoingEvent = false;
                break;
            case 2:
                dh.ongoingEvent = true;
                combatUIAnim.SetBool("Looping", false);
                yield return new WaitForEndOfFrame();
                dh.ongoingEvent = false;
                break;
            case 3:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 4:
                dh.ongoingEvent = true;
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                waitForScript = false;
                nm.bt.gameObject.GetComponent<ActionSelector>().isReady = true;
                dh.ongoingEvent = false;
                break;
            case 5:
                dh.ongoingEvent = true;
                if (loops <= 1) {
                    combatUIAnim.SetBool("Looping", true);
                    combatUIAnim.SetTrigger("Ingredient");
                    yield return new WaitForSeconds(1.5f);
                    combatUIAnim.SetBool("Looping", false);
                }
                dh.ongoingEvent = false;
                break;
        }
    }

    public void convoStartNS1(int whichConvo)
    {
        switch (whichConvo)
        {
            case 3:
                StartCoroutine(dh.GenericFirstConvo(3, true));
                break;
            case 4:
                StartCoroutine(dh.GenericFirstConvo(4, true));
                break;
            case 5:
                StartCoroutine(dh.GenericFirstConvo(5, true));
                break;
            case 6:
                StartCoroutine(dh.GenericFirstConvo(6, true));
                break;
            case 7:
                StartCoroutine(dh.GenericFirstConvo(7, true));
                break;
            case 8:
                StartCoroutine(dh.GenericFirstConvo(8, true));
                break;
            case 9:
                StartCoroutine(dh.GenericFirstConvo(9, true));
                break;
            case 10:
                StartCoroutine(dh.GenericFirstConvo(10, false));
                break;
            case 11:
                StartCoroutine(dh.GenericFirstConvo(11, false));
                break;
        }
    }

    IEnumerator convo3Events(int dia, int scriptedConvo) //called from convochecker. These are where 'events' throughout conversations like people turning around or walking should be called.
    {
        switch (scriptedConvo)
        {
            case 0:
                dh.ongoingEvent = true;
                waitForScript = true;
                nm.bt.gameObject.GetComponent<ActionSelector>().isReady = false;
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
                combatUIAnim.SetBool("Looping", true);
                combatUIAnim.SetTrigger("Time");
                dh.ongoingEvent = false;
                break;
            case 4:
                dh.ongoingEvent = true;
                combatUIAnim.SetBool("Looping", false);
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                waitForScript = false;
                nm.bt.gameObject.GetComponent<ActionSelector>().isReady = true;
                tutEnemy = GameObject.FindGameObjectWithTag("BattleEnemy");
                te = tutEnemy.GetComponent<TutorialEnemy>();
                StartCoroutine(te.EnemyTimer());
                te.convoToCall++;
                te.seconds = 10;
                dh.ongoingEvent = false;
                break;
            case 5:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
        }
    }
    IEnumerator convo4Events(int dia, int scriptedConvo) //called from convochecker. These are where 'events' throughout conversations like people turning around or walking should be called.
    {
        switch (scriptedConvo)
        {
            case 0:
                dh.ongoingEvent = true;
                waitForScript = true;
                nm.bt.gameObject.GetComponent<ActionSelector>().isReady = false;
                dh.ongoingEvent = false;
                break;
            case 1:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 2:
                dh.ongoingEvent = true;
                TV = GameObject.FindGameObjectWithTag("TV");
                TV.GetComponent<Animator>().SetTrigger("Go");
                dh.ongoingEvent = false;
                break;
            case 3:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 4:
                dh.ongoingEvent = true;
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                waitForScript = false;
                nm.bt.gameObject.GetComponent<ActionSelector>().isReady = true;
                tutEnemy = GameObject.FindGameObjectWithTag("BattleEnemy");
                te = tutEnemy.GetComponent<TutorialEnemy>();
                StartCoroutine(te.EnemyTimer());
                te.seconds = 10;
                dh.ongoingEvent = false;
                break;
            case 5:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
        }
    }

    IEnumerator SingleLineConvos(int dia, int scriptedConvo) {
        yield return new WaitForSeconds(0);
        switch (dia)
        {
            case 5:
                break;
            case 7:
                waitForScript = true;
                dh.ongoingEvent = true;
                nm.bt.gameObject.GetComponent<ActionSelector>().isReady = false;
                yield return new WaitForSeconds(1);
                nm.bt.gameObject.GetComponent<ActionSelector>().isReady = true;
                tutEnemy = GameObject.FindGameObjectWithTag("BattleEnemy");
                te = tutEnemy.GetComponent<TutorialEnemy>();
                StartCoroutine(te.EnemyTimer());
                te.seconds = 10;
                waitForScript = false;
                nm.bt.gameObject.GetComponent<ActionSelector>().isReady = true;
                dh.ongoingEvent = false;
                break;
            case 8:
                waitForScript = true;
                dh.ongoingEvent = true;
                nm.bt.gameObject.GetComponent<ActionSelector>().isReady = false;
                yield return new WaitForSeconds(1);
                nm.bt.gameObject.GetComponent<ActionSelector>().isReady = true;
                //te.convoToCall++;
                tutEnemy = GameObject.FindGameObjectWithTag("BattleEnemy");
                te = tutEnemy.GetComponent<TutorialEnemy>();
                StartCoroutine(te.EnemyTimer());
                te.seconds = 10;
                waitForScript = false;
                nm.bt.gameObject.GetComponent<ActionSelector>().isReady = true;
                dh.ongoingEvent = false;
                break;
        }
    }

    IEnumerator convo9Events(int dia, int scriptedConvo) //called from convochecker. These are where 'events' throughout conversations like people turning around or walking should be called.
    {
        yield return new WaitForSeconds(0);
        switch (scriptedConvo)
        {
            case 0:
                dh.ongoingEvent = true;
                waitForScript = true;
                nm.bt.gameObject.GetComponent<ActionSelector>().isReady = false;
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
                Debug.Log("battle should close now");
                StartCoroutine(nm.bt.EndOfBattle(false));
                waitForScript = false;
                dh.ongoingEvent = false;
                break;
        }
    }
    IEnumerator DennisConvo2(int dia, int scriptedConvo)
    {
        yield return new WaitForSeconds(0);
        switch (scriptedConvo)
        {
            case 0:
                dh.ongoingEvent = true;

                animationFlag = false;
                playerAnim.SetTrigger("OfficeDennis");
                yield return new WaitUntil(() => animationFlag == true);
                animationFlag = false;
                dennisAnim.SetInteger("Scene1", 5);
                yield return new WaitUntil(() => !nm.db.enabled);
                nm.db.enabled = true;
                nm.textTS.enabled = true;
                nm.imageTS.enabled = true;
                nm.nameTS.enabled = true;
                dh.ongoingEvent = false;
                break;
            case 1:
                dh.ongoingEvent = true;
                playerAnim.SetTrigger("ResetIdle");
                dennisAnim.SetInteger("Scene2", 1);
                dh.ongoingEvent = false;
                break;
            case 2:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 3:
                dh.ongoingEvent = true;
                dennisAnim.SetInteger("Scene2", 2);
                dh.ongoingEvent = false;
                break;
            case 4:
                dh.ongoingEvent = true;
                dennisAnim.SetInteger("Scene2", 3);
                dh.ongoingEvent = false;
                break;
            case 5:
                dh.ongoingEvent = true;
                dennisAnim.SetBool("Loop", true);
                dennisAnim.SetInteger("Scene2", 1);
                dh.ongoingEvent = false;
                break;
            case 6:
                dh.ongoingEvent = true;
                dennisAnim.SetBool("Loop", false);
                dh.ongoingEvent = false;
                break;
            case 7:
                dh.ongoingEvent = true;
                dennis.GetComponent<SpriteRenderer>().flipX = false;
                dennisAnim.SetInteger("Scene2", 4);
                dh.ongoingEvent = false;
                break;
            case 8:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                convoDone = true;
                dennisAnim.SetInteger("Scene2", 5);
                yield return new WaitForSeconds(0.11f);
                dennis.GetComponent<SpriteRenderer>().flipX = true;
                dh.CancelDialogue(true);
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
