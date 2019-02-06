using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BurgerComponentInstantiator : MonoBehaviour {

    //necessary scripts + gameobjects
    public PlayerHealth ph;
    public GameObject player;
    public EnemyBehavior eb;
    public GameObject enemy;

    public float burgerPosition; //add 0.2 per pixel of the object
    float formerBurgerPosition; //burger position before prefab was instantiated
    public float sinkBP; //reduces burgerposition by amount

    public bool canSpawn; //called by animations when they go static
    //bool bottomPlaced; //set when bottom bun is placed - defunct
    bool topPlaced; //set when top bun is placed
    public bool spawnReset; //checked by individual component to despawn
    public int componentNumber; //number of components 
    public int bounceTrigger; //used to determine bouncing
    public int componentCap; //caps components to this int
    public int turns; //tracks how many turns have passed
    public int critRoll; //variable for tracking crit

    public GameObject bottomBun; //all prefabs that are instantiated
    public GameObject tomato;
    public GameObject lettuce;
    public GameObject onion;
    public GameObject bacon;
    public GameObject sauce;
    public GameObject pickles;
    public GameObject ketchup;
    public GameObject mustard;
    public GameObject cheese;
    public GameObject patty;
    public GameObject topBun;

    public int[] finalCombo; //array checked against combos when topbun comes down
    public int fcArray; //array location set by components
    public Text comboText; // text that displays combo name
    bool noCombo; //true when no combo matches final array;

    public Text[] infoText; //gives info about ingredients
    public Animator[] iconAnim; //icons flash when buttons are pressed
    public Text[] iconText; //text giving information about ingredients

    //combo strings. identity: 0 = blank 1 = tomato 2 = lettuce 3 = onion 4 = bacon 5 = sauce 6 = pickles 7 = ketchup 8 = mustard 9 = cheese 10 = patty
    public int[] classicCombo3 = { 7, 8, 6, 10, 9, 1, 2, 3, 5, 0 };
    public int[] baconCombo = { 9, 10, 9, 4, 10, 9, 4, 7, 5, 0 };
    public int[] simpleCombo = { 10, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public int[] classicCombo = { 10, 1, 2, 3, 0, 0, 0, 0, 0, 0 };
    public int[] veggieCombo = { 2, 10, 2, 0, 0, 0, 0, 0, 0, 0 };
    public int[] weepCombo = { 3, 10, 3, 2, 1, 0, 0, 0, 0, 0 };
    public int[] doubleCombo = { 10, 1, 2, 3, 10, 0, 0, 0, 0, 0 };
    public int[] doubleCombo3 = { 10, 1, 2, 3, 10, 0, 0, 0, 0, 0 };

    //final attack stats - ingredient modified
    public float dropMult; //multiplies loot recieved when fight is over
    public float heal; //amount healed
    public int crying; //adds to enemy miss chance - stacked with diminishing returns
    public float damageMult; //multiplies damage
    public float critChance; //adds flat amount to percentage crit chance
    public float armorPen; //penetrates enemy armor by flat amount
    public bool ketchupDamage; // if active, deals ketchup damage
    public bool mustardDamage; // if active, deals mustard damage
    public int slow; //slows enemy by percentage amount - stacked with diminishing returns
    public float damage; //raw damage number
    public float finalDamage; //final damage number after multipliers and resistances are taken into account
    public bool pattyDropped; // active when a patty is dropped
    public bool playerDead; // true when dead

    // player variables INV = inventory
    public int[] ingredientINV = new int[10];

    // text displaying stats
    public Text dropText;
    public Text healText;
    public Text cryingText;
    public Text critChanceText;
    public Text armorPenText;
    public Text damageTypeText;
    public Text slowText;
    public Text damageText;

    //beginning and ending UI
    public Image fadeBlack; //fades in and out
    public Text winOrLose;
    public GameObject lootRecieved;
    public Text totalLoot;
    public Image[] lootIcon;
    public Text[] lootText;
    public Text replayDemo;

    //called on start
    void Start () {
        player = GameObject.Find("Player");
        ph = player.GetComponent<PlayerHealth>();
        enemy= GameObject.Find("Friedman");
        eb = enemy.GetComponent<EnemyBehavior>();
        canSpawn = true;
        finalCombo = new int[componentCap];
        turns = 1;
        StartCoroutine(ComponentSpawn(KeyCode.Space, 3, bottomBun, 0)); //bottom bun must spawn before others
        ingredientINV[0] = 1;
        ingredientINV[10] = 1;
        //IconDimmer();
        IconTextUpdate();
        StartCoroutine(enableCheats());
        StartCoroutine(FadeImageToZeroAlpha(1, fadeBlack));
    }

    void IconTextUpdate()
    {
        for (int i = 0; i < 9; i++)
        {
          iconText[i].text = ingredientINV[i+1].ToString();   
        }
    }

    void IconDimmer() //dims icons who are at 0 inventory
    {
        for (int i = 0; i < 9; i++)
        {
            if (ingredientINV[i] == 0 && i != 0)
            {
                iconAnim[i].SetTrigger("Dim");
            }
        }
    }

    IEnumerator enableCheats () //press p for unlimited ammo
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.P) == true);
        ph.playerHealth = 100;
        for (int i = 1; i < 10; i++)
        {
            if (ingredientINV[i] == 0)
            {
                iconAnim[i-1].SetTrigger("Brighten");
            }
            ingredientINV[i] = 99;
        }
        IconTextUpdate();
        StartCoroutine(enableCheats());
    }

    IEnumerator ComponentSpawn(KeyCode key, float pixels, GameObject prefab, int identity)//controls how components spawn 
    //key: corresponding keyboard key 
    //pixels: number of pixels to occupy 
    //prefab: desired prefab 
    //identity: 0 = buns 1 = tomato 2 = lettuce 3 = onion 4 = bacon 5 = sauce 6 = pickles 7 = ketchup 8 = mustard 9 = cheese 10 = patty
    {
        yield return new WaitUntil(() => Input.GetKeyDown(key) == true || topPlaced == true);
        if (topPlaced == false)
        {
            yield return new WaitUntil(() => canSpawn == true);

            if (ingredientINV[identity] > 0)
            {
                if (identity >= 1 && identity <= 9)
                {
                    ingredientINV[identity] = ingredientINV[identity] - 1;
                }
                canSpawn = false;
                componentNumber++;
                if (identity > 0 && identity < 10)
                {
                    iconAnim[identity - 1].SetTrigger("Flash");
                }
                switch (identity) //checks identity of placed prefab to determine position relative to BurgerPosition, and also pass the correct arguments to the coroutine. There's probably a more efficient way to do this.
                {
                    case 0: //called only for bottom bun. starts the process
                        Instantiate(prefab, new Vector3(0, burgerPosition, 0), Quaternion.identity);
                        ResetAttack();
                        StartCoroutine(ComponentSpawn(KeyCode.A, 1, tomato, 1));
                        StartCoroutine(ComponentSpawn(KeyCode.S, 1, lettuce, 2));
                        StartCoroutine(ComponentSpawn(KeyCode.D, 1, onion, 3));

                        StartCoroutine(ComponentSpawn(KeyCode.Q, 1, bacon, 4));
                        StartCoroutine(ComponentSpawn(KeyCode.W, 1, sauce, 5));
                        StartCoroutine(ComponentSpawn(KeyCode.E, 1, pickles, 6));

                        StartCoroutine(ComponentSpawn(KeyCode.Z, 1, ketchup, 7));
                        StartCoroutine(ComponentSpawn(KeyCode.X, 1, mustard, 8));
                        StartCoroutine(ComponentSpawn(KeyCode.C, 1, cheese, 9));

                        StartCoroutine(ComponentSpawn(KeyCode.LeftShift, 2, patty, 10));
                        StartCoroutine(TopBunSpawn());
                        break;
                    case 1:
                        Instantiate(prefab, new Vector3(0, burgerPosition - 0.2f - sinkBP, 0), Quaternion.identity);
                        StartCoroutine(ComponentSpawn(KeyCode.A, 1, tomato, 1));
                        break;
                    case 2:
                        Instantiate(prefab, new Vector3(0, burgerPosition - 0.2f - sinkBP, 0), Quaternion.identity);
                        StartCoroutine(ComponentSpawn(KeyCode.S, 1, lettuce, 2));
                        break;
                    case 3:
                        Instantiate(prefab, new Vector3(0, burgerPosition - 0.2f - sinkBP, 0), Quaternion.identity);
                        StartCoroutine(ComponentSpawn(KeyCode.D, 1, onion, 3));
                        break;
                    case 4:
                        Instantiate(prefab, new Vector3(0, burgerPosition - 0.2f - sinkBP, 0), Quaternion.identity);
                        StartCoroutine(ComponentSpawn(KeyCode.Q, 1, bacon, 4));
                        break;
                    case 5:
                        Instantiate(prefab, new Vector3(0, burgerPosition - 0.2f - sinkBP, 0), Quaternion.identity);
                        StartCoroutine(ComponentSpawn(KeyCode.W, 1, sauce, 5));
                        break;
                    case 6:
                        Instantiate(prefab, new Vector3(0, burgerPosition - 0.2f - sinkBP, 0), Quaternion.identity);
                        StartCoroutine(ComponentSpawn(KeyCode.E, 1, pickles, 6));
                        hidePickles(0.2f, true);
                        break;
                    case 7:
                        Instantiate(prefab, new Vector3(0, burgerPosition - 0.2f - sinkBP, 0), Quaternion.identity);
                        StartCoroutine(ComponentSpawn(KeyCode.Z, 1, ketchup, 7));
                        hidePickles(0.2f, true);
                        break;
                    case 8:
                        Instantiate(prefab, new Vector3(0, burgerPosition - 0.2f - sinkBP, 0), Quaternion.identity);
                        StartCoroutine(ComponentSpawn(KeyCode.X, 1, mustard, 8));
                        hidePickles(0.2f, true);
                        break;
                    case 9:
                        Instantiate(prefab, new Vector3(0, burgerPosition - 0.4f - sinkBP, 0), Quaternion.identity);
                        StartCoroutine(ComponentSpawn(KeyCode.C, 1, cheese, 9));
                        break;
                    case 10:
                        Instantiate(prefab, new Vector3(0, burgerPosition - sinkBP, 0), Quaternion.identity);
                        StartCoroutine(ComponentSpawn(KeyCode.LeftShift, 2, patty, 10));
                        ph.DealDamage(2);
                        break;
                }
                setBurger(pixels);
                if (identity != 0)
                {
                    addToCombo(identity);
                    IconTextUpdate();
                    if (ingredientINV[identity] == 0)
                    {
                        iconAnim[identity-1].SetTrigger("Dim");
                    }
                }
            }
        }
    }

    void hidePickles (float x, bool add) //used to 'hide' certain components underneath others
    {
        if (add == true)
        {
            sinkBP = sinkBP + x;
        } else
        {
            sinkBP = 0f;
        }
    }

    void setBurger (float pixels) //this is a bit of legacy code, tracking the former burger position. Could be useful in the future, so I kept it
    {
        formerBurgerPosition = burgerPosition;
        burgerPosition = burgerPosition + (0.2f * pixels);
    }

    IEnumerator TopBunSpawn() //Triggers when the cap is reached or space is pressed a second time.
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) == true || (canSpawn == true && componentNumber >= componentCap));
        canSpawn = false;
        if (componentNumber >= componentCap)
        {
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitUntil(() => canSpawn == true);
        componentNumber = 0;
        topPlaced = true;
        Instantiate(topBun, new Vector3(0, burgerPosition - 0.2f - sinkBP, 0), Quaternion.identity);
        executeCombo();
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) == true && eb.movingBackwards == false|| Input.GetKeyDown(KeyCode.LeftControl) == true);
        if (Input.GetKeyDown(KeyCode.LeftControl) == true)
        {
            StartCoroutine(ClearBurger());
        } else if (Input.GetKeyDown(KeyCode.Space) == true)
        {
            ph.protag.SetTrigger("Attack");
            if (heal > 0)
            {
                ph.HealDamage(Mathf.RoundToInt(heal));
            }
            StartCoroutine(ClearBurger());
        }
    }

    public void LaunchBurger () //unsure if any of this will calculate correctly. Treat with caution. called from protag attack animation
    {
        if (crying > 0) 
        {
            eb.cryingStacks = eb.cryingStacks + crying;
            //crying icon
        }
        if (slow > 0)
        {
            eb.slowStacks = eb.slowStacks + slow;
            //slow icon
        }
        critRoll = Random.Range(1, 100);
        if (critRoll <= critChance)
        {
            finalDamage = finalDamage * 2;
            StartCoroutine(eb.setAboveText("Critical Hit!"));
        }
        eb.TakeDamage(finalDamage);
        eb.drops = eb.drops + dropMult;
        if (dropMult > 0)
        {
            StartCoroutine(eb.setAboveText("+" + dropMult + " Drops!"));
        }
    }

    IEnumerator ClearBurger() //resets most variables
    {
        spawnReset = true;
        hidePickles(0, false);
        topPlaced = false;
        pattyDropped = false;
        heal = 0;
        burgerPosition = 0;
        formerBurgerPosition = 0;
        if (noCombo == true)
        {
            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            yield return new WaitForSeconds(2f);
            noCombo = false;
        }
        clearText();
        spawnReset = false;
        StartCoroutine(ComponentSpawn(KeyCode.Space, 3, bottomBun, 0));
        bounceTrigger = 0;
    }

    void addToCombo(int component) // adds component number to array in order, then activates info text. Also where attack calculations occur
    {
        finalCombo[fcArray] = component;
        fcArray++;
        switch (component) {
            case 0:
                break;
            case 1: //tomato
                damage = damage + 1;
                DropMultUpdate(3, false);
                infoText[componentNumber - 2].text = "+3 Loot Drops, +1 Damage";
                finalDamageCalculator();
                break;
            case 2: //lettuce
                damage = damage + 1;
                HealUpdate(10, false);
                infoText[componentNumber - 2].text = "+10 Healing, +1 Damage";
                finalDamageCalculator();
                break;
            case 3: //onion
                damage = damage + 1;
                CryingUpdate(1, false);
                infoText[componentNumber - 2].text = "+1 Crying, +1 Damage";
                finalDamageCalculator();
                break;
            case 4: //bacon
                damage = damage + 1;
                DamageMultUpdate(10, false);
                infoText[componentNumber - 2].text = "+10% Damage, + 1 Damage";
                finalDamageCalculator();
                break;
            case 5: //sauce
                damage = damage + 1;
                CritUpdate(10, false);
                infoText[componentNumber - 2].text = "+10% Critical, + 1 Damage";
                finalDamageCalculator();
                break;
            case 6: //pickles
                damage = damage + 1;
                ArmorPenUpdate(20, false);
                infoText[componentNumber - 2].text = "+20% Penetration, + 1 Damage";
                finalDamageCalculator();
                break;
            case 7: //ketchup
                damage = damage + 1;
                DamageTypeUpdate(true, false);
                infoText[componentNumber - 2].text = "Ketchup DMG, + 1 Damage";
                finalDamageCalculator();
                break;
            case 8: //mustard
                damage = damage + 1;
                DamageTypeUpdate(false, false);
                infoText[componentNumber - 2].text = "Mustard DMG, + 1 Damage";
                finalDamageCalculator();
                break;
            case 9: //cheese
                damage = damage + 1;
                SlowUpdate(1, false);
                infoText[componentNumber - 2].text = "+1 Slow, +1 Damage";
                finalDamageCalculator();
                break;
            case 10: //patty
                damage = damage + 10;
                if (pattyDropped == false)
                {
                    pattyDropped = true;
                    infoText[componentNumber - 2].text = "Damage Activated";
                } else
                {
                    infoText[componentNumber - 2].text = "+10 Damage";
                }
                finalDamageCalculator();
                break;
        }
    }

    //following functions are for combos
    void clearText()
    {
        for (int i = 0; i < componentCap; i++)
        {
            infoText[i].text = "--";
        }
    }

    bool CheckCombo(int[] comboCheck) // checks if final combo is a combo
    {
       if (finalCombo.Length != comboCheck.Length)
                return false;
            for (int i = 0; i < finalCombo.Length; i++)
            {
                if (finalCombo[i] != comboCheck[i])
                    return false;
            }
            return true;
    }

    IEnumerator setComboText(string combo)
    {
        comboText.text = combo;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) == true || Input.GetKeyDown(KeyCode.LeftControl) == true);
        comboText.text = "";
    }

    void executeCombo() // checks all combos against final combo, resets array for next loop
    {
        fcArray = 0;
        if (CheckCombo(simpleCombo) == true) //checking combos
        {
            StartCoroutine(setComboText("Make It Fast Combo: +1 Slow, You Poor Soul"));
            SlowUpdate(1, false);
        }
        else if (CheckCombo(classicCombo) == true) //checking combos
        {
            StartCoroutine(setComboText("The Classic Combo: +10% Crit Chance"));
            CritUpdate(10, false);
        }
        else if (CheckCombo(veggieCombo) == true) //checking combos
        {
            StartCoroutine(setComboText("The Vegetarian(?) Combo: +5 Healing"));
            HealUpdate(5, false);
        }
        else if (CheckCombo(weepCombo) == true) //checking combos
        {
            StartCoroutine(setComboText("Make Them Weep Combo: +1 Crying, +1 Slow"));
            SlowUpdate(1, false);
            CryingUpdate(1, false);
        }
        else if (CheckCombo(doubleCombo) == true) //checking combos
        {
            StartCoroutine(setComboText("Double Combo: +20% Penetration"));
            ArmorPenUpdate(20, false);
        }
        else if (CheckCombo(classicCombo3) == true) //checking combos
        {
            StartCoroutine(setComboText("The Classic Combo 3: +5% to Everything"));
            DropMultUpdate(1, false);
            HealUpdate(2, false);
            DamageMultUpdate(5, false);
            CritUpdate(5, false);
            ArmorPenUpdate(5, false);
        }
        else if (CheckCombo(baconCombo) == true) //checking combos
        {
            StartCoroutine(setComboText("Definitely Kosher Combo: +15 Damage on First Turn"));
            if (turns == 1)
            {
                damage = damage + 15;
                finalDamageCalculator();
            }
        }
            else
        {
            noCombo = true;
        }
        for (int i = 0; i < componentCap; i++) //returns finalcombo array to all 0
        {
           finalCombo[i] = 0;
        }
    }

    //following functions are for streamlining text and mechanics updates to variables
    void finalDamageCalculator()
    {
        if (pattyDropped == true)
        {
            finalDamage = damage * (1f + (damageMult * 0.01f));
        }
        else
        {
            finalDamage = 0;
        }
        damageText.text = "Damage: " + finalDamage.ToString("F2");
    }

    void DropMultUpdate(int x, bool reset)
    {
        if (reset == false)
        {
            dropMult = dropMult + x;

        } else
        {
            dropMult = 0;
        }
        dropText.text = "+" + dropMult + " Loot Drops";
    }

    void HealUpdate(int x, bool reset)
    {
        if (reset == false)
        {
            heal = heal + x;

        }
        else
        {
            heal = 0;
        }
        healText.text = "+" + heal + " Healing";
    }

    void CryingUpdate(int x, bool reset)
    {
        if (reset == false)
        {
            crying = crying + x;
            if (crying != 1)
            {
                cryingText.text = "+" + crying + " Crying Stacks";
            }
            else
            {
                cryingText.text = "+" + crying + " Crying Stack";
            }
        } else
        {
            crying = 0;
            cryingText.text = "+" + crying + " Crying Stacks";
        }
    }

    void DamageMultUpdate(int x, bool reset)
    {
        if (reset == false)
        {
            damageMult = damageMult + x;
        } else
        {
            damageMult = 0;
        }
    }

    void CritUpdate(int x, bool reset)
    {
        if (reset == false)
        {
            critChance = critChance + x;
        } else
        {
            critChance = 0;
        }
        critChanceText.text = critChance + "% Critical Chance";
    }

    void ArmorPenUpdate(int x, bool reset)
    {
        if (reset == false)
        {
            if (armorPen < 100)
            {
                armorPen = armorPen + x;
            }
        }
        else
        {
            armorPen = 0;
        }
        armorPenText.text = "+" + armorPen + "% Penetration";
    }

    void DamageTypeUpdate(bool isKetchup, bool reset)
    {
        if (isKetchup == true && reset == false)
        {
            if (mustardDamage == true)
            {
                mustardDamage = false;
                damageTypeText.text = "Bun Damage";
            }
            else
            {
                ketchupDamage = true;
                damageTypeText.text = "Ketchup Damage";
            }
        } else if (reset == false && isKetchup == false)
        {
            if (ketchupDamage == true)
            {
                ketchupDamage = false;
                damageTypeText.text = "Bun Damage";
            }
            else
            {
                mustardDamage = true;
                damageTypeText.text = "Mustard Damage";
            }
        } else if (reset == true)
        {
            ketchupDamage = false;
            mustardDamage = false;
            damageTypeText.text = "Bun Damage";
        }
    }

    void SlowUpdate(int x, bool reset)
    {
        if (reset == false)
        {
            slow = slow + 1;
            if (slow != 1)
            {
                slowText.text = "+" + slow + " Slowing Stacks";
            }
            else
            {
                slowText.text = "+" + slow + " Slowing Stack";
            }
        }
        else
        {
            slow = 0;
            slowText.text = "+" + slow + " Slowing Stacks";
        }
    }
    
    void ResetAttack() //Resets variables controlled by all above functions
    {
        damage = 0;
        finalDamageCalculator();
        DropMultUpdate(0, true);
        HealUpdate(0, true);
        CryingUpdate(0, true);
        DamageMultUpdate(0, true);
        CritUpdate(0, true);
        ArmorPenUpdate(0, true);
        DamageTypeUpdate(false, true);
        SlowUpdate(0, true);
    }

    public void UponDeath () //triggers through animation on death
    {
        playerDead = true;
        ClearBurger();
        ResetAttack();
        StartCoroutine(FadeImageToFullAlpha(2, fadeBlack));
    }

    public IEnumerator whenBlackScreen () //controls UI, triggering when screen is fully black
    {
        //yield return new WaitUntil(() => fadeBlack.color.a == 1);
        yield return new WaitForSeconds(1f);
        Debug.Log("fully black");
        if (playerDead == false)
        {
            Debug.Log("player alive");
            winOrLose.enabled = true;
            winOrLose.text = "Order Filled";
            yield return new WaitForSeconds(0.5f);
            float totalDrops = eb.drops;
            lootRecieved.SetActive(true);
            totalLoot.text = totalDrops.ToString() + " Ingredients Recieved";
            yield return new WaitForSeconds(0.25f);
            for (int j = 0; j < lootIcon.Length; j++) //uses enemy behavior and expended ingredients to figure out drops. Or it will, eventually.
            {
                lootIcon[j].enabled = true;
                lootText[j].text = "+1";
            }
            yield return new WaitForSeconds(0.5f);
            replayDemo.enabled = true;
        }
        else if (playerDead == true)
        {
            winOrLose.enabled = true;
            winOrLose.text = "Employee Terminated";
            yield return new WaitForSeconds(0.5f);
            replayDemo.enabled = true;
        }
    }

    public IEnumerator FadeImageToFullAlpha(float t, Image i) //used to fade in and fade out scene, and controls UI for death
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        bool coCalled = false;
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
        while (i.color.a == 1.0f && coCalled == false)
        {
            StartCoroutine(whenBlackScreen());
            coCalled = true;
            yield return null;
        }
    }

    public IEnumerator FadeImageToZeroAlpha(float t, Image i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }

}
