using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSelector : MonoBehaviour {

    public GameObject BCI;
    public int option;
    public Vector3[] optionLocations;
    public GameObject Indicator;
    public bool isReady;
    public GameObject Player;
    public GameObject choiceText;

	// Use this for initialization
	void Start () {
        option = 1;
        isReady = true;
        //BCI = GameObject.FindGameObjectWithTag("BurgerSpawner");
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.DownArrow) && isReady && option < 4)
        {
            option++;
            Indicator.transform.SetPositionAndRotation(optionLocations[option - 1], Quaternion.identity);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && isReady && option > 1) {
            option--;
            Indicator.transform.SetPositionAndRotation(optionLocations[option - 1], Quaternion.identity);
        }
        else if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space)) && isReady) {
            Confirm(option);
        }
	}

    public IEnumerator Confirm(int choice) {
        yield return new WaitForSeconds(0);        
        switch (choice) {
            case 1: // Fight
                isReady = false;
                BCI.SetActive(true);
                Indicator.SetActive(false);
                choiceText.SetActive(false);
                yield return new WaitUntil(() => BCI.activeSelf == false);
                choiceText.SetActive(true);
                Indicator.SetActive(true);
                isReady = true;  
                break;
            case 2: // Run
                isReady = false;
                Player.GetComponent<Animator>().SetTrigger("Run");
                GetComponent<BattleTransitions>().EndOfBattle(false);
                isReady = true;
                break;
            case 3: // Items

                break;
            case 4: // Combos

                break;
        }
    }
}
