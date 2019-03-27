using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionSelector : MonoBehaviour
{

    [Tooltip("NarrativeScript1, attached to the MainCamera.")] public NarrativeScript1 ns1;
    [Tooltip("BurgerComponentInstantiator, attached to the BurgerSpawner object.")] public GameObject BCI;
    [Tooltip("Current option in battle selected.")] public int option;
    [Tooltip("An array for where the indicator moves as each option is selected.")] public Vector3[] optionMovements;
    [Tooltip("The indicator game object, used in selecting commands in battle.")] public GameObject Indicator;
    [Tooltip("Whether a command can actually be selected or not; if yes, a round can start. If no, selecting the option does nothing.")] public bool isReady;
    [Tooltip("Determines if the commands should be in an 'active' state or if they should be grayed out.")] public bool commandReady = true;
    [Tooltip("Pwayer")] public GameObject Player;
    [Tooltip("The text for each command in battle.")] public GameObject[] choiceText;
    [Tooltip("Whether the player can flee from a given fight.")] public bool canRun = false;

    // Use this for initialization
    void Start()
    {
        option = 1;
        //isReady = true;
        //BCI = GameObject.FindGameObjectWithTag("BurgerSpawner");
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < choiceText.Length; i++)
        {
            if (commandReady && (!choiceText[i].activeInHierarchy || !Indicator.activeInHierarchy))
            {
                choiceText[i].SetActive(true);
                choiceText[i].GetComponent<Text>().color = Color.white;
                Indicator.SetActive(true);
            }
            else if (!commandReady && (choiceText[i].activeInHierarchy || Indicator.activeInHierarchy))
            {
                choiceText[i].GetComponent<Text>().color = Color.gray;
                Indicator.SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && isReady && option < 4 && ns1.waitForScript == false)
        {
            option++;
            switch (option)
            {
                case 2:
                    Indicator.transform.Translate(optionMovements[0] * 2.6f);
                    break;
                case 3:
                    Indicator.transform.Translate(optionMovements[1] * 2.6f);
                    break;
                case 4:
                    Indicator.transform.Translate(optionMovements[2] * 2.6f);
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && isReady && option > 1 && ns1.waitForScript == false)
        {
            option--;
            switch (option)
            {
                case 1:
                    Indicator.transform.Translate(-optionMovements[0] * 2.6f);
                    break;
                case 2:
                    Indicator.transform.Translate(-optionMovements[1] * 2.6f);
                    break;
                case 3:
                    Indicator.transform.Translate(-optionMovements[2] * 2.6f);
                    break;
            }
        }
        else if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space)) && isReady && commandReady)
        {
            StartCoroutine(Confirm(option));
        }
    }

    public IEnumerator Confirm(int choice)
    {
        Debug.Log("confirm");
        yield return new WaitForSeconds(0);
        if (ns1.waitForScript == false)
            {
                switch (choice)
                {
                    case 1: // Fight
                        Debug.Log("fight");
                        isReady = false;
                        BCI.SetActive(true);
                        //Indicator.SetActive(false);
                        //choiceText.SetActive(false);
                        StartCoroutine(BCI.GetComponent<BurgerComponentInstantiator>().ComponentSpawn(KeyCode.Space, 3, BCI.GetComponent<BurgerComponentInstantiator>().bottomBun, 0));
                    BCI.GetComponent<BurgerComponentInstantiator>().serveStart = true;
                        StartCoroutine(BCI.GetComponent<BurgerComponentInstantiator>().StartStuff());
                        StartCoroutine(BCI.GetComponent<BurgerComponentInstantiator>().enableCheats());
                        yield return new WaitUntil(() => BCI.activeInHierarchy == false);
                        //choiceText.SetActive(true);
                        Indicator.SetActive(true);
                        isReady = true;
                        break;
                    case 2: // Run
                        if (canRun)
                        {
                            isReady = false;
                            Player.GetComponent<Animator>().SetTrigger("Run");
                            yield return new WaitForSeconds(1);
                            StartCoroutine(GetComponent<BattleTransitions>().EndOfBattle(false));
                        } else {
                            isReady = false;
                            BCI.GetComponent<BurgerComponentInstantiator>().comboText.text = "Can't run from this battle";
                            yield return new WaitForSeconds(1);
                            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z));
                            BCI.GetComponent<BurgerComponentInstantiator>().comboText.text = "";
                            isReady = true; 
                        }
                        //isReady = true;
                        break;
                    case 3: // Items

                        break;
                    case 4: // Combos

                        break;
                }
            } else {
                Debug.Log("wait for script");
            }
        }
    }