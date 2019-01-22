using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BurgerComponentInstantiator : MonoBehaviour {

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

    public int[] classicCombo = { 7, 8, 6, 10, 9, 1, 2, 3, 5, 0 };
    public int[] baconCombo = { 9, 10, 9, 4, 10, 9, 4, 7, 5, 0 };

    //called on start
    void Start () {
        canSpawn = true;
        finalCombo = new int[componentCap];
        StartCoroutine(ComponentSpawn(KeyCode.Space, 3, bottomBun, 0)); //bottom bun must spawn before others
    }

    IEnumerator ComponentSpawn(KeyCode key, float pixels, GameObject prefab, int identity) 
        //key: corresponding keyboard key 
        //pixels: number of pixels to occupy 
        //prefab: desired prefab 
        //identity: 0 = buns 1 = tomato 2 = lettuce 3 = onion 4 = bacon 5 = sauce 6 = pickles 7 = ketchup 8 = mustard 9 = cheese 10 = patty
    {
        yield return new WaitUntil(() => Input.GetKeyDown(key) == true || topPlaced == true);
        if (topPlaced == false)
        {
            yield return new WaitUntil(() => canSpawn == true);
            canSpawn = false;
            componentNumber++;
            switch (identity) //checks identity of placed prefab to determine position relative to BurgerPosition, and also pass the correct arguments to the coroutine. There's probably a more efficient way to do this.
            {
                case 0: //called only for bottom bun. starts the process
                    Instantiate(prefab, new Vector3(0, burgerPosition, 0), Quaternion.identity);
                    setBurger(pixels);
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
                    //bottomPlaced = true;
                    break;
                case 1:
                    Instantiate(prefab, new Vector3(0, burgerPosition - 0.2f - sinkBP, 0), Quaternion.identity);
                    setBurger(pixels);
                    addToCombo(identity);
                    StartCoroutine(ComponentSpawn(KeyCode.A, 1, tomato, 1));
                    break;
                case 2:
                    Instantiate(prefab, new Vector3(0, burgerPosition - 0.2f - sinkBP, 0), Quaternion.identity);
                    setBurger(pixels);
                    addToCombo(identity);
                    StartCoroutine(ComponentSpawn(KeyCode.S, 1, lettuce, 2));
                    break;
                case 3:
                    Instantiate(prefab, new Vector3(0, burgerPosition - 0.2f - sinkBP, 0), Quaternion.identity);
                    setBurger(pixels);
                    addToCombo(identity);
                    StartCoroutine(ComponentSpawn(KeyCode.D, 1, onion, 3));
                    break;
                case 4:
                    Instantiate(prefab, new Vector3(0, burgerPosition - 0.2f - sinkBP, 0), Quaternion.identity);
                    setBurger(pixels);
                    addToCombo(identity);
                    StartCoroutine(ComponentSpawn(KeyCode.Q, 1, bacon, 4));
                    break;
                case 5:
                    Instantiate(prefab, new Vector3(0, burgerPosition - 0.2f - sinkBP, 0), Quaternion.identity);
                    setBurger(pixels);
                    addToCombo(identity);
                    StartCoroutine(ComponentSpawn(KeyCode.W, 1, sauce, 5));
                    break;
                case 6:
                    Instantiate(prefab, new Vector3(0, burgerPosition - 0.2f - sinkBP, 0), Quaternion.identity);
                    setBurger(pixels);
                    addToCombo(identity);
                    StartCoroutine(ComponentSpawn(KeyCode.E, 1, pickles, 6));
                    hidePickles(0.2f, true);
                    break;
                case 7:
                    Instantiate(prefab, new Vector3(0, burgerPosition - 0.2f - sinkBP, 0), Quaternion.identity);
                    setBurger(pixels);
                    addToCombo(identity);
                    StartCoroutine(ComponentSpawn(KeyCode.Z, 1, ketchup, 7));
                    hidePickles(0.2f, true);
                    break;
                case 8:
                    Instantiate(prefab, new Vector3(0, burgerPosition - 0.2f - sinkBP, 0), Quaternion.identity);
                    setBurger(pixels);
                    addToCombo(identity);
                    StartCoroutine(ComponentSpawn(KeyCode.X, 1, mustard, 8));
                    hidePickles(0.2f, true);
                    break;
                case 9:
                    Instantiate(prefab, new Vector3(0, burgerPosition - 0.4f - sinkBP, 0), Quaternion.identity);
                    setBurger(pixels);
                    addToCombo(identity);
                    StartCoroutine(ComponentSpawn(KeyCode.C, 1, cheese, 9));
                    break;
                case 10:
                    Instantiate(prefab, new Vector3(0, burgerPosition - sinkBP, 0), Quaternion.identity);
                    setBurger(pixels);
                    addToCombo(identity);
                    StartCoroutine(ComponentSpawn(KeyCode.LeftShift, 2, patty, 10));
                    break;
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
        yield return new WaitUntil(() => canSpawn == true);
        componentNumber = 0;
        canSpawn = false;
        topPlaced = true;
        Instantiate(topBun, new Vector3(0, burgerPosition - 0.2f - sinkBP, 0), Quaternion.identity);
        yield return new WaitForSeconds(1f);
        spawnReset = true;
        hidePickles(0, false);
        //bottomPlaced = false;
        topPlaced = false;
        canSpawn = true;
        burgerPosition = 0;
        formerBurgerPosition = 0;
        executeCombo();
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

    void addToCombo(int component) // adds component number to array in order, then activates info text
    {
        finalCombo[fcArray] = component;
        fcArray++;
        switch (component) {
            case 0:
                break;
            case 1: //tomato
                infoText[componentNumber-2].text = "+% Loot Drops";
                break;
            case 2: //lettuce
                infoText[componentNumber-2].text = "X Healing";
                break;
            case 3: //onion
                infoText[componentNumber-2].text = "+% Crying";
                break;
            case 4: //bacon
                infoText[componentNumber-2].text = "+% Damage";
                break;
            case 5: //sauce
                infoText[componentNumber-2].text = "+% Critical";
                break;
            case 6: //pickles
                infoText[componentNumber-2].text = "+% Penetration";
                break;
            case 7: //ketchup
                infoText[componentNumber-2].text = "Ketchup DMG";
                break;
            case 8: //mustard
                infoText[componentNumber-2].text = "Mustard DMG";
                break;
            case 9: //cheese
                infoText[componentNumber-2].text = "+% Slow";
                break;
            case 10: //patty
                infoText[componentNumber-2].text = "+10 damage";
                break;
        }
    }

    void clearText()
    {
        for (int i = 0; i < componentCap; i++)
        {
            infoText[i].text = "";
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
        yield return new WaitForSeconds(2f);
        comboText.text = "";
    }

    void executeCombo() // checks all combos against final combo, resets array for next loop
    {
        fcArray = 0;
        if (CheckCombo(classicCombo) == true) //checking combos
        {
            StartCoroutine(setComboText("Classic Combo"));
        }
        else if (CheckCombo(baconCombo) == true) //checking combos
        {
            StartCoroutine(setComboText("Bacon Combo"));
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

}
