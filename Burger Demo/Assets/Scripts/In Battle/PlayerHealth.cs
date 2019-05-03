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
    public Image healthBar;
    public Image shieldBar;

    void Start () {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        healthBar = GameObject.FindGameObjectWithTag("HealthBar").transform.Find("HealthBar Fill").GetComponent<Image>();
        shieldBar = GameObject.FindGameObjectWithTag("HealthBar").transform.Find("HealthBar Shield").GetComponent<Image>();
        burgerSpawner = GameObject.Find("CombatUI").transform.GetChild(2).gameObject;
        playerHealth = gameController.GetComponent<BattleTransitions>().playerHealth;       // takes from the gameController for now, will probably be changed to the overworld player controller
        playerHealthMax = gameController.GetComponent<BattleTransitions>().playerHealthMax; // also takes from the gameController
        BCI = burgerSpawner.GetComponent<BurgerComponentInstantiator>();
        healthText = GameObject.Find("healthtext").GetComponent<Text>();
        healthText.text = playerHealth.ToString();
        healthBar.fillAmount = (float)playerHealth / (float)playerHealthMax;
        healthText.text = playerHealth.ToString();
        StartCoroutine(DelayedHealthBarSpawn());
    }

    public IEnumerator DelayedHealthBarSpawn() {
        yield return new WaitForSeconds(1);
        if (healthBar == null)
            healthBar = GameObject.FindGameObjectWithTag("HealthBar").transform.Find("HealthBar Fill").GetComponent<Image>();
        healthBar.transform.parent.gameObject.SetActive(true);
    }
	
	public void DealDamage(int damage) //used to deal damage, takes amount of damage as an argument
    {
        int newDamage = damage;
        Debug.Log("PlayerHealth: Dealt damage! Damage = " + damage);
        if (shields > 0) {
            newDamage = damage - shields;
        }
        if (newDamage >= 1)
        {
            protag.SetTrigger("Hurt");
            Debug.Log("Hurt! Damage animation.");
        }
        else if (newDamage <= 0)
        {
            protag.SetTrigger("Shielded");
        } 

        if (newDamage >= 0) {
            shields = 0;
            previousHealth = playerHealth;
            playerHealth = playerHealth - newDamage;
            roundedHealth = Mathf.FloorToInt(playerHealth / 10);
            roundedPreviousHealth = Mathf.FloorToInt(previousHealth / 10);
            healthBar.fillAmount = (float)playerHealth / (float)playerHealthMax;
        }
        else
        {
            shields -= damage;
        }
        if (healthText == null) {

        }
        healthText.text = playerHealth.ToString();
        if (playerHealth <= 0)
        {
            protag.SetTrigger("CombatLost");
            StartCoroutine(BCI.nm.bt.EndOfBattle(false));
        }
        if (shieldBar == null)
            shieldBar = GameObject.FindGameObjectWithTag("HealthBar").transform.Find("HealthBar Shield").GetComponent<Image>();
        shieldBar.fillAmount = (float)shields / (float)playerHealthMax;
    }

    public void HealDamage(int addShields) // used to heal damage, takes amount of healing as an argument
    {
        healIcon.SetActive(true);
        Debug.Log("gaveShields");
        if ((playerHealth + addShields) >= playerHealthMax) {
            addShields = (playerHealthMax - playerHealth);
            shields = addShields;
        }
        else {
            shields += addShields;
            if (playerHealth + shields >= playerHealthMax) {
                shields = (playerHealthMax - playerHealth);
            }
        }
        previousHealth = playerHealth;
        //playerHealth = playerHealth + addShields;
        roundedHealth = Mathf.FloorToInt(playerHealth / 10);
        roundedPreviousHealth = Mathf.FloorToInt(previousHealth / 10);

        if (healthBar == null)
            healthBar = GameObject.FindGameObjectWithTag("HealthBar").transform.Find("HealthBar Fill").GetComponent<Image>();
        healthBar.fillAmount = (float)playerHealth / (float)playerHealthMax;
        healthText.text = playerHealth.ToString();
        shieldBar.fillAmount = (float)shields / (float)playerHealthMax;
    }

    public void healthUpdate() {
        gameController.GetComponent<BattleTransitions>().playerHealth = playerHealth;
    }

    public void BurgerTax(int amount) {
        playerHealth -= amount;
        healthBar.fillAmount = (float)playerHealth / (float)playerHealthMax;
        healthText.text = playerHealth.ToString();
    }
}
