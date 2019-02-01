using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public GameObject battlePrefab;
    public GameObject gameController;
    public float leftBound = -10, rightBound = 10;
    public bool movingLeft = true;
    public bool rotates = false;
    public float moveSpeed = 1;
    public float rotateSpeed = 1;

	void Awake () {
        gameController = GameObject.FindGameObjectWithTag("GameController");
	}
	
	// Update is called once per frame
	void Update () {
        /*if (!gameController.GetComponent<BattleTranistions>().battling && movingLeft && transform.position.x > leftBound)
        {
            transform.Translate(-1 * moveSpeed * 0.1f,0,0);
            if (rotates) {
                transform.Rotate(0, 0, rotateSpeed);
            }
        }
        else if (!gameController.GetComponent<BattleTranistions>().battling && !movingLeft && transform.position.x < rightBound)
        {
            transform.Translate(Vector3.right * moveSpeed * 0.1f);
            if (rotates)                                                                                                                    // I'll figure this out
            {
                transform.Rotate(0, 0, -rotateSpeed);
            }
        }
        else if (!gameController.GetComponent<BattleTranistions>().battling && movingLeft && leftBound >= transform.position.x) {
            movingLeft = false;
        }
        else if (!gameController.GetComponent<BattleTranistions>().battling && !movingLeft && rightBound <= transform.position.x)
        {
            movingLeft = true;
        }*/
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player" && !gameController.GetComponent<BattleTranistions>().battling) {
            StartCoroutine(gameController.GetComponent<BattleTranistions>().StartBattle(this.gameObject));
        }
    }
}
