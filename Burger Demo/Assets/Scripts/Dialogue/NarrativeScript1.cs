using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]

public class NarrativeScript1 : MonoBehaviour {

    //critical game objects
    [Header("Critical Game Objects")]
    [Tooltip("The Main Camera object, found via script. Allows us to pull other objects.")] public GameObject MainCamera;
    [HideInInspector] [Tooltip("Narrative Manager, attached to the camera.")] public NarrativeManager nm;
    [HideInInspector] [Tooltip("Dialogue Holder, attached to the camera.")] public DialogueHolder dh;
    [HideInInspector] [Tooltip("Scripted movement, the object to which it's attached often changes between the player and NPCs.")] public ScriptedMovement sm;
    [HideInInspector] [Tooltip("Our SaveLoad script. Found via script.")] public SaveLoad saveLoad;
    [Tooltip("The animator attached to the combat UI. Set in inspector.")] public Animator combatUIAnim;
    [Tooltip("Our transition manager script, to detect as we load into a new room. Found via script.")] public TransitionManager transitMan;
    [Tooltip("The black screen that fades in and out a bunch to hide things! It might be found via script, honestly idk")] public Image blackScreen;

    [Header("Player & Related")]
    [Tooltip("The player holder, which contains theplayer. Found via script.")] public GameObject playerHolder;
    [Tooltip("The actual overworld player, found via the playerholder.")] public GameObject player;
    [HideInInspector] [Tooltip("The Sprite Renderer attached to the player.")] public SpriteRenderer playerSR;
    [HideInInspector] [Tooltip("The player animator.")] public Animator playerAnim;
    [Tooltip("The animation that plays upon starting a new save file. Or rather, the animator on the meat tube!")] public Animator startAnim;

    [Header("Flags")]
    [Tooltip("A bool that prevents other scripts from functioning if need be. Cough tutorial fight cough.")] public bool waitForScript;
    [Tooltip("Whether a conversation is done.")] public bool convoDone;
    [Tooltip("Set from various animation events in order to time the cutscenes properly.")] public bool animationFlag; //set from animation events to get timing right
    [Tooltip("Set by certain timers, again, for cutscene timing.")] public bool timerFlag; //set by timer to ger timing right
    [Tooltip("Set during events, namely while loops, for whether their conditions are satisfied or not.")] public bool canProceed = true;
    private bool startTimer;

    [Header("Dennis & Related")]
    [Tooltip("It Dennis.")] public GameObject dennis;
    [HideInInspector] [Tooltip("See above, but Denni.s")] public SpriteRenderer dennisSR;
    [HideInInspector] [Tooltip("I hope you get the picture at this point, IT DENNIS, but his animator.")] public Animator dennisAnim;
    [HideInInspector] [Tooltip("The chair in the manager office.")] public GameObject chair;

    [Header("Tutorial & Related")]
    [Tooltip("The Tutorial Enemy; the Master.")] public GameObject tutEnemy;
    [HideInInspector] [Tooltip("The TutorialEnemy behaviour script.")] public TutorialEnemy te;
    [Tooltip("The television present in the training room.")] public GameObject TV;
    [Tooltip("Previously used in calculating the flash animation for the UI in our previous tutorial iteration.")] private int loops;
    [Tooltip("The Master hologram present in the overworld training room.")] public GameObject holoMaster;
    [HideInInspector] [Tooltip("Cough sprite renderer for Master cough")] public SpriteRenderer holomSR;
    [HideInInspector] [Tooltip("COUGH COUGH HOLOMASTER ANIMATOR COUGH")] public Animator holomAnim;
    [Tooltip("The win/loss text object at the end of the tutorial, I presume? The white text.")] public Text winLossText;
    [Tooltip("The master hologram in battle, I think.")] public GameObject masterHologram;
    [Tooltip("Above but the red text at the bottom.")] public Text battleEndText2;
    private bool eventInProgress = false; // Called at the beginning and end of each convo event. Don't call ConvoChecker if we're in the middle of an event.

    [Header("Kitchen & Related")]
    [Tooltip("SET THIS IN THE INSPECTOR. It can be found at Prefabs/BattleEnemies/Dixie. Allows us to fight Dixie I presume.")] public GameObject BattleDixie;
    [Tooltip("The sprite for Dixie's face.")] public Sprite DixieBattleFace;

    [HideInInspector] public Dialogue dennis1; // specific dialogue modified by player choice
    //public string[] choiceFor_dennis1; //string holding the dialogue for choice changes. Kept in for template only





    public void PseudoStart()
    {
        playerHolder = GameObject.FindWithTag("Player");
        player = playerHolder.transform.Find("OverworldPlayer").gameObject;
        playerAnim = player.GetComponent<Animator>();
        playerSR = player.GetComponent<SpriteRenderer>();
        sm = player.GetComponent<ScriptedMovement>();
        MainCamera = GameObject.FindWithTag("MainCamera");
        nm = MainCamera.GetComponent<NarrativeManager>();
        dh = GetComponent<DialogueHolder>();

        if (saveLoad == null)
        {
            saveLoad = GameObject.FindGameObjectWithTag("Base").GetComponent<SaveLoad>();
        }

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
        //StartCoroutine(battleEarly());
    }

