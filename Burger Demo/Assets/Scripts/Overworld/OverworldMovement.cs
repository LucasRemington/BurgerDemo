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
    public Animator playerAnim;
    public SpriteRenderer playerSprite;

    private BoxCollider2D intTrigger;
    private Vector2 intTriggerBaseOffset;

    public void PseudoStart () {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        playerAnim = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
        intTrigger = GetComponent<BoxCollider2D>();
        intTriggerBaseOffset = intTrigger.offset;
    }
	
	void Update () {

        // If our interaction trigger is null, fetch it. Set the base offset, which we call when we move to flip it.
        if (intTrigger.Equals(null) || intTrigger == null)
        {
            intTrigger = GetComponent<BoxCollider2D>();
            intTriggerBaseOffset = intTrigger.offset;
        }


        if (!gameController.GetComponent<BattleTransitions>().battling) { 
            if (Input.GetKey(KeyCode.RightArrow) && !jumping && canMove) {
                transform.Translate(Vector3.right * moveSpeed / 100);
                playerAnim.SetBool("Walking", true);
                playerSprite.flipX = false;
                intTrigger.offset = intTriggerBaseOffset;
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && !jumping && canMove) {
                transform.Translate(Vector3.left * moveSpeed / 100);
                playerAnim.SetBool("Walking", true);
                playerSprite.flipX = true;
                intTrigger.offset = intTriggerBaseOffset * new Vector2(-1, 1);
            }
            else if (!jumping)
            {
                playerAnim.SetBool("Walking", false);
            }


            /*if (Input.GetKeyDown(KeyCode.UpArrow) && !jumping && canMove) {
                StartCoroutine(Jump());
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && !jumping && transform.position.y > floorHeight && canMove) {
                StartCoroutine(Drop());
            }*/
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
