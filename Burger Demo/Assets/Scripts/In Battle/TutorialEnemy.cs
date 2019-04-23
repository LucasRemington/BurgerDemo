using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialEnemy : MonoBehaviour {
    //other scripts
    public PlayerHealth ph;
    public GameObject player;
    public GameObject burgerSpawner; //This and BCI reference the static script
    public BurgerComponentInstantiator BCI;
    public GameObject mainCamera;
    public AudioSource background;
    public AudioSource victory;
    public GameObject gameController;
    public NarrativeScript1 ns1;
    public int[] lastCombo = { 0, 0, 0, 0};
    public int[] classicCombo = { 10, 3, 2, 1 };

    //public attack stats
    public float drops;
    public float baseMissChance;
    public float finalMissChance;
    public int cryingStacks;
    public float baseArmor;
    public float enemyArmor;
    public bool vulnKetchup;
    public bool vulnMustard;
    public bool resistsKetchup;
    public bool resistsMustard;
    public bool resistsBun;
    public float baseSpeed;
    public float enemySpeed;
    public int slowStacks;
    public int baseDamage;
    public int enemyDamage;
    public float enemyHealth;

    //for use with movement
    public bool beingAttacked; //set by player when attacking
    public bool hasBeenDamaged; //true when already damaged this turn
    public bool movingForwards;
    public bool movingBackwards = false;
    public bool cantMove;
    public bool attackCalled;

    public float ticks = 0;
    public Vector3 startPosition; //initial position
    public Vector3 endPosition; //in front of player
    public Vector3 damagedPosition; //position where damaged
    public Transform start;
    public Transform end;

    public Animator anim; //animator

    public int seconds; //for timer at top
    public Text secondsText;
    public bool timerStarted;

    public TextMesh[] aboveText; //text following position
    public int targetAT; //target above text to put words in
    public TextMesh HealthText; // text displaying enemy health
    public Animator healthBar;
    public GameObject clock;
    public Animator clockAnim;
    public TextMesh cheeseText;
    public GameObject cheese;
    public Animator cheeseAnim;
    public TextMesh tearText;
    public GameObject tear;
    public Animator tearAnim;
    public GameObject LightningBolt;
    public bool animFlag; //checked and unchecked by animation events
    public int convoToCall;

    // James here. Gonna introduce several values specifically for the tutorial and for my own sake of making things easier; it may make some things redundant, unfortunately.
    public int convoStage = 0; // Where we are in the conversation with the Master; ie, which dialogue do we need? Feed this into NarrScr1, which BattleConvoChecker references to decide which conversation to load. 
    public int ingOut; // Whether the player has run out of ingredients once before or nah. 0 = No. 1 = First screw-up. 2 = Second screw-up, and termination.
    public bool wrongCombo; // If the player screwed up by virtue of making the wrong combo, reflect as much.
    public bool fuckedUp; // Whether the player screwed up a stage of the tutorial.
    public ActionSelector actSel; // Our Action Selector script.
    public bool timerEnabled;
    public bool timerInProgress;
    

    private void Awake() // this is just to set some things when its being instantiated freely
    {
        fuckedUp = false;
        gameController = GameObject.FindGameObjectWithTag("GameController");
        StartCoroutine(StartSets());
        burgerSpawner = GameObject.Find("CombatUI").transform.Find("BurgerSpawner").gameObject;
        BCI = burgerSpawner.GetComponent<BurgerComponentInstantiator>();
        BCI.isTutorial = true;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        ns1 = mainCamera.GetComponent<NarrativeScript1>();
        LightningBolt = transform.Find("LightningBolt").gameObject;
        actSel = BCI.actSelect;

    }

    void Start()
    {
        //startPosition = new Vector3(start.position.x, start.position.y, start.position.z);
        //endPosition = new Vector3(end.position.x, end.position.y, end.position.z);
        //gameObject.transform.position.Set(start.position.x, start.position.y, start.position.z);
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        ph = player.GetComponent<PlayerHealth>();

        //clock = GameObject.Find("UIClock");
        //clockAnim = clock.GetComponent<Animator>();
        //cheeseAnim = cheese.GetComponent<Animator>();
        //tearAnim = tear.GetComponent<Animator>();
        
        movingForwards = true;
        seconds = Mathf.RoundToInt(enemySpeed - 2);
        mainCamera = GameObject.Find("MainCamera");
        background = mainCamera.GetComponent<AudioSource>();
        BCI.GetTutorial();
        StartCoroutine(ph.DelayedHealthBarSpawn());
        //clock = GameObject.Find("ClockUI");
    }

    public void RecieveAttack ()
    {
        Debug.Log("Took damage.");
        ns1.waitForScript = true;
        //lastCombo = BCI.LastCombo;
        StartCoroutine(recieveAttackTimer());

        for (int i = 0; i < BCI.actSelect.commandsAvailable.Length; i++) // Removes the ability to select any command at all after a burger is launched.
        {
            BCI.actSelect.commandsAvailable[i] = false;
        }
    }

    public IEnumerator recieveAttackTimer()
    {
        timerInProgress = false; // Make sure anything checking if our timer is running knows we stopped.
        StopCoroutine(StartTimerSet());
        bool same = false; // Tracks if we have the correct combo or not.
        ph.protag.ResetTrigger("Hurt"); // Reset the damage trigger on the player.
        yield return new WaitUntil(() => !ns1.combatUIAnim.GetBool("BCI"));
        yield return new WaitForSeconds(0.5f);

        if (convoStage > 1)
        {
            if (lastCombo.Length == classicCombo.Length) // If we have completed a full burger, we reflect that in our 'same' bool.
            {
                same = true;

                for (int i = 0; i < lastCombo.Length; i++) // With a for-loop, we change the 'same' bool to reflect if we are making the correct combo.
                {
                    if (lastCombo[i] != classicCombo[i])
                        same = false;
                }

                if (!same) // If we're at a point that we're making combos, we are considered to have fucked up if we make the incorrect combo.
                {
                    fuckedUp = true;
                    wrongCombo = true;
                }

                Debug.Log("Full length burger: = " + same);
            }

            for (int i = 1; i < 4; i++)
            {
                if (ns1.nm.bt.ingredients[i] < 1) // If any of our ingredients run out...we done fucked up. Only once we get into combos, though.
                {
                    fuckedUp = true;
                    ingOut++;
                    i = 5; // Break.
                }
            }
        }

        if (timerStarted && seconds < 0)
        {
            fuckedUp = true;
        }

        if (convoStage < 2 || fuckedUp)
        {
            LightningBolt.GetComponent<Animator>().SetTrigger("strike"); // Lightning strike animation, and its necessary delays. Only do so if this is one of the first few stages of the tutorial.
            yield return new WaitForSeconds(1.5f);
        }

        if (BCI.playerDead) // If the player is dead, we do its actions and immediately break.
        {
            fuckedUp = true;
            ns1.BattleConvoChecker(9);
        }

        if (fuckedUp)
        {
            Debug.Log("Done fucked up");
            fuckedUp = false;
            if (ingOut > 0 && (ns1.nm.bt.ingredients[1] < 1 || ns1.nm.bt.ingredients[2] < 1 || ns1.nm.bt.ingredients[3] < 1))
            {
                ns1.BattleConvoChecker(6);
                // Call ingredient out conversation.
            }
            else if (timerStarted && seconds < 0 && seconds > -2)
            {
                ns1.BattleConvoChecker(7);
            }
            else if (wrongCombo)
            {
                wrongCombo = false;
                ns1.BattleConvoChecker(8);
                // Call wrong combo conversation.
            }
            
        }
        else
        {
            convoStage++; // Increment where we are in the conversation by one stage.
            seconds = -2; // Effectively disable our timer.

            Debug.Log("TE: Call ns1.battleconvochecker stage " + convoStage);
            ns1.BattleConvoChecker(convoStage); // Call the BattleConvoChecker.
        }

        

        /*if (convoToCall <= 3)
        {
            ns1.convoStartNS1(convoToCall + 3);
            convoToCall++;
            seconds = -2;
        } else if (same) {
            Debug.Log("correct combo");
            convoToCall++;
            ns1.convoStartNS1(convoToCall + 3);
            seconds = -2;
        }
        else
        {
            Debug.Log("incorrect combo");
            ns1.convoStartNS1(7);
        }*/
        BCI.StopAllCoroutines();
    }

    public void TakeDamage(float finalDamage) //calculates damage taken by enemy
    {
        Debug.Log("tookdamage");
        enemyArmor = baseArmor;
        Debug.Log(BCI);
        Debug.Log(enemyArmor);
        enemyArmor = enemyArmor - (enemyArmor * BCI.armorPen * 0.01f);
        if (enemyArmor <= 0)
        {
            enemyArmor = 0;
        }
        finalDamage = finalDamage - enemyArmor;
        if (BCI.ketchupDamage == true && resistsKetchup == true) //resistances and weaknesses
        {
            finalDamage = finalDamage / 2;
            StartCoroutine(setAboveText("Resisted!"));
        }
        else if (BCI.ketchupDamage == true && vulnKetchup == true)
        {
            finalDamage = finalDamage * 1.5f;
            StartCoroutine(setAboveText("Vulnerable!"));
        }
        else if (BCI.mustardDamage == true && resistsMustard == true)
        {
            finalDamage = finalDamage / 2;
            StartCoroutine(setAboveText("Resisted!"));
        }
        else if (BCI.mustardDamage == true && vulnMustard == true)
        {
            finalDamage = finalDamage * 1.5f;
            StartCoroutine(setAboveText("Vulnerable!"));
        }
        else if (BCI.ketchupDamage == false && BCI.mustardDamage == false && resistsBun == true)
        {
            finalDamage = finalDamage / 2;
            StartCoroutine(setAboveText("Resisted!"));
        }
        SlowEnemy();
        hasBeenDamaged = true;
        cantMove = true;
        damagedPosition = this.transform.position;
        StartCoroutine(SetTicksWhenReady());
        //clockAnim.SetBool("Stopped", true);                           fix this later
        if (finalDamage > 0)
        {
            enemyHealth = enemyHealth - finalDamage;
            anim.SetTrigger("Damaged");
            HealthText.text = Mathf.RoundToInt(enemyHealth).ToString();
            healthBar.SetTrigger("Damaged");
            StartCoroutine(setAboveText(finalDamage + " damage!"));
        }
        else
        {
            cantMove = false;
            StartCoroutine(setAboveText("No damage!"));
            //StartCoroutine(MoveForwards());
        }
        if (cryingStacks > 0)
        {
            StartCoroutine(setAboveText("Crying!"));
        }
    }

    public IEnumerator setAboveText(string text)
    {
        for (int i = 0; i < aboveText.Length; i++)
        {
            if (aboveText[i].text == "")
            {
                aboveText[i].text = text;
                yield return new WaitForSeconds(2f);
                aboveText[i].text = "";
                i = aboveText.Length + 1;
            }
        }
    }

    IEnumerator SetTicksWhenReady()
    {
        float tempTicks = ticks;
        yield return new WaitUntil(() => tempTicks != ticks);
        ticks = 0;
    }

    public void CheckDeath()
    {
        if (enemyHealth <= 0)
        {
            anim.SetTrigger("Damaged");
            anim.SetTrigger("Dead");
            StartCoroutine(BCI.FadeImageToFullAlpha(2, BCI.fadeBlack));
            StartCoroutine(BCI.whenBlackScreen());
            background.Stop();                  // sound
            victory.Play();                     // sound
            for (int i = 0; i < aboveText.Length; i++)
            {
                aboveText[i].GetComponent<FollowWithOffset>().stop = true;
            }
            StartCoroutine(gameController.GetComponent<BattleTransitions>().EndOfBattle(true));     // i think this the only thing i added to this

        }
        else
        {
            cantMove = false;
            //StartCoroutine(MoveForwards());
        }
    }

    public IEnumerator MoveForwards() //movement function. contains turn reset
    {

        if (movingForwards == true && hasBeenDamaged == false && cantMove == false)
        {
            yield return new WaitForSeconds(0.01f);
            ticks++;
            //transform.position = Vector3.Lerp(startPosition, endPosition, (ticks / (50f * enemySpeed)));
            if (seconds <= 0)
            {
                Attack();
            }
            else
            {
                StartCoroutine(MoveForwards());
            }
        }
        else if (movingForwards == true && hasBeenDamaged == true && cantMove == false)
        {
            yield return new WaitForSeconds(0.01f);
            ticks++;
            //transform.position = Vector3.Lerp(damagedPosition, endPosition, (ticks / 50f));
            if (transform.position == endPosition)
            {
                Attack();
            }
            else
            {
                //StartCoroutine(MoveForwards());
            }
        }
        else if (movingBackwards == true && cantMove == false)
        {
            yield return new WaitForSeconds(0.01f);
            ticks++;
            tear = HealthText.transform.GetChild(3).gameObject;
            //transform.position = Vector3.Lerp(endPosition, startPosition, (ticks / 50f));
            if (transform.position == startPosition) // turn reset
            {
                anim.SetTrigger("Reset");
                if (cryingStacks > 0)
                {
                    tearText.text = cryingStacks.ToString();
                    cryingStacks--;
                }
                else if (slowStacks <= 0)
                {
                    tearText.text = "";
                    if (tear.activeInHierarchy == true)
                    {
                        tearAnim.SetTrigger("tearEnd");
                    }
                }
                if (slowStacks > 0)
                {
                    cheeseText.text = slowStacks.ToString();
                    slowStacks--;
                }
                else if (slowStacks <= 0)
                {
                    cheeseText.text = "";
                    if (cheese.activeInHierarchy == true)
                    {
                        cheeseAnim.SetTrigger("endCheese");
                    }
                }
                enemyDamage = baseDamage;
                movingBackwards = false;
                movingForwards = true;
                StartCoroutine(SetTicksWhenReady());
                hasBeenDamaged = false;
                BCI.turns++;
                BCI.canSpawn = true;
                seconds = Mathf.RoundToInt(enemySpeed - 2);
                timerStarted = false;
                attackCalled = false;
                // clockAnim.SetBool("Stopped", false);                                                             Fix this later too
            }
            else
            {
                //StartCoroutine(MoveForwards());
            }
        }
    }

    public void Attack()
    {
        if (attackCalled == false)
        {
            anim.SetTrigger("TimesUp");
        }
        attackCalled = true;
    }

    public void DamagePlayer()
    {
        ChanceToMiss();
        ph.DealDamage(enemyDamage);
        movingBackwards = true;
        movingForwards = false;
        ticks = 0;//StartCoroutine(SetTicksWhenReady());
    }

    public void ChanceToMiss()
    {
        finalMissChance = baseMissChance;
        tear = HealthText.transform.GetChild(3).gameObject;
        if (cryingStacks > 0)
        {
            tear.SetActive(true);
        }
        for (int i = 1; i <= cryingStacks; i++)
        {
            finalMissChance = finalMissChance + (20 / i);
            tearText = HealthText.transform.GetChild(2).gameObject.GetComponent<TextMesh>();
            tearText.text = cryingStacks.ToString();
        }
        if (finalMissChance >= Random.Range(1, 100))
        {
            enemyDamage = 0;
            StartCoroutine(setAboveText("Miss!"));
        }
    }

    public void SlowEnemy()
    {
        enemySpeed = baseSpeed;
        for (int i = 1; i <= slowStacks; i++)
        {
            enemySpeed = enemySpeed + (2 / i);
        }
        if (baseSpeed != enemySpeed)
        {
            StartCoroutine(setAboveText("Slowed!"));
            cheeseText = HealthText.transform.GetChild(1).gameObject.GetComponent<TextMesh>();
            cheeseText.text = slowStacks.ToString();
            cheese = HealthText.transform.GetChild(4).gameObject;
            if (cheese.activeInHierarchy == false)
            {
                cheese.SetActive(true);
            }
        }
    }

    public void setStartTimer()
    {
        //timerInProgress = true;
        StopCoroutine(StartTimerSet());
        StartCoroutine(StartTimerSet());
    }

    public IEnumerator StartTimerSet()
    {
        if (secondsText == null)
        {
            yield return new WaitUntil(() => secondsText != null);
        }
        secondsText.text = (enemySpeed - 2).ToString();
    }

    public IEnumerator EnemyTimer()
    {
        /*if (clock == null) {
            yield return new WaitUntil(() => clock != null);
        }
        if (clock.activeInHierarchy == false)
        {
            clock.SetActive(true);
        }*/
        yield return new WaitForSeconds(1f);
        seconds--;
        timerInProgress = true;
        if (secondsText == null)
        {
            yield return new WaitUntil(() => secondsText != null);
        }
        secondsText.text = seconds.ToString();
        if (seconds > 0)
        {
            StartCoroutine(EnemyTimer());
        }
        else if (seconds == 0)
        {
            //ns1.convoStartNS1(5);
            //LightningBolt.GetComponent<Animator>().SetTrigger("strike");
            secondsText.text = "00";
            //seconds = (int)enemySpeed - 2;
            //StartCoroutine(EnemyTimer());

            StartCoroutine(BCI.ClearBurger(true));
            BCI.bciDone = true;
            actSel.enemyReset = true;
            RecieveAttack();
            BCI.gameObject.SetActive(false);
        }
        else {
            secondsText.text = "";
        }
        
    }

    public IEnumerator StartSets()
    {
        while (/*clock == null || clockAnim == null || start == null || end == null ||*/ secondsText == null || HealthText == null || healthBar == null || cheese == null || cheeseText == null || tearText == null || tear == null || GameObject.Find("HealthText").GetComponent<FollowWithOffset>().target == null || GameObject.Find("EnemyHealth_0").GetComponent<FollowWithOffset>().target == null)
        {
            /*start = GameObject.Find("EnemyStart").transform;
            end = GameObject.Find("EnemyEnd").transform;*/
            aboveText[0] = GameObject.Find("FullBattlePrefab").transform.GetChild(3).GetComponent<TextMesh>();
            aboveText[1] = GameObject.Find("FullBattlePrefab").transform.GetChild(4).GetComponent<TextMesh>();
            aboveText[2] = GameObject.Find("FullBattlePrefab").transform.GetChild(5).GetComponent<TextMesh>();
            aboveText[3] = GameObject.Find("FullBattlePrefab").transform.GetChild(6).GetComponent<TextMesh>();
            aboveText[4] = GameObject.Find("FullBattlePrefab").transform.GetChild(7).GetComponent<TextMesh>();
            aboveText[5] = GameObject.Find("FullBattlePrefab").transform.GetChild(8).GetComponent<TextMesh>();
            aboveText[6] = GameObject.Find("FullBattlePrefab").transform.GetChild(9).GetComponent<TextMesh>();
            aboveText[7] = GameObject.Find("FullBattlePrefab").transform.GetChild(10).GetComponent<TextMesh>();
            aboveText[8] = GameObject.Find("FullBattlePrefab").transform.GetChild(11).GetComponent<TextMesh>();
            secondsText = GameObject.Find("enemytimer").GetComponent<Text>();
            HealthText = GameObject.Find("HealthText").GetComponent<TextMesh>();
            healthBar = GameObject.Find("EnemyHealth_0").GetComponent<Animator>();
            //clock = GameObject.Find("ClockUI");
            //clockAnim = GameObject.Find("ClockUI").GetComponent<Animator>();
            cheeseText = HealthText.transform.GetChild(1).gameObject.GetComponent<TextMesh>();
            cheese = HealthText.transform.GetChild(4).gameObject;
            cheeseAnim = cheese.GetComponent<Animator>();
            tearText = HealthText.transform.GetChild(2).gameObject.GetComponent<TextMesh>();
            tear = HealthText.transform.GetChild(3).gameObject;
            tearAnim = tear.GetComponent<Animator>();
            GameObject.Find("HealthText").GetComponent<FollowWithOffset>().target = this.gameObject.transform;
            GameObject.Find("EnemyHealth_0").GetComponent<FollowWithOffset>().target = this.gameObject.transform;
            for (int i = 0; i < aboveText.Length; i++)
            {
                //aboveText[i].GetComponent<FollowWithOffset>().stop = false;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1.5f);
        gameObject.GetComponent<Animator>().SetTrigger("Awake");
    }

    void AppearSound()
    {
        GetComponent<AudioSource>().Play();
    }

    public void triggerAnim() {
        //does nothing but fixes a bug
    }
}

