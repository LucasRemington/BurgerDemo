﻿using System.Collections;
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
    public GameObject enemyStart;

    public bool battling = false;

    public GameObject MainCamera;
    public NarrativeManager nm;

    // Use this for initialization
    void Start () {
        OverworldObjects = GameObject.FindGameObjectsWithTag("Overworld");
        DontDestroyOnLoad(this);
        MainCamera = GameObject.FindWithTag("MainCamera");
        nm = MainCamera.GetComponent<NarrativeManager>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Delete))
            Destroy(battle);
	}

    public IEnumerator StartBattle(GameObject enemy) {
        currentEnemy = enemy;
        battling = true;
        yield return new WaitForSeconds(0.1f);
        battlePrefab.SetActive(true);
        battle = Instantiate(enemy.GetComponent<Enemy>().battlePrefab, enemyStart.transform.position, new Quaternion(0,0,0,0),this.transform);
        for (int i = 0; i < OverworldObjects.Length; i++) {
            OverworldObjects[i].SetActive(false);
        }
        yield return new WaitForSeconds(0.1f);
        //Instantiate(battlePrefab, this.gameObject.transform);
        //battle = GameObject.Find("FullBattlePrefab(Clone)");
        battle.transform.parent = null;
        
        player = GameObject.Find("FullBattlePrefab").transform.GetChild(0).gameObject;
        ph = player.GetComponent<PlayerHealth>();
        //Instantiate(enemy.GetComponent<Enemy>().battlePrefab, this.transform);
        battling = true;
    }

    public IEnumerator EndOfBattle()            //this gets called by the enemy's death in enemyBehavior
    {
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < OverworldObjects.Length; i++) {
            OverworldObjects[i].SetActive(true);
        }
        yield return new WaitForSeconds(1);
        //ph.healthUpdate();
        player = GameObject.Find("FullBattlePrefab").transform.GetChild(0).gameObject;
        ph = player.GetComponent<PlayerHealth>();
        playerHealth = ph.playerHealth;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        Destroy(battle.gameObject);
        Destroy(currentEnemy.gameObject);
        GameObject thing = GameObject.Find("FullBattlePrefab");
        thing.SetActive(false);
        thing = null;
        battling = false;
        nm.combatUI.SetActive(false);
        currentEnemy = null;
    }
}
