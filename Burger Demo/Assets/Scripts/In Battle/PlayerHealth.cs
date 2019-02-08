using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    public GameObject burgerSpawner; //This and BCI reference the static script
    public BurgerComponentInstantiator BCI;
    public int playerHealth;
    public int previousHealth;
    public int playerHealthMax;
    public int roundedHealth;
    public int roundedPreviousHealth;
    public Animator[] healthAnim;
    public Animator protag;
    public Text healthText;
    public GameObject healIcon; //icon that pops up for healing
    public GameObject gameController;
    public int shields;

    void Start () {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        burgerSpawner = GameObject.Find("BurgerSpawner");
        playerHealth = gameController.GetComponent<BattleTranistions>().playerHealth;       // takes from the gameController for now, will probably be changed to the overworld player controller
        BCI = burgerSpawner.GetComponent<BurgerComponentInstantiator>();
        healthText.text = playerHealth.ToString();

        for (int i = 0; i < healthAnim.Length; i++)
        {
            healthAnim[i].SetBool("Healing", true);
            if (i == roundedHealth)
            {
                healthAnim[roundedHealth].SetInteger("Health", playerHealth % 10);
            }
            else if (i == roundedPreviousHealth)
            {
                healthAnim[roundedPreviousHealth].SetInteger("Health", 10);
            }
        }
        healthText.text = playerHealth.ToString();
    }
	
	public void DealDamage(int damage) //used to deal damage, takes amount of damage as an argument
    {
        int newDamage = damage;
        Debug.Log("dealtdamage");
        if (shields > 0) {
            newDamage = damage - shields;
        }
        if (newDamage >= 3)
        {
            protag.SetTrigger("Hurt");
        }
        if (newDamage >= 0) {
            shields = 0;
            previousHealth = playerHealth;
            playerHealth = playerHealth - newDamage;
            roundedHealth = Mathf.FloorToInt(playerHealth / 10);
            roundedPreviousHealth = Mathf.FloorToInt(previousHealth / 10);
            for (int i = 0; i < healthAnim.Length; i++)
            {
                healthAnim[i].SetBool("Healing", false);
                if (i == roundedHealth)
                {
                    healthAnim[roundedHealth].SetInteger("Health", playerHealth % 10);
                }
                else if (i == roundedPreviousHealth)
                {
                    healthAnim[roundedPreviousHealth].SetInteger("Health", 0);
                }
            }
        }
        else
        {
            shields -= damage;
        }
        
        healthText.text = playerHealth.ToString();
        if (playerHealth <= 0)
        {
            protag.SetBool("CombatLost", true);
        }
    }

    public void HealDamage(int addShields) //used to heal damage, takes amount of healing as an argument
    {
        healIcon.SetActive(true);
        Debug.Log("gaveShields");
        if ((playerHealth + addShields) >= playerHealthMax) {
            addShields = (100 - playerHealth);
            shields = addShields;
        }
        else {
            shields += addShields;
            if (playerHealth + shields >= playerHealthMax) {
                shields = (100 - playerHealth);
            }
        }
        previousHealth = playerHealth;
        playerHealth = playerHealth + addShields;
        roundedHealth = Mathf.FloorToInt(playerHealth / 10);
        roundedPreviousHealth = Mathf.FloorToInt(previousHealth / 10);
        for (int i = 0; i < healthAnim.Length; i++)                                         // if health bar changes, this also needs to change
        {
            healthAnim[i].SetBool("Healing", true);
            if (i == roundedHealth)
            {
                healthAnim[roundedHealth].SetInteger("Health", playerHealth % 10);
            }
            else if (i == roundedPreviousHealth)
            {
                healthAnim[roundedPreviousHealth].SetInteger("Health", 10);
            }
        }
        healthText.text = playerHealth.ToString();
    }
    public void healthUpdate() {
        gameController.GetComponent<BattleTranistions>().playerHealth = playerHealth;
    }
}