    public IEnumerator battleEarly()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.J));
        Debug.Log("j");
        StartCoroutine(nm.bt.StartBattle(masterHologram, true));
        yield return new WaitUntil(() => nm.bt.battling == true);
        StartCoroutine(dh.GenericFirstConvo(2, true));
    }

    // Literally just makes it so that if the player doesn't hit a key, the game will start anyway.
    private IEnumerator BriefTimer()
    {
        yield return new WaitForSeconds(3);
        startTimer = true;
    }

    public IEnumerator eventOne() //first event. Eventually, this will be the 'master function' calling shit in order via coroutines.
    {
        yield return new WaitForSeconds(0.5f);
        // Black screen on startup. Pressing anything fades it out and spawns in the player.
        if (!nm.gameStarted)
        {
            Debug.Log("Launching initial cutscene.");
            if (blackScreen == null)
                blackScreen = GameObject.FindGameObjectWithTag("BlackScreen").GetComponent<Image>();
            blackScreen.gameObject.SetActive(true);
            blackScreen.color = new Color(0, 0, 0, 1);
            StartCoroutine(BriefTimer());
            yield return new WaitUntil(() => Input.anyKey || startTimer);

            StartCoroutine(nm.bci.FadeImageToZeroAlpha(2, blackScreen));
            yield return new WaitForSeconds(4f);

            while (startAnim == null)
            {
                startAnim = GameObject.FindGameObjectWithTag("BirthLocker").GetComponent<Animator>();
                yield return null;
            }

            saveLoad.meatLockerList.Add(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            saveLoad.meatLockerIndex = 0;
            saveLoad.meatLockerPos = startAnim.gameObject.transform.parent.position;

            startAnim.SetTrigger("Start");

            nm.gameStarted = true;
        }



        // Wait until we walk into Dennis' office for the first time. 0.75f waits until we load in before grabbing Dennis and his components.
        Debug.Log("Event 1: Dennis into Master.");
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
        playerAnim.gameObject.GetComponent<SpriteRenderer>().flipX = false;
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
        yield return new WaitUntil(() => nm.owm.canMove);
        nm.owm.canMove = false;
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
        //dh.CancelDialogue(true);
        yield return new WaitUntil(() => !nm.db.enabled); // Waits until the dialogue box closes.
        Debug.Log("BEGIN TUTORIAL");
        StartCoroutine(nm.bt.StartBattle(masterHologram, true));
        yield return new WaitUntil(() => nm.bt.battling == true);
        AudioSource music = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        music.Play();
        yield return new WaitForSeconds(1);
        StartCoroutine(dh.GenericFirstConvo(2, false));
        Debug.Log("Dialogue should begin now.");
        /*yield return new WaitUntil(() => animationFlag == true); //change this to wait until combat finishes + flag set from animation event   (!nm.bt.battling)
        animationFlag = false;*/
        //StartCoroutine(dh.GenericFirstConvo(2, true));
        yield return new WaitUntil(() => /*animationFlag == true && */nm.bt.battling == false); //change this to wait until combat finishes + flag set from animation event 
        Debug.Log("battle is over");
        music.Stop();
        if (!nm.bci.playerDead) // Once the battle is over, this event ends if the player dies midway through.
        {
            battleEndText2.enabled = false;
            winLossText.enabled = false;
            yield return new WaitUntil(() => !eventInProgress);
            /*for (int i = 0; i < 101; i++)
            {
                blackScreen.color = new Color(0, 0, 0, blackScreen.color.a - 0.01f);
                yield return new WaitForEndOfFrame();
            }*/
            //dh.CancelDialogue(true);
            //nm.dbAnim.ResetTrigger("Popdown");
            yield return new WaitUntil(() => nm.owm.canMove == true);
            nm.owm.canMove = false;
            StartCoroutine(nm.bci.FadeImageToZeroAlpha(1f, blackScreen));
            yield return new WaitForSeconds(1.5f);
            convoStartNS1(8);
            yield return new WaitUntil(() => nm.owm.canMove == true);
            nm.owm.canMove = false;
            yield return new WaitUntil(() => dh.scriptedConvoDone[10] == true);
            nm.owm.canMove = true;
            nm.ev++;

            nm.CheckEvent();
        }
        
    }

    public IEnumerator eventTwo() {
        Debug.Log("Event 2 Start");
        yield return new WaitUntil(() => nm.room == 2 || nm.room == 0);
        player.GetComponent<OverworldMovement>().grounded = false;
        dh.CancelDialogue(true);
        nm.dbAnim.ResetTrigger("Popdown");
        nm.combatText = false;
        nm.autoAdvance = false;
        /*if (nm.room == 2)
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
        }*/
        yield return new WaitUntil(() => nm.room == 1);
        Debug.Log("In Dennis' room");
        transitMan.readyForFade = false;
        yield return new WaitForSeconds(1);
        player.GetComponent<SpriteRenderer>().flipX = true;
        dennis = GameObject.FindGameObjectWithTag("Dennis");
        dennis = dennis.transform.Find("dennis").gameObject;
        dennisAnim = dennis.GetComponent<Animator>();
        dennisAnim.SetBool("SitImmediate", true);
        bool left = false;
        if (dennis.transform.position.x < player.transform.position.x)
        {
            dennis.GetComponent<SpriteRenderer>().flipX = true;
            StartCoroutine(sm.MoveTo(dennis, new Vector3(2.88f, 0, 0), 0.01f));
        }
        else
        {
            left = true;
            dennis.GetComponent<SpriteRenderer>().flipX = false;
            StartCoroutine(sm.MoveTo(dennis, new Vector3(-3.04f, 0, 0), 0.01f));
            playerSR.flipX = false;
        }
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
        if (!left)
            convoStartNS1(11);
        else
            convoStartNS1(13);
        //nm.dbAnim.SetTrigger("Popup");
        yield return new WaitUntil(() => convoDone);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        yield return new WaitForSeconds(2);
        dennisAnim.SetInteger("Scene1", 0);
        if (!left)
        {
            dennis.GetComponent<SpriteRenderer>().flipX = false;
            StartCoroutine(sm.MoveTo(dennis, new Vector3(-4.58f, 0, 0), 1f));

        }
        else {
            StartCoroutine(sm.MoveTo(dennis, new Vector3(3.04f, 0, 0), 1f));
            dennis.GetComponent<SpriteRenderer>().flipX = true;
        }

        dennisAnim.SetInteger("Scene2", 6);
        nm.ev++;
        nm.CheckEvent();
        yield return new WaitUntil(() => sm.finished);
        dennis.GetComponent<SpriteRenderer>().flipX = false;
        nm.bt.ingredients[1] = 15;
        nm.bt.ingredients[2] = 15;
        nm.bt.ingredients[3] = 15;
        dennisAnim.SetTrigger("ResetSitting");
    }

    public IEnumerator eventThree() {
        Debug.Log("Event 3 start");
        yield return new WaitUntil(() => nm.room == 4);
        GetComponent<FollowPlayer>().battleCamera = new Vector3(-44, 10, -1);
        player.GetComponent<OverworldMovement>().canMove = false;
        yield return new WaitForSeconds(0.5f);
        dennis = GameObject.FindGameObjectWithTag("Dennis");
        dennisAnim = dennis.GetComponent<Animator>();
        Debug.Log("move now");
        StartCoroutine(sm.MoveTo(dennis, new Vector3(1f, 0, 0), 0.1f));
        yield return new WaitUntil(() => sm.finished);
        dennis.GetComponent<SpriteRenderer>().flipX = false;
        convoStartNS1(12);
        yield return new WaitUntil(() => nm.owm.canMove == true);
        Debug.Log("Can Move but shouldn't");
        nm.owm.canMove = false;
        Debug.Log("Can't move again");
        yield return new WaitUntil(() => dh.scriptedConvo[12] == 1);
        dennisAnim.SetTrigger("Gun");
        yield return new WaitUntil(() => dh.scriptedConvo[12] == 2);
        dennisAnim.SetTrigger("Bounce");
        nm.ev++;
        nm.CheckEvent();
        yield return new WaitUntil(() => dh.scriptedConvo[12] == 3);
        dennisAnim.SetTrigger("Write");
        
    }

    public IEnumerator eventFour() {
        Debug.Log("Event 4 start");
        animationFlag = false;
        yield return new WaitUntil(() => nm.room == 4);
        nm.imageTSCombat.sprite = DixieBattleFace;
        nm.nameTSCombat.text = "Dixie";
        //playerSR.flipX = false;
        MainCamera.GetComponent<FollowPlayer>().battleCamera = new Vector3(-44f, 10, -1);
        yield return new WaitUntil(() => animationFlag && nm.room == 4);
        animationFlag = false;
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(nm.bt.StartBattle(BattleDixie, true));
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(nm.dialogueEnd());
        yield return new WaitUntil(() => nm.bci.gameObject);
        nm.bci.isTutorial = false;
        yield return new WaitUntil(() => nm.BattleDone);
        nm.BattleDone = false;
        if (!nm.BattleWon)
        {
            nm.ev++;
            nm.CheckEvent();
        }
        else {
            nm.ev += 2;
            nm.CheckEvent();
        }

        yield return new WaitForSeconds(1);
        nm.db.GetComponent<Animator>().ResetTrigger("Popdown");
    }

    public IEnumerator eventFive() {
        yield return new WaitUntil(() => nm.room == 4);
        //nm.imageTSCombat.sprite = DixieBattleFace;
        //nm.nameTSCombat.text = "Dixie";
        //playerSR.flipX = false;
        MainCamera.GetComponent<FollowPlayer>().battleCamera = new Vector3(-44f, 10, -1);
        yield return new WaitUntil(() => animationFlag);
        animationFlag = false;
        StartCoroutine(nm.bt.StartBattle(BattleDixie, true));
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(nm.dialogueEnd());
        yield return new WaitUntil(() => nm.bci.gameObject);
        nm.bci.isTutorial = false;
        yield return new WaitUntil(() => nm.BattleDone);
        nm.BattleDone = false;
        if (!nm.BattleWon)
        {
            nm.CheckEvent();
        }
        else
        {
            nm.ev++;
            nm.CheckEvent();
        }
        yield return new WaitForSeconds(1);
        nm.db.GetComponent<Animator>().ResetTrigger("Popdown");
    }

    public IEnumerator eventSix() {
        yield return new WaitUntil(()=>!nm.owm.canMove);
        nm.owm.canMove = true;
        Debug.Log("Event 6 Start");
        yield return new WaitUntil(() => saveLoad.louNotesSeen[0]);
        Debug.Log("Lou Note Seen");
        if (!dennis) {
            dennis = GameObject.FindGameObjectWithTag("Dennis");
        }
        dennis.transform.position = player.transform.position - new Vector3(10,-0.22f,0);
        dennis.GetComponent<Animator>().SetTrigger("Walk");
        nm.owm.canMove = false;

        StartCoroutine(sm.MoveTo(dennis, (player.transform.position - dennis.transform.position - new Vector3(0.6f,0,0))*2, 2));
        yield return new WaitUntil(() => sm.finished);
        dennis.GetComponent<Animator>().SetTrigger("Idle");
        player.GetComponent<SpriteRenderer>().flipX = true;
        convoStartNS1(17);
        yield return new WaitUntil(() => nm.owm.canMove);
        nm.owm.canMove = false;
        player.GetComponent<Animator>().SetTrigger("BeingDragged");
        player.transform.parent = dennis.transform;
        dennis.GetComponent<SpriteRenderer>().flipX = true;
        dennis.GetComponent<Animator>().SetTrigger("DragPlayer");
        dennis.GetComponent<BoxCollider2D>().enabled = true;
        dennis.GetComponent<Rigidbody2D>().simulated = true;
        StartCoroutine(sm.MoveTo(dennis, new Vector3(-37, 0, 0), 2.5f));
        yield return new WaitForSeconds(2.3f);
        //yield return new WaitUntil(() => !player.GetComponent<OverworldMovement>().grounded && sm.finished);
        player.transform.parent = GameObject.FindGameObjectWithTag("Player").transform;
        dennis.GetComponent<Animator>().SetTrigger("Fall");
        yield return new WaitForSeconds(0.9f);
        dennis.GetComponent<Animator>().SetTrigger("HitGround");
        blackScreen = GameObject.FindGameObjectWithTag("BlackScreen").GetComponent<Image>();
        for (int i = 0; i < 100; i++)
        {
            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a + 0.01f);
            yield return new WaitForEndOfFrame();
        }
        nm.ev++;
        nm.CheckEvent();
    }

    public void convoChecker(int dia, int scriptedConvo) //if the conversation has events, they're called from here. If the conversation has no events, this should immediately break.
    {
        Debug.Log("Convo Checker: Calling case " + dia);
        if (true) // Don't start a new event if we're in the middle of one. Actually wait this is recursive, so nevermind?
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
                    StartCoroutine(convo3Events(dia, scriptedConvo));
                    break;
                case 4:
                    StartCoroutine(convo4Events(dia, scriptedConvo));
                    break;
                case 5:
                    StartCoroutine(convo5Events(dia, scriptedConvo));
                    break;
                case 6:
                    StartCoroutine(convo6Events(dia, scriptedConvo));
                    break;
                case 7:
                    StartCoroutine(convo7Events(dia, scriptedConvo));
                    break;
                case 8:
                    StartCoroutine(convo8Events(dia, scriptedConvo));
                    break;
                case 9:
                    StartCoroutine(TutorialDeath(dia, scriptedConvo));
                    break;
                case 10:
                    StartCoroutine(TutorialTimeout(dia, scriptedConvo));
                    break;
                case 11:
                    //Debug.Log("Call thing");
                    StartCoroutine(DennisConvo2(dia, scriptedConvo));
                    break;
                case 12:
                    StartCoroutine(DennisConvo3(dia, scriptedConvo));
                    //Debug.Log("calls the thing");
                    break;
                case 13:
                    StartCoroutine(DennisConvo2(dia, scriptedConvo));
                    break;
                case 14:
                    StartCoroutine(TutorialWrongCombo(dia, scriptedConvo));
                    break;
                case 15:
                    StartCoroutine(TutorialIngredientsOut(dia, scriptedConvo, true));
                    break;
                case 16:
                    StartCoroutine(TutorialIngredientsOut(dia, scriptedConvo, false));
                    break;
                case 17:
                    // FILL THIS IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIN
                    break;
            }

        }
    }

    public void BattleConvoChecker(int convo) // Similar to convoStartNS1, except called explicitly for the tutorial encounter.
    {
        //Debug.Log("NS1: Call dialogue holder.genericfirstconvo at stage " + convo);
        switch (convo)
        {
            case 0: // Burger 1; Bun only. Not called from here.
                Debug.Log("Case 0 has been called in BattleConvoChecker; check code in TutorialEnemy.cs");
                break;
            case 1: // Burger 2, one of each ingredient.
                StartCoroutine(dh.GenericFirstConvo(3, true));
                break;
            case 2: // Burger 3, introduce combos.
                StartCoroutine(dh.GenericFirstConvo(4, true));
                break;
            case 3: // Burger 4, introduce timer.
                StartCoroutine(dh.GenericFirstConvo(5, true));
                break;
            case 4: // Burger 5, repeat timed combo.
                StartCoroutine(dh.GenericFirstConvo(6, true));
                break;
            case 5: // Combat finished.
                StartCoroutine(dh.GenericFirstConvo(7, true));
                break;
            case 6: // Ran out of ingredients. Uses an if-statement for whether the player has already run out of ingredients once or not.
                if (te.ingOut <= 1)
                {
                    StartCoroutine(dh.GenericFirstConvo(15, true));
                }
                else
                {
                    StartCoroutine(dh.GenericFirstConvo(16, true));
                }
                break;
            case 7: // Ran out of time.
                dh.scriptedConvoStart[10] = false;
                dh.scriptedConvo[10] = 0;
                StartCoroutine(dh.GenericFirstConvo(10, true));
                break;
            case 8: // Wrong combo.
                dh.scriptedConvoStart[14] = false;
                dh.scriptedConvo[14] = 0;
                StartCoroutine(dh.GenericFirstConvo(14, true));
                break;
            case 9: // Dead.
                StartCoroutine(dh.GenericFirstConvo(9, true));
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
            case 1: // Dennis runs over, shakes the player's hand.
                dh.ongoingEvent = true;
                dennisAnim.SetInteger("Scene1", 2);
                StartCoroutine(sm.MoveTo(dennis, new Vector3(-2.75f, 0, 0), 0.8f));
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
            case 3: // Dennis grabs the player and drags them over.
                dh.ongoingEvent = true;
                yield return new WaitUntil(() => animationFlag == true);
                animationFlag = false;
                dennisAnim.SetInteger("Scene1", 6);
                player.transform.SetParent(dennis.transform); // Tie player to Dennis
                StartCoroutine(sm.MoveTo(dennis, new Vector3(1.6f, 0, 0), 0.3f)); // Drag player to chair.
                playerSR.sortingOrder = 9;
                playerAnim.SetTrigger("OfficeDennis");
                yield return new WaitUntil(() => sm.finished == true);
                dennisSR.sortingOrder = 4;
                dennisAnim.SetInteger("Scene1", 7);
                player.transform.SetParent(playerHolder.transform); // Untie player from Dennis
                chair = GameObject.Find("Chair");
                playerAnim.SetTrigger("OfficeDennis");
                playerSR.flipX = false;
                chair.SetActive(false);
                dennisSR.flipX = true;
                StartCoroutine(sm.MoveTo(dennis, new Vector3(1.05f, 0, 0), 0.3f)); // Dennis runs from the player's chair to his desk.
                yield return new WaitUntil(() => sm.finished == true);
                GameObject dChair = GameObject.Find("DWB Dennis's Chair");
                dChair.SetActive(false);
                dennisSR.flipX = false;
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
                yield return new WaitForSeconds(0.5f);
                //Debug.Log("timer true");
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
            case 12: // I'll interpret that silence as a resounding yes!
                break;
            case 13:
                dh.ongoingEvent = true;
                dennisAnim.SetInteger("Scene1", 14);
                StartCoroutine(sm.MoveTo(dennis, new Vector3(0.2f, 0, 0), 0.1f)); // Quickly slide Dennis just a tad over to his button so he can hit it properly.
                yield return new WaitUntil(() => animationFlag == true);
                animationFlag = false;

                playerSR.flipX = true;
                yield return new WaitForSeconds(0.5f); // Flip the player, wait a brief moment, and THEN they go flying.

                sm = player.GetComponent<ScriptedMovement>();
                StartCoroutine(sm.MoveTo(player, new Vector3(7.8f, 0, 0), 0.5f));   //And then the player is sent off to the training hallway!    
                //playerSR.flipX = true;
                dennisAnim.SetInteger("Scene1", 15);
                playerAnim.SetTrigger("OfficeDennis");
                dh.ongoingEvent = false;
                dh.autoAdvance = true;
                nm.owm.canMove = false;
                break;
        }
    }

    IEnumerator convo2Events(int dia, int scriptedConvo) //called from convochecker. First Master in-battle conversation. 
    {
        ActionSelector actSel = nm.bt.GetComponent<ActionSelector>();

        switch (scriptedConvo)
        {
            case 0:
                // In the course of your duties, you will be prompted to serve burgers to our loyal customers.
                eventInProgress = true;
                actSel.isReady = false;
                actSel.option = -255;

                tutEnemy = GameObject.FindGameObjectWithTag("BattleEnemy"); // Set our tutorial enemy and relevant script once battle is underway.
                te = tutEnemy.GetComponent<TutorialEnemy>();
                te.secondsText.text = "";

                dh.ongoingEvent = true; // Make this true at the beginning of each case, and false at the end of each. Has dialogue holder wait up on us here.

                combatUIAnim = gameObject.transform.Find("Canvas").Find("CombatUI").GetComponent<Animator>();

                //loops++;

                waitForScript = true;
                dh.ongoingEvent = false;
                break;
            case 1:
                // To begin this process, select Serve from your Command menu by pressing your Action button. [Flash Command menu]
                dh.ongoingEvent = true;
                combatUIAnim.SetTrigger("OptionsFlash"); // Flash command menu.
                actSel.commandsAvailable[0] = true;
                actSel.isReady = true;
                actSel.option = 0;
                yield return new WaitUntil(() => nm.canAdvance); // Wait until the dialogue box finishes
                waitForScript = false;

                canProceed = false; // If we try to move elsewhere in the menu, don't let us. Then wait until we hit our Action button.
                while (!canProceed)
                {
                    if (actSel.option != 0)
                    {
                        actSel.option = 0;
                    }
                    else if (actSel.option == 0 && Input.GetButtonDown("Submit"))
                    {
                        combatUIAnim.SetTrigger("OptionsFlash"); // Stop flashing command menu.
                        //yield return new WaitUntil(() => Input.GetButtonDown("Submit"));
                        canProceed = true;
                    }
                    yield return null;
                }
                yield return new WaitUntil(() => combatUIAnim.GetBool("BCI")); // Once we select it, wait for BCI to load in, as well as at least buns to finally appear before we continue on.
                yield return new WaitUntil(() => nm.bci.iconAnim[0].GetComponent<Image>().enabled);
                actSel.indicatorOn = false;
                dh.autoAdvance = true;

                dh.ongoingEvent = false;
                break;
            case 2:
                // As you can see, your first bun has been placed. Pressing your Action button again will place another, completing the order and delivering it to the customer.
                waitForScript = true;
                dh.ongoingEvent = true;
                combatUIAnim.SetTrigger("BCItrueFlash"); // Flash our main BCI panel instead.
                yield return new WaitUntil(() => actSel.row == 2);

                actSel.row = 0;
                actSel.col = 2;
                actSel.indicatorOn = true;

                yield return new WaitUntil(() => nm.canAdvance); // Wait until our text box is full, then stop waiting for script.
                //combatUIAnim.SetTrigger("BCItrueFlash"); // Stop flashing our BCI main panel once the text box fills up.    // Now handled in BCI itself if this is the first stage of the Master fight.
                //yield return new WaitForEndOfFrame(); // Pass two frames to allow BCI to stop flashing.
                yield return new WaitUntil(() => Input.GetButtonDown("Submit"));
                waitForScript = false;
                dh.ongoingEvent = false;
                eventInProgress = false;


                // CONVERSATION END HERE.
                break;


                /*case 3:
                    dh.ongoingEvent = true;
                    dh.ongoingEvent = false;
                    break;
                case 4:
                    dh.ongoingEvent = true;
                    waitForScript = false;
                    if (loops <= 1)
                    {
                        combatUIAnim.SetBool("Looping", true);
                        combatUIAnim.SetTrigger("Ingredient");
                        yield return new WaitForSeconds(1.5f);
                        combatUIAnim.SetBool("Looping", false);
                    }
                    nm.bt.gameObject.GetComponent<ActionSelector>().isReady = true;
                    dh.ongoingEvent = false;
                    break;
                case 5:
                    dh.ongoingEvent = true;
                    Debug.Log("Convo 2: " + scriptedConvo);
                    nm.bt.gameObject.GetComponent<ActionSelector>().commandReady = true;
                    dh.ongoingEvent = false;
                    break;*/
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
            case 12:
                StartCoroutine(dh.GenericFirstConvo(12, false));
                break;
            case 13:
                StartCoroutine(dh.GenericFirstConvo(13, false));
                break;
            case 17:
                StartCoroutine(dh.GenericFirstConvo(17, false));
                break;
        }
    }

    IEnumerator convo3Events(int dia, int scriptedConvo) //called from convochecker. Second dialogue with the Master in battle. Introduce ingredients.
    {
        ActionSelector actSel = nm.bt.GetComponent<ActionSelector>();
        //Debug.Log("NS1: Convo 3! Convo " + dia+ ", stage " + scriptedConvo);
        switch (scriptedConvo)
        {
            case 0: // Truly pathetic.
                eventInProgress = true;
                dh.ongoingEvent = true;
                waitForScript = true;

                actSel.isReady = false;
                actSel.option = -255;
                actSel.commandsAvailable[0] = false;

                dh.ongoingEvent = false;
                break;
            case 1: // When you perform poorly, you will be punished. Punishment results in the loss of Meat, the very core of your being. Rune out of Meat, and you will be terminated. [Flash Status panel]
                dh.ongoingEvent = true;
                combatUIAnim.SetTrigger("CharacterFlash"); // Flash status panel.
                dh.ongoingEvent = false;
                break;
            case 2: // This time, serve me using ingredients. Select Serve again.
                dh.ongoingEvent = true;
                combatUIAnim.SetTrigger("CharacterFlash"); // Stop flashing status panel.
                actSel.commandsAvailable[0] = true;
                actSel.option = 0;
                actSel.isReady = true;
                yield return new WaitUntil(() => nm.canAdvance); // Wait until the dialogue box finishes
                waitForScript = false;

                // Now we unlock Meat, Tomato, Lettuce, and Cheese.
                nm.bt.ingUnlocked[10] = true;
                nm.bt.ingredients[10] = 0;
                nm.bci.iconAnim[10].SetInteger("Count", 0);
                nm.bt.ingUnlocked[1] = true;
                nm.bt.ingUnlocked[2] = true;
                nm.bt.ingUnlocked[3] = true;
                nm.bci.IconTextUpdate();

                canProceed = false; // If we try to move elsewhere in the menu, don't let us. Then wait until we hit our Action button.
                while (!canProceed)
                {
                    if (actSel.option != 0)
                    {
                        actSel.option = 0;
                    }
                    else if (actSel.option == 0 && Input.GetButtonDown("Submit"))
                    {
                        canProceed = true;
                    }
                    yield return null;
                }
                yield return new WaitUntil(() => combatUIAnim.GetBool("BCI")); // Once we select it, wait for BCI to load in, as well as at least buns to finally appear before we continue on.
                yield return new WaitUntil(() => nm.bci.iconAnim[0].GetComponent<Image>().enabled);
                actSel.isReady = false;
                //yield return new WaitForSeconds(0.25f);
                waitForScript = true;
                dh.autoAdvance = true;

                dh.ongoingEvent = false;
                break;
            case 3: // You currently have four ingredients available to you: Lettuce, Tomato, Cheese, and Meat. As of now, Lettuce is highlighted. [ Flash ingredients ]
                dh.ongoingEvent = true;
                waitForScript = true;
                combatUIAnim.SetTrigger("BCIingFlash"); // Flash our ingredients panel.
                actSel.isReady = false;
                dh.ongoingEvent = false;
                break;
            case 4: // When you have an ingredient highlighted, your Info window will provide vital information, such as how to add it to your burger. [ Flash Info panel ]
                dh.ongoingEvent = true;
                combatUIAnim.SetTrigger("BCIingFlash"); // Stop flashing our ingredients panel.
                combatUIAnim.SetTrigger("InfoFlash"); // Flash our information panel instead.

                yield return new WaitForEndOfFrame();
                //yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

                /*tutEnemy = GameObject.FindGameObjectWithTag("BattleEnemy");
                te = tutEnemy.GetComponent<TutorialEnemy>();
                StartCoroutine(te.EnemyTimer());
                te.convoToCall++;
                te.seconds = 30;*/
                dh.ongoingEvent = false;
                break;
            case 5: // Use your arrow keys to highlight each item, and then follow the instructions to add one of each to your burger. As a new employee, you may only add four total ingredients to each burger.
                dh.ongoingEvent = true;
                combatUIAnim.SetTrigger("InfoFlash"); // Stop flashing our info panel.

                // Now we get ingredients. Reflect that in their animators.
                for (int i = 1; i < 4; i++)
                {
                    nm.bt.ingredients[i] = 1;
                    nm.bci.IconTextUpdate();
                }
                nm.bt.ingredients[10] = 1;
                nm.bci.iconAnim[10].SetInteger("Count", 1);

                while (nm.bci.componentNumber < 4) // Loop up until our top bun spawns in. If we add a patty, we remove the ability to do so so that one of each is mandatory.
                {
                    if (nm.bci.componentNumber >= (4 - nm.bt.ingredients[1] - nm.bt.ingredients[2] - nm.bt.ingredients[3]))
                    {
                        nm.bt.ingredients[10] = 0;
                        nm.bci.iconAnim[10].SetInteger("Count", 0);
                    }
                    yield return null;
                }
                dh.ongoingEvent = false;
                break;
            case 6: // Now, serve me.
                dh.ongoingEvent = true;
                actSel.isReady = true;
                yield return new WaitUntil(() => Input.GetButtonDown("Submit") && nm.canAdvance);
                waitForScript = false;
                dh.ongoingEvent = false;
                eventInProgress = false;
                break;
        }
    }

    IEnumerator convo4Events(int dia, int scriptedConvo) //called from convochecker. Third dialogue with the Master in battle. Introduce combos.
    {
        ActionSelector actSel = nm.bt.GetComponent<ActionSelector>();

        switch (scriptedConvo)
        {
            case 0: // Your creativity is appalling, yet you are improving. The improvement is negligble, but it is improvement nonetheless. [ Disable options, etc]
                dh.ongoingEvent = true;
                eventInProgress = true;
                actSel.isReady = false;
                actSel.commandsAvailable[0] = false;
                waitForScript = true;

                dh.ongoingEvent = false;
                break;
            case 1: // Rather than exercise free will, you will perform Combos. Select the Combos option from your Command menu. [ Lock command to Combos ]
                dh.ongoingEvent = true;
                yield return new WaitUntil(() => nm.canAdvance); // Wait until text box fills.
                waitForScript = false;
                actSel.commandsAvailable[1] = true; // Ready combo menu.
                actSel.isReady = true;

                canProceed = false; // If we try to move elsewhere in the menu, don't let us. Then wait until we hit our Action button.
                while (!canProceed)
                {
                    if (actSel.option != 1)
                    {
                        actSel.option = 1;
                    }
                    else if (actSel.option == 1 && Input.GetButtonDown("Submit"))
                    {
                        canProceed = true;
                    }
                    yield return null;
                }
                dh.ongoingEvent = false;
                yield return new WaitUntil(() => combatUIAnim.GetBool("ItemCombo")); // Once we select it, wait for the Combo menu to load in, as well as the first combo to load in before we continue.
                yield return new WaitUntil(() => actSel.comboHolder.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>()); // This SHOULD return the first combo's image component... ComboHolder -> Row2 ->
                actSel.isReady = false;
                waitForScript = true;
                dh.autoAdvance = true;

                dh.ongoingEvent = false;
                break;
            case 2: // All Combos that you have discovered will appear in your Combo menu. As a new employee, you are already familiar with the Classic Combo #1. [ Flash combo panel ]
                dh.ongoingEvent = true;
                actSel.isReady = false;
                waitForScript = true;

                combatUIAnim.SetTrigger("ItemComboFlash");


                dh.ongoingEvent = false;
                break;
            case 3: // Selecting an individual Combo with your Action button will bring you to the Serve menu. Do so now. [ Stop flashing the Combo menu. Lock selected combo to top left, proceed. ]
                dh.ongoingEvent = true;

                combatUIAnim.SetTrigger("ItemComboFlash");

                nm.bt.ingUnlocked[11] = true; // Enable the trash button here for a moment, just enough that its icon appears.

                yield return new WaitUntil(() => nm.canAdvance); // Wait until text box is full, then allow player to do the thing.
                waitForScript = false;
                actSel.isReady = true;

                canProceed = false; // If we try to move elsewhere in the menu, don't let us. Then wait until we hit our Action button.

                while (!canProceed)
                {
                    if (actSel.row != 2 || actSel.col != 0)
                    {
                        actSel.row = 2;
                        actSel.col = 0;
                    }
                    else if (actSel.row == 2 && actSel.col == 0 && Input.GetButtonDown("Submit"))
                    {
                        canProceed = true;
                    }
                    yield return null;
                }
                yield return new WaitUntil(() => combatUIAnim.GetBool("BCI")); // Once we select it, wait for the Combo menu to load in, as well as the first combo to load in before we continue.
                yield return new WaitUntil(() => nm.bci.iconAnim[0].GetComponent<Image>().enabled); // This SHOULD return the first combo's image component... ComboHolder -> Row2 ->
                actSel.isReady = false;
                waitForScript = true;
                dh.autoAdvance = true;

                dh.ongoingEvent = false;
                break;
            case 4: // Instructions on how to assemble the selected Combo will appear in the Information Panel. Observe.
                dh.ongoingEvent = true;
                waitForScript = true;
                combatUIAnim.SetTrigger("InfoFlash"); // Flash information panel again.
                dh.ongoingEvent = false;
                break;
            case 5: // Note that having an ingredient selected is not necessary to add it to your burger, nor is it necessary to first select a Combo from the Combo menu to serve one. [ Hold. ]
                actSel.isReady = false;
                combatUIAnim.SetTrigger("InfoFlash");
                //waitForScript = true;
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
            case 6: // Also note that you have been given a limited quantity of each ingredient. Although you may discard a failure by pressing your Cancel button, any Ingredients used are forfeit. [ Hold ]
                dh.ongoingEvent = true;

                dh.ongoingEvent = false;
                break;
            case 7: // Serve me. Failure to serve the appropriate Combo will result in punishment. [ Won't serve burger until dialogue is closed ]
                dh.ongoingEvent = true;
                actSel.commandsAvailable[0] = true; // Unlock command menu in case we decide to retreat a menu.
                yield return new WaitUntil(() => nm.canAdvance);
                actSel.isReady = true;
                waitForScript = true;
                nm.bt.ingredients[10] = 1; // Set our ingredients to their proper values, now.
                nm.bt.ingredients[1] = 10;
                nm.bt.ingredients[2] = 10;
                nm.bt.ingredients[3] = 10;
                nm.bt.ingUnlocked[11] = true;
                nm.bci.iconAnim[10].SetInteger("Count", 1);
                nm.bci.IconTextUpdate();
                yield return new WaitUntil(() => Input.GetButtonDown("Submit"));
                yield return new WaitForSeconds(0.1f);
                dh.autoAdvance = true;
                waitForScript = false;
                eventInProgress = false;
                dh.ongoingEvent = false;
                break;
        }
    }

    IEnumerator convo5Events(int dia, int scriptedConvo) //called from convochecker. Fourth dialogue with the Master in battle. Introduce timer.
    {
        ActionSelector actSel = nm.bt.GetComponent<ActionSelector>();

        switch (scriptedConvo)
        {
            case 0: // Hm...acceptable. [ Reset ]
                dh.ongoingEvent = true;
                eventInProgress = true;
                actSel.isReady = false;
                actSel.commandsAvailable[0] = false;
                actSel.commandsAvailable[1] = false;
                waitForScript = true;

                dh.ongoingEvent = false;
                break;
            case 1: // However, customers expect efficiency. From now on, you will be timed. Time starts once select a Command. [ Flash timer, make it appear. ]
                dh.ongoingEvent = true;

                combatUIAnim.SetTrigger("TimerFlash"); // Flash timer here.
                nm.bci.te.setStartTimer();
                te.timerEnabled = true; // Make sure the tutorial enemy script knows to reapply the timer from now on.

                dh.ongoingEvent = false;
                break;
            case 2: // Serve the Classic Combo #1 within the alloted time. Failure will result in additional punishment. [ Unflash timer. Hold. ]
                dh.ongoingEvent = true;
                combatUIAnim.SetTrigger("TimerFlash"); // Stop flashing timer.
                dh.ongoingEvent = false;
                break;
            case 3: // Now, serve me. [ Allow player free reign. ]
                dh.ongoingEvent = true;
                yield return new WaitUntil(() => nm.canAdvance);

                actSel.isReady = true;
                waitForScript = false;
                actSel.commandsAvailable[0] = true;
                actSel.commandsAvailable[1] = true;

                eventInProgress = false;
                dh.ongoingEvent = false;
                break;
        }
    }

    IEnumerator convo6Events(int dia, int scriptedConvo) //called from convochecker. Fifth dialogue with the Master in battle. Final test.
    {
        ActionSelector actSel = nm.bt.GetComponent<ActionSelector>();

        switch (scriptedConvo)
        {
            case 0: // Good. Once more, serve me. [ Reset, wait until dialogue finishes, player regains control. ]
                dh.ongoingEvent = true;
                eventInProgress = true;
                actSel.isReady = false;
                actSel.commandsAvailable[0] = false;
                actSel.commandsAvailable[1] = false;
                waitForScript = true;

                yield return new WaitUntil(() => nm.canAdvance);
                actSel.isReady = true;
                actSel.commandsAvailable[1] = true;
                actSel.commandsAvailable[0] = true;
                waitForScript = false;

                eventInProgress = false;
                dh.ongoingEvent = false;
                break;
        }
    }

    IEnumerator convo7Events(int dia, int scriptedConvo) //called from convochecker. Sixth dialogue with the Master in battle. Ends battle.
    {
        ActionSelector actSel = nm.bt.GetComponent<ActionSelector>();

        switch (scriptedConvo)
        {
            case 0: // Satisfactory. [ Reset. ]
                dh.ongoingEvent = true;
                eventInProgress = true;
                actSel.isReady = false;
                actSel.commandsAvailable[0] = false;
                actSel.commandsAvailable[1] = false;
                waitForScript = true;

                yield return new WaitUntil(() => nm.canAdvance);

                blackScreen = GameObject.FindGameObjectWithTag("BlackScreen").GetComponent<Image>();
                StartCoroutine(nm.bci.FadeImageToFullAlpha(1.5f, blackScreen));


                StartCoroutine(nm.bt.EndOfTutorialBattle(true));

                dh.ongoingEvent = false;
                break;
            case 1: // When a customer is satisfied, they will depart. You will retrieve ingredients, which you will use to serve more customers. [ On black screen. Hold until next dialogue box to let player continue. ]
                dh.ongoingEvent = true;
                winLossText.text = "Training Complete!";
                winLossText.enabled = true;
                yield return new WaitForSeconds(0.25f);
                battleEndText2.text = "Meat Returned";
                battleEndText2.enabled = true;
                dh.ongoingEvent = false;
                break;
            case 2: // Education complete. [ Let the player end dialogue and truly leave combat. ]
                dh.ongoingEvent = true;

                yield return new WaitUntil(() => nm.canAdvance);
                waitForScript = false;

                dh.ongoingEvent = false;
                eventInProgress = false;
                break;
        }
    }

    IEnumerator SingleLineConvos(int dia, int scriptedConvo)
    {
        yield return new WaitForSeconds(0);
        switch (dia)
        {
            case 5:
                nm.bt.gameObject.GetComponent<ActionSelector>().isReady = false;
                nm.bt.gameObject.GetComponent<ActionSelector>().commandReady = false;
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                nm.bt.gameObject.GetComponent<ActionSelector>().isReady = true;
                nm.bt.gameObject.GetComponent<ActionSelector>().commandReady = true;
                break;
            case 7:
                waitForScript = true;
                dh.ongoingEvent = true;

                nm.bt.gameObject.GetComponent<ActionSelector>().isReady = false;
                nm.bt.gameObject.GetComponent<ActionSelector>().commandReady = false;
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

                tutEnemy = GameObject.FindGameObjectWithTag("BattleEnemy");
                te = tutEnemy.GetComponent<TutorialEnemy>();
                StartCoroutine(te.EnemyTimer());
                te.seconds = 30;
                waitForScript = false;
                nm.bt.gameObject.GetComponent<ActionSelector>().isReady = true;
                nm.bt.gameObject.GetComponent<ActionSelector>().commandReady = true;
                dh.ongoingEvent = false;
                break;
            case 8:
                waitForScript = true;
                dh.ongoingEvent = true;
                nm.bt.gameObject.GetComponent<ActionSelector>().isReady = false;
                nm.bt.gameObject.GetComponent<ActionSelector>().commandReady = false;
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                //nm.bt.gameObject.GetComponent<ActionSelector>().isReady = true;
                //te.convoToCall++;
                tutEnemy = GameObject.FindGameObjectWithTag("BattleEnemy");
                te = tutEnemy.GetComponent<TutorialEnemy>();
                StartCoroutine(te.EnemyTimer());
                te.seconds = 30;
                waitForScript = false;
                nm.bt.gameObject.GetComponent<ActionSelector>().isReady = true;
                nm.bt.gameObject.GetComponent<ActionSelector>().commandReady = true;
                dh.ongoingEvent = false;
                break;
        }
    }

    IEnumerator TutorialDeath(int dia, int scriptedConvo) //called from convochecker. These are where 'events' throughout conversations like people turning around or walking should be called.
    {
        yield return new WaitForSeconds(0);
        switch (scriptedConvo)
        {
            case 0: // Failure terminated. Education complete.
                dh.ongoingEvent = true;
                eventInProgress = true;
                waitForScript = true;
                nm.bt.gameObject.GetComponent<ActionSelector>().isReady = false;

                yield return new WaitForSeconds(2.0f);
                StartCoroutine(ForceLoad());

                waitForScript = false;
                eventInProgress = false;
                dh.ongoingEvent = false;
                break;


            case 1:
                dh.ongoingEvent = true;
                Debug.Log("battle should close now");
                blackScreen = GameObject.FindGameObjectWithTag("BlackScreen").GetComponent<Image>();
                for (int i = 0; i < 100; i++)
                {
                    blackScreen.color = new Color(0, 0, 0, blackScreen.color.a + 0.01f);
                    yield return new WaitForEndOfFrame();
                }
                nm.bci.CombatUI.gameObject.transform.Find("HealthBar").gameObject.SetActive(false);
                yield return new WaitForSeconds(1.05f);
                winLossText.text = "Training Complete!";
                winLossText.enabled = true;
                yield return new WaitForSeconds(0.25f);
                battleEndText2.text = "Meat Returned";
                battleEndText2.enabled = true;
                StartCoroutine(nm.bt.EndOfTutorialBattle(false));
                waitForScript = false;
                dh.ongoingEvent = false;
                break;
        }
    }

    IEnumerator convo8Events(int dia, int scriptedConvo) //called from convochecker. Final master conversation.
    {
        nm.owm.canMove = false;
        switch (scriptedConvo)
        {
            case 0:
                dh.ongoingEvent = true;
                //yield return new WaitUntil(() => !nm.db.enabled);
                //nm.db.enabled = true;
                //nm.textTS.enabled = true;
                //nm.imageTS.enabled = true;
                //nm.nameTS.enabled = true;
                dh.ongoingEvent = false;
                break;
            case 5:
                dh.ongoingEvent = true;
                animationFlag = false;
                nm.owm.canMove = true;
                dh.ongoingEvent = false;
                yield return new WaitUntil(() => Input.GetButtonDown("Submit") && nm.canAdvance);
                dh.scriptedConvoDone[10] = true;
                holomAnim.SetTrigger("Sleep");
                dh.CancelDialogue(true);
                break;
        }
    }

    IEnumerator TutorialTimeout(int dia, int scriptedConvo)
    {
        yield return null;
        ActionSelector actSel = nm.bt.GetComponent<ActionSelector>();

        switch (scriptedConvo)
        {
            case 0: // Inefficient. You have run out of time, and as such, have suffered punishment.
                dh.ongoingEvent = true;
                eventInProgress = true;

                actSel.isReady = false;

                dh.ongoingEvent = false;
                break;
            case 1: // Try again.
                dh.ongoingEvent = true;

                yield return new WaitUntil(() => nm.canAdvance);
                actSel.isReady = true;
                actSel.commandsAvailable[0] = true;
                actSel.commandsAvailable[1] = true;
                waitForScript = false;

                eventInProgress = false;
                dh.ongoingEvent = false;
                break;
        }
    }

    IEnumerator TutorialIngredientsOut(int dia, int scriptedConvo, bool firstSet)
    {
        yield return null;
        ActionSelector actSel = nm.bt.GetComponent<ActionSelector>();

        if (firstSet) // First time player runs out of ingredients.
        {
            switch (scriptedConvo)
            {
                case 0: // Carelessly, you have run out of an ingredient.
                    dh.ongoingEvent = true;
                    eventInProgress = true;
                    waitForScript = true;
                    dh.ongoingEvent = false;
                    break;
                case 1: // As this is your first offense, I will replenish your stocks. Do not expect this same generosity again.
                    dh.ongoingEvent = true;
                    nm.bt.ingredients[1] = 10;
                    nm.bt.ingredients[2] = 10;
                    nm.bt.ingredients[3] = 10;

                    yield return new WaitUntil(() => nm.canAdvance);
                    actSel.isReady = true;
                    actSel.commandsAvailable[0] = true;
                    actSel.commandsAvailable[1] = true;
                    waitForScript = false;


                    eventInProgress = false;
                    dh.ongoingEvent = false;
                    break;
            }
        }
        else // Second time player runs out of ingredients.
        {
            switch (scriptedConvo)
            {
                case 0: // ...
                    dh.ongoingEvent = true;
                    eventInProgress = true;
                    actSel.isReady = false;
                    actSel.commandsAvailable[0] = false;
                    actSel.commandsAvailable[1] = false;
                    dh.ongoingEvent = false;
                    break;
                case 1: // If I were not so disappointed, I would almost be impressed at your carelessness.
                    dh.ongoingEvent = true;
                    
                    dh.ongoingEvent = false;
                    break;
                case 2: // Prepare for termination.
                    dh.ongoingEvent = true;
                    waitForScript = true;
                    yield return new WaitForSeconds(1.5f);
                    te.LightningBolt.GetComponent<Animator>().SetTrigger("strike"); // Wait a moment, then kill the player.
                    yield return new WaitForSeconds(1.0f);
                    te.ph.DealDamage(1000);
                    dh.ongoingEvent = false;
                    break;
                case 3: // Failure terminated. Education complete.
                    dh.ongoingEvent = true;
                    yield return new WaitForSeconds(2.0f);
                    StartCoroutine(ForceLoad());
                    eventInProgress = false;
                    dh.ongoingEvent = false;
                    break;
            }
        } 
    }

    IEnumerator TutorialWrongCombo(int dia, int scriptedConvo)
    {
        ActionSelector actSel = nm.bt.GetComponent<ActionSelector>();

        
        switch (scriptedConvo)
        {
            case 0: // Incorrect. The correct Combo is present in your Combo menu: pressing your Action button when you have it selected will provide detailed assembly instructions while serving me.
                dh.ongoingEvent = true;
                eventInProgress = true;
                waitForScript = true;
                actSel.isReady = false;

                dh.ongoingEvent = false;
                break;
            case 1: // Try again.
                dh.ongoingEvent = true;

                yield return new WaitUntil(() => nm.canAdvance);
                actSel.isReady = true;
                actSel.commandsAvailable[0] = true;
                actSel.commandsAvailable[1] = true;
                waitForScript = false;
                eventInProgress = false;
                dh.ongoingEvent = false;
                break;
        }
    }

    IEnumerator ForceLoad()
    {
        Debug.Log("Waiting for battle to end and dialogue box to go away...");
        yield return new WaitUntil(() => !nm.db.enabled && !nm.bt.battling);
        Debug.Log("LOADING");
        StartCoroutine(saveLoad.LoadGame(true));
        //StopAllCoroutines();
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
            case 8:
                dh.ongoingEvent = true;
                dennisAnim.SetInteger("Scene2", 2);
                dh.ongoingEvent = false;
                break;
            case 9:
                dh.ongoingEvent = true;
                dennisAnim.SetInteger("Scene2", 3);
                dh.ongoingEvent = false;
                break;
            case 10:
                dh.ongoingEvent = true;
                dennisAnim.SetBool("Loop", true);
                dennisAnim.SetInteger("Scene2", 1);
                dh.ongoingEvent = false;
                break;
            case 11:
                dh.ongoingEvent = true;
                dennisAnim.SetBool("Loop", false);
                dh.ongoingEvent = false;
                break;
            case 12:
                dh.ongoingEvent = true;
                dennis.GetComponent<SpriteRenderer>().flipX = false;
                dennisAnim.SetInteger("Scene2", 4);
                dh.ongoingEvent = false;
                break;
            case 13:
                dh.ongoingEvent = true;
                convoDone = true;
                dennisAnim.SetInteger("Scene2", 5);
                yield return new WaitForSeconds(0.11f);
                dennis.GetComponent<SpriteRenderer>().flipX = true;
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                dh.CancelDialogue(true);
                dh.ongoingEvent = false;
                break;
            default:
                dh.ongoingEvent = true;
                dh.ongoingEvent = false;
                break;
        }
    }

    public IEnumerator DennisConvo3(int dia, int scriptedConvo)
    {
        yield return new WaitForSeconds(0);
        switch (scriptedConvo)
        {
            case 0:
                dh.ongoingEvent = true;
                yield return new WaitForSeconds(0.5f);
                dennisAnim.SetTrigger("Gun");
                dh.ongoingEvent = false;
                break;
            case 1:
                dh.ongoingEvent = true;
                dennisAnim.SetTrigger("Bounce");
                dh.ongoingEvent = false;
                break;
            case 2:
                dh.ongoingEvent = true;
                dennisAnim.SetBool("Looping", true);
                dennisAnim.SetTrigger("Write");
                yield return new WaitUntil(() => Input.GetButtonDown("Submit"));
                dennis.GetComponent<SpriteRenderer>().flipX = true;
                dh.ongoingEvent = false;
                break;

        }
    }
    public IEnumerator DennisConvo4(int dia, int scriptedConvo) {
        yield return null;
        switch (scriptedConvo)
        {
            case 0:
                dh.ongoingEvent = true;
                yield return new WaitForSeconds(0.5f);
                dennisAnim.SetTrigger("Gun");
                dh.ongoingEvent = false;
                break;
            case 1:
                dh.ongoingEvent = true;
                dennisAnim.SetTrigger("Bounce");
                dh.ongoingEvent = false;
                break;
            case 2:
                dh.ongoingEvent = true;
                dennisAnim.SetBool("Looping", true);
                dennisAnim.SetTrigger("Write");
                yield return new WaitUntil(() => Input.GetButtonDown("Submit"));
                dennis.GetComponent<SpriteRenderer>().flipX = true;
                dh.ongoingEvent = false;
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