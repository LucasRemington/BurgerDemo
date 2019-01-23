using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    public GameObject burgerSpawner; //This and BCI reference the static script
    public BurgerComponentInstantiator BCI;
    public int playerHealth;
    public int playerHealthMax;
    public int roundedHealth;
    public Animator[] healthAnim;
    public Text healthText;

    void Start () {
        burgerSpawner = GameObject.Find("BurgerSpawner");
        BCI = burgerSpawner.GetComponent<BurgerComponentInstantiator>();
        healthText.text = playerHealth.ToString();
    }
	
	public void DealDamage(int damage) //used to deal damage, takes amount of damage as an argument
    {
        playerHealth = playerHealth - damage;
        roundedHealth = Mathf.FloorToInt(playerHealth / 10);
        healthAnim[roundedHealth].SetBool("Healing", false);
        healthAnim[roundedHealth].SetInteger("Health", playerHealth % 10);
        healthText.text = playerHealth.ToString();
        if (playerHealth <= 0)
        {
            //die
        }
    }

    public void HealDamage(int healing) //used to heal damage, takes amount of healing as an argument
    {
        playerHealth = playerHealth + healing;
        if (playerHealth > playerHealthMax)
        {
            playerHealth = playerHealthMax;
        }
        //if (playerHealth )
        roundedHealth = Mathf.FloorToInt(playerHealth / 10);
        healthAnim[roundedHealth].SetBool("Healing", true);
        healthAnim[roundedHealth].SetInteger("Health", playerHealth % 10);
        healthText.text = playerHealth.ToString();
    }

}
