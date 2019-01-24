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

    //public attack stats
    public float drops;
    public float missChance;
    public float enemyArmor;
    public bool vulnKetchup;
    public bool vulnMustard;
    public bool resistsKetchup;
    public bool resistsMustard;
    public bool resistsBun;
    public float enemySpeed;
    public int enemyDamage;
    public float enemyHealth;

    //for use with movement
    public bool beingAttacked; //set by player when attacking
    public bool hasBeenDamaged; //true when already damaged this turn
    public bool movingForwards;
    public bool movingBackwards;
    public bool cantMove;

    float ticks = 0;
    public Vector3 startPosition; //initial position
    public Vector3 endPosition; //in front of player
    public Vector3 damagedPosition; //position where damaged
    public Transform start;
    public Transform end;

    public Animator anim; //animator

    public int seconds;
    public Text secondsText;
    public bool timerStarted;

    void Start () {
        startPosition = new Vector3(start.position.x, start.position.y, start.position.z);
        endPosition = new Vector3(end.position.x, end.position.y, end.position.z);
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        ph = player.GetComponent<PlayerHealth>();
        burgerSpawner = GameObject.Find("BurgerSpawner");
        BCI = burgerSpawner.GetComponent<BurgerComponentInstantiator>();
        movingForwards = true;
        seconds = Mathf.RoundToInt(enemySpeed-2);
    }

    public void TakeDamage (float finalDamage)
    {
        enemyArmor = enemyArmor * (1 - BCI.armorPen);
        finalDamage = finalDamage - enemyArmor;
        hasBeenDamaged = true;
        cantMove = true;
        damagedPosition = this.transform.position;
        ticks = 0;
        if (finalDamage > 0)
        {
            enemyHealth = enemyHealth - finalDamage;
            anim.SetTrigger("Damaged");
            
        } else
        {
            cantMove = false;
            StartCoroutine(MoveForwards());
        }
    }

    /*IEnumerator SetTicksWhenReady ()
    {
        float tempTicks = ticks;
        Wai
    }
    */
    public void CheckDeath ()
    {
        if (enemyHealth <= 0)
        {
            anim.SetTrigger("Damaged");
            anim.SetTrigger("Dead");
        } else
        {
            cantMove = false;
            StartCoroutine(MoveForwards());
        }
    }

    public IEnumerator MoveForwards ()
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
            if (transform.position == startPosition)
            {
                anim.SetTrigger("Reset");
                movingBackwards = false;
                movingForwards = true;
                ticks = 0;
                hasBeenDamaged = false;
                BCI.turns++;
                BCI.canSpawn = true;
                seconds = Mathf.RoundToInt(enemySpeed-2);
                timerStarted = false;
            }
            else
            {
                StartCoroutine(MoveForwards());
            }
        }
    }

    public void Attack ()
    {
        anim.SetTrigger("TimesUp");
    }

    public void DamagePlayer ()
    {
        ph.DealDamage(enemyDamage);
        movingBackwards = true;
        movingForwards = false;
        ticks = 0;
    }

    public void setStartTimer ()
    {
        secondsText.text = (enemySpeed -2).ToString();
    }

    public IEnumerator EnemyTimer ()
    {
        yield return new WaitForSeconds(1f);
        if (hasBeenDamaged == false || movingBackwards == false)
        {
            seconds--;
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

}
