using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehavior : MonoBehaviour {

    //other scripts
    public PlayerHealth ph;
    public GameObject player;
    public GameObject burgerSpawner; //This and BCI reference the static script
    public BurgerComponentInstantiator BCI;
    public GameObject mainCamera;
    public AudioSource background;
    public AudioSource victory;
    public GameObject gameController;

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
    public bool movingBackwards;
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

    private void Awake() // this is just to set some things when its being instantiated freely
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        StartCoroutine(StartSets());
    }

    void Start () {
        startPosition = new Vector3(start.position.x, start.position.y, start.position.z);
        endPosition = new Vector3(end.position.x, end.position.y, end.position.z);
        //gameObject.transform.position.Set(start.position.x, start.position.y, start.position.z);
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        ph = player.GetComponent<PlayerHealth>();
        burgerSpawner = GameObject.Find("BurgerSpawner");
        //clock = GameObject.Find("UIClock");
        //clockAnim = clock.GetComponent<Animator>();
        //cheeseAnim = cheese.GetComponent<Animator>();
        //tearAnim = tear.GetComponent<Animator>();
        BCI = burgerSpawner.GetComponent<BurgerComponentInstantiator>();
        movingForwards = true;
        seconds = Mathf.RoundToInt(enemySpeed-2);
        mainCamera = GameObject.Find("Main Camera");
        background = mainCamera.GetComponent<AudioSource>();
        clock = GameObject.Find("ClockUI");
    }

    public void TakeDamage (float finalDamage) //calculates damage taken by enemy
    {
        enemyArmor = baseArmor;
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
        else if (BCI.ketchupDamage == true && vulnKetchup == true){
            finalDamage = finalDamage * 1.5f;
            StartCoroutine(setAboveText("Vulnerable!"));
        }
        else if (BCI.mustardDamage == true && resistsMustard == true){
            finalDamage = finalDamage / 2;
            StartCoroutine(setAboveText("Resisted!"));
        }
        else if (BCI.mustardDamage == true && vulnMustard == true){
            finalDamage = finalDamage * 1.5f;
            StartCoroutine(setAboveText("Vulnerable!"));
        }
        else if (BCI.ketchupDamage == false && BCI.mustardDamage == false && resistsBun == true){
            finalDamage = finalDamage / 2;
            StartCoroutine(setAboveText("Resisted!"));
        }
        SlowEnemy();
        hasBeenDamaged = true;
        cantMove = true;
        damagedPosition = this.transform.position;
        StartCoroutine(SetTicksWhenReady());
        clockAnim.SetBool("Stopped", true);
        if (finalDamage > 0)
        {
            enemyHealth = enemyHealth - finalDamage;
            anim.SetTrigger("Damaged");
            HealthText.text = Mathf.RoundToInt(enemyHealth).ToString();
            healthBar.SetTrigger("Damaged");
            StartCoroutine(setAboveText(finalDamage + " damage!"));
        } else
        {
            cantMove = false;
            StartCoroutine(setAboveText("No damage!"));
            StartCoroutine(MoveForwards());
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

    IEnumerator SetTicksWhenReady ()
    {
        float tempTicks = ticks;
        yield return new WaitUntil(() => tempTicks != ticks);
        ticks = 0;
    }
    
    public void CheckDeath ()
    {
        if (enemyHealth <= 0)
        {
            anim.SetTrigger("Damaged");
            anim.SetTrigger("Dead");
            StartCoroutine(BCI.FadeImageToFullAlpha(2, BCI.fadeBlack));
            StartCoroutine(BCI.whenBlackScreen());  
            background.Stop();                  // sound
            victory.Play();                     // sound
            StartCoroutine(gameController.GetComponent<BattleTranistions>().EndOfBattle());     // i think this the only thing i added to this

        } else
        {
            cantMove = false;
            StartCoroutine(MoveForwards());
        }
    }

    public IEnumerator MoveForwards () //movement function. contains turn reset
    {

        if (movingForwards == true && hasBeenDamaged == false && cantMove == false)
        {
            yield return new WaitForSeconds(0.01f);
            ticks++;
            transform.position = Vector3.Lerp(startPosition, endPosition, (ticks/(50f * enemySpeed)));
            if (seconds <= 0)
            {
                Attack();
            } else
            {
                StartCoroutine(MoveForwards());
            }
        }
        else if (movingForwards == true && hasBeenDamaged == true && cantMove == false)
        {
            yield return new WaitForSeconds(0.01f);
            ticks++;
            transform.position = Vector3.Lerp(damagedPosition, endPosition, (ticks/50f));
            if (transform.position == endPosition)
            {
                Attack();
            }
            else
            {
                StartCoroutine(MoveForwards());
            }
        }
        else if (movingBackwards == true && cantMove == false)
        {
            yield return new WaitForSeconds(0.01f);
            ticks++;
            transform.position = Vector3.Lerp(endPosition, startPosition, (ticks/50f));
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
                seconds = Mathf.RoundToInt(enemySpeed-2);
                timerStarted = false;
                attackCalled = false;
                clockAnim.SetBool("Stopped", false);
            }
            else
            {
                StartCoroutine(MoveForwards());
            }
        }
    }

    public void Attack ()
    {
        if (attackCalled == false)
        {
            anim.SetTrigger("TimesUp");
        }
        attackCalled = true;
    }

    public void DamagePlayer ()
    {
        Debug.Log("its trying");
        ChanceToMiss();
        ph.DealDamage(enemyDamage);
        movingBackwards = true;
        movingForwards = false;
        ticks = 0;//StartCoroutine(SetTicksWhenReady());
    }

    public void ChanceToMiss()
    {
        finalMissChance = baseMissChance;
        if (cryingStacks > 0)
        {
            tear.SetActive(true);
        }
        for (int i = 1; i <= cryingStacks; i++)
        {
            finalMissChance = finalMissChance + (20 / i);
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
            cheeseText.text = slowStacks.ToString();
            if (cheese.activeInHierarchy == false)
            {
                cheese.SetActive(true);
            }
        }
    }

    public void setStartTimer ()
    {
        StartCoroutine(StartTimerSet());        
    }

    public IEnumerator StartTimerSet() {
        if (secondsText == null) {
            yield return new WaitUntil(() => secondsText != null);
        }
        secondsText.text = (enemySpeed - 2).ToString();
    }

    public IEnumerator EnemyTimer ()
    {
        if (clock == null) {
            yield return new WaitUntil(() => clock != null);
        }
        if (clock.activeInHierarchy == false)
        {
            clock.SetActive(true);
        }
        yield return new WaitForSeconds(1f);
        if (hasBeenDamaged == false || movingBackwards == false)
        {
            seconds--;
            if (secondsText == null) {
                yield return new WaitUntil(() => secondsText != null);
            }
            secondsText.text = seconds.ToString();
            if (seconds > 0)
            {
                StartCoroutine(EnemyTimer());
            }
        } else
        {
            secondsText.text = "";
        }
    }

    public IEnumerator StartSets() {
        while (clock == null || clockAnim == null || start == null || end == null || secondsText == null || HealthText == null || healthBar == null || cheese == null || cheeseText == null || tearText == null || tear == null || GameObject.Find("HealthText").GetComponent<FollowWithOffset>().target == null || GameObject.Find("EnemyHealth_0").GetComponent<FollowWithOffset>().target == null) {
            start = GameObject.Find("EnemyStart").transform;
            end = GameObject.Find("EnemyEnd").transform;
            secondsText = GameObject.Find("enemytimer").GetComponent<Text>();
            HealthText = GameObject.Find("HealthText").GetComponent<TextMesh>();
            healthBar = GameObject.Find("EnemyHealth_0").GetComponent<Animator>();
            clock = GameObject.Find("ClockUI");
            clockAnim = GameObject.Find("ClockUI").GetComponent<Animator>();
            cheeseText = GameObject.Find("cheesetext").GetComponent<TextMesh>();
            cheese = GameObject.Find("cheese slow_0");
            cheeseAnim = cheese.GetComponent<Animator>();
            tearText = GameObject.Find("TearText").GetComponent<TextMesh>();
            tear = GameObject.Find("Tear (1)_0");
            tearAnim = tear.GetComponent<Animator>();
            GameObject.Find("HealthText").GetComponent<FollowWithOffset>().target = this.gameObject.transform;
            GameObject.Find("EnemyHealth_0").GetComponent<FollowWithOffset>().target = this.gameObject.transform;
            for (int i = 0; i < aboveText.Length; i++) {
                aboveText[i].transform.parent = gameController.GetComponent<BattleTranistions>().battle.gameObject.transform;
            }
            yield return new WaitForSeconds(0);
        }
    }
}
