using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public GameObject battlePrefab;             // i currently have the battle on the enemy, because the battle needs to be instantiated as a whole, each battle will be a modified prefab
    public GameObject gameController;
    public float leftBound = -10, rightBound = 10;
    public bool movingLeft = true;
    public bool rotates = false;
    public bool patrols = false;
    public float moveSpeed = 1;
    public float rotateSpeed = 1;

	void Awake () {
        gameController = GameObject.FindGameObjectWithTag("GameController");
	}
	
	// Update is called once per frame
	void Update () {
        if (patrols)
        {
            if (!gameController.GetComponent<BattleTransitions>().battling && movingLeft && transform.position.x > leftBound)
            {
                GetComponent<Rigidbody2D>().MovePosition(new Vector2(transform.position.x - moveSpeed * 0.1f, transform.position.y));

            }
            else if (!gameController.GetComponent<BattleTransitions>().battling && !movingLeft && transform.position.x < rightBound)
            {
                GetComponent<Rigidbody2D>().MovePosition(new Vector2(transform.position.x + moveSpeed * 0.1f, transform.position.y));

            }
            else if (!gameController.GetComponent<BattleTransitions>().battling && movingLeft && leftBound >= transform.position.x)
            {
                movingLeft = false;
            }
            else if (!gameController.GetComponent<BattleTransitions>().battling && !movingLeft && rightBound <= transform.position.x)
            {
                movingLeft = true;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.name == "OverworldPlayer" && !gameController.GetComponent<BattleTransitions>().battling) {
            StartCoroutine(gameController.GetComponent<BattleTransitions>().StartBattle(this.gameObject));
        }
    }
}
