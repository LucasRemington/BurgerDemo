using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSelector : MonoBehaviour
{

    public GameObject BCI;
    public int option;
    public Vector3[] optionMovements;
    public GameObject Indicator;
    public bool isReady;
    public GameObject Player;
    public GameObject choiceText;

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
        if (Input.GetKeyDown(KeyCode.DownArrow) && isReady && option < 4)
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
        else if (Input.GetKeyDown(KeyCode.UpArrow) && isReady && option > 1)
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
        else if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space)) && isReady)
        {
            StartCoroutine(Confirm(option));
        }
    }

    public IEnumerator Confirm(int choice)
    {
        yield return new WaitForSeconds(0);
        switch (choice)
        {
            case 1: // Fight
                isReady = false;
                BCI.SetActive(true);
                Indicator.SetActive(false);
                choiceText.SetActive(false);
                StartCoroutine(BCI.GetComponent<BurgerComponentInstantiator>().ComponentSpawn(KeyCode.Space, 3, BCI.GetComponent<BurgerComponentInstantiator>().bottomBun, 0));
                StartCoroutine(BCI.GetComponent<BurgerComponentInstantiator>().StartStuff());
                StartCoroutine(BCI.GetComponent<BurgerComponentInstantiator>().enableCheats());
                yield return new WaitUntil(() => BCI.activeInHierarchy == false);
                choiceText.SetActive(true);
                Indicator.SetActive(true);
                isReady = true;
                break;
            case 2: // Run
                isReady = false;
                Player.GetComponent<Animator>().SetTrigger("Run");
                yield return new WaitForSeconds(1);
                StartCoroutine(GetComponent<BattleTransitions>().EndOfBattle(false));
                //isReady = true;
                break;
            case 3: // Items

                break;
            case 4: // Combos

                break;
        }
    }
}