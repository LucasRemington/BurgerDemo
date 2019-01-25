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

    void Start () {
        burgerSpawner = GameObject.Find("BurgerSpawner");
        BCI = burgerSpawner.GetComponent<BurgerComponentInstantiator>();
        healthText.text = playerHealth.ToString();
    }
	
	public void DealDamage(int damage) //used to deal damage, takes amount of damage as an argument
    {
        Debug.Log("dealtdamage");
        if (damage >= 3)
        {
            protag.SetTrigger("Damaged");
        }
        previousHealth = playerHealth;
        playerHealth = playerHealth - damage;
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
        healthText.text = playerHealth.ToString();
        if (playerHealth <= 0)
        {
            protag.SetBool("Dead", true);
        }
    }

    public void HealDamage(int healing) //used to heal damage, takes amount of healing as an argument
    {
        healIcon.SetActive(true);
        Debug.Log("gavehealing");
        previousHealth = playerHealth;
        playerHealth = playerHealth + healing;
        if (playerHealth > 100)
        {
            playerHealth = 100;
        }
        roundedHealth = Mathf.FloorToInt(playerHealth / 10);
        roundedPreviousHealth = Mathf.FloorToInt(previousHealth / 10);
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

}
