using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMovement : MonoBehaviour {

    public GameObject lastTouched;  // used to interact with things/people
    public float moveSpeed = 10.0f;
    public GameObject gameController;
    public float jumpHeight = 0.3f;
    public float jumpSpeed = 2;
    public float floorHeight; // this is the height of the lowest floor so the player can't jump below that
    public bool jumping = false;
    public bool canMove = true;

    void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!gameController.GetComponent<BattleTranistions>().battling) { 
            if (Input.GetKey(KeyCode.RightArrow) && !jumping && canMove) {
                transform.Translate(Vector3.right * moveSpeed / 100);
            }
            if (Input.GetKey(KeyCode.LeftArrow) && !jumping && canMove) {
                transform.Translate(Vector3.left * moveSpeed / 100);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) && !jumping && canMove) {
                StartCoroutine(Jump());
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && !jumping && transform.position.y > floorHeight && canMove) {
                StartCoroutine(Drop());
            }
            /*if (Input.GetKeyDown(KeyCode.Space) && lastTouched != null && canMove) {
                lastTouched.GetComponent<Interactable>().Interact();                            //this is in the interactable script now
            }*/
        }
    }

    /*private void OnCollisionEnter2D(Collision2D other) {
        lastTouched = other.gameObject;                 
        Debug.Log("Touched " + other.gameObject.name);
    }
                                                                        // these two are also in the interactable script now
    private void OnCollisionExit2D(Collision2D other) {
        if(other.gameObject == lastTouched)
            lastTouched = null;
    }*/
    public IEnumerator Jump() {
        jumping = true;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<CapsuleCollider2D>().isTrigger = true;
        float i = 0;
        while (i <= jumpHeight) {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y + jumpSpeed, transform.position.z), 0.1f); // this is a weird way to do it, but it looks nice
            yield return new WaitForEndOfFrame();
            i = i + Time.deltaTime;
        }
        GetComponent<Rigidbody2D>().gravityScale = 2;
        GetComponent<CapsuleCollider2D>().isTrigger = false;
        yield return new WaitForSeconds(0.1f);
        GetComponent<Rigidbody2D>().gravityScale = 1;
        jumping = false;
    }

    public IEnumerator Drop() {
        jumping = true;
        GetComponent<CapsuleCollider2D>().isTrigger = true;
        GetComponent<Rigidbody2D>().gravityScale = 2;
        yield return new WaitForSeconds(0.5f);
        GetComponent<CapsuleCollider2D>().isTrigger = false;
        GetComponent<Rigidbody2D>().gravityScale = 1;
        jumping = false;
    }
}
