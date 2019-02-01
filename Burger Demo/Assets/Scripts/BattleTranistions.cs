using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTranistions : MonoBehaviour {

    public GameObject battlePrefab;
    public GameObject battle;
    public int playerHealth = 100;
    public GameObject player;
    public PlayerHealth ph;

    public bool battling = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Delete))
            Destroy(battle);
	}

    public IEnumerator StartBattle(GameObject enemy) {
        Instantiate(enemy.GetComponent<Enemy>().battlePrefab, new Vector3(0,-6,0), new Quaternion(0,0,0,0),this.transform);
        //Instantiate(battlePrefab, this.gameObject.transform);
        battle = GameObject.Find("FullBattlePrefab(Clone)");
        battle.transform.parent = null;
        player = GameObject.Find("Player");
        ph = player.GetComponent<PlayerHealth>();
        //Instantiate(enemy.GetComponent<Enemy>().battlePrefab, this.transform);
        battling = true;
        yield return new WaitUntil(() => battle == null);
        Destroy(enemy.gameObject);
        battling = false;
    }

    public IEnumerator EndOfBattle()
    {
        yield return new WaitForSeconds(3);
        ph.healthUpdate();
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        Destroy(battle);
    }
}
