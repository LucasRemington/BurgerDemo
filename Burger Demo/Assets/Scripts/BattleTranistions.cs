using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTranistions : MonoBehaviour {

    public GameObject battlePrefab;
    public GameObject battle;
    public int playerHealth = 100;
    public GameObject player;
    public PlayerHealth ph;
    public GameObject[] OverworldObjects;
    public GameObject currentEnemy;

    public bool battling = false;
	// Use this for initialization
	void Start () {
        OverworldObjects = GameObject.FindGameObjectsWithTag("Overworld");
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Delete))
            Destroy(battle);
	}

    public IEnumerator StartBattle(GameObject enemy) {
        currentEnemy = enemy;
        Instantiate(enemy.GetComponent<Enemy>().battlePrefab, new Vector3(0,-6,0), new Quaternion(0,0,0,0),this.transform);
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < OverworldObjects.Length; i++) {
            OverworldObjects[i].SetActive(false);
        }
        //Instantiate(battlePrefab, this.gameObject.transform);
        battle = GameObject.Find("FullBattlePrefab(Clone)");
        battle.transform.parent = null;
        player = GameObject.Find("Player");
        ph = player.GetComponent<PlayerHealth>();
        //Instantiate(enemy.GetComponent<Enemy>().battlePrefab, this.transform);
        battling = true;
        yield return new WaitUntil(() => battle == null);
        
    }

    public IEnumerator EndOfBattle()            //this gets called by the enemy's death in enemyBehavior
    {
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < OverworldObjects.Length; i++)
        {
            OverworldObjects[i].SetActive(true);
        }
        yield return new WaitForSeconds(1);
        ph.healthUpdate();
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        Destroy(battle);
        Destroy(currentEnemy.gameObject);
        battling = false;
        currentEnemy = null;
    }
}
