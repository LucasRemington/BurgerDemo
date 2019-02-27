﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class OverworldMovement : MonoBehaviour {

    [Header("Managers and Objects")]
    [Tooltip("The GameController gameobject within the scene. Set this in the inspector!")] public GameObject gameController;
    [HideInInspector] [Tooltip("For being invincible after taking overworld damage.")] public bool invuln;
    [Tooltip("For how long does the player have i-frames after taking overworld damage?")] public float invulnTime;

    [Header("Interaction")]
    [Tooltip("Used to interact with objects.")] public GameObject lastTouched;
    [Tooltip("The interaction triggerbox on the player, set via script.")] private BoxCollider2D intTrigger;
    [Tooltip("The initial offset of the interaction triggerbox.")] private Vector2 intTriggerBaseOffset;

    [Header("Basic Movement")]
    [Tooltip("Player move speed.")] public float moveSpeed = 10.0f;
    [Tooltip("Player ladder-climb speed.")] public float climbSpeed = 3.0f;
    [Tooltip("How much to divide move speed by when crouching.")] public float crouchSpeedDivider;
    [Tooltip("Can the player currently move? Used for the sake of cutscenes and if the player falls from a high place.")] public bool canMove = true;
    [Tooltip("Whether or not the player is slipping and can't control their movement.")] [HideInInspector] public bool slipping;
    [HideInInspector] public bool slipLeft;
    [HideInInspector] public bool slipRight;
    [Tooltip("Is the player crouching? Do I need to explain this?")] public bool crouching;
    private float tempMoveSpeed;

    private CapsuleCollider2D playColl;
    private Vector2 baseCollSize;
    private Vector2 baseCollOffset;
    [Tooltip("The X and Y dimensions for the capsule collider's size when crouching.")] public Vector2 crouchCollSize;
    [Tooltip("The X and Y dimensions for the capsule collider's offset when crouching.")] public Vector2 crouchCollOffset;

    // [Header("Jumping")]
    //public float jumpHeight = 0.3f;
    // public float jumpSpeed = 2;
    //public float floorHeight; // this is the height of the lowest floor so the player can't jump below that
    //public bool jumping = false;

    [Header("Animation and Sprites")]
    [Tooltip("The animation component on the player. Set via script.")] public Animator playerAnim;
    [Tooltip("The spriterenderer component on the player. Set via script.")] public SpriteRenderer playerSprite;

    // Used in climbing stairs or ladders. Later on we can add a bool that determines if something uses a ladder climb anim or a stair climb anim. That also might just be coded differently here.
    [Header("Ladders and Climbing")]
    [Tooltip("Whether the player is currently locked into climbing a ladder or not.")] public bool onLadder;
    [Tooltip("Keeps track of if the player is within the bounds of a ladder's triggerbox.")] private bool inLadderHitBox;
    [Tooltip("This list is used to enable and disable colliders of any ladders you touch.")] private List<GameObject> ladder = new List<GameObject>(1);
    [Tooltip("A small tracker int that keeps track of the index of the ladder the player is currently holding in the aforementioned list.")] private int x = 0;
    [Tooltip("When the player grabs onto a ladder, this takes the ladder's position at the start so that it can be lerped to.")] private float tempXPos;
    [Tooltip("When the player grabs onto a ladder from the top, this takes the player's y position at the start so that it can be lerped.")] private float tempYPos;
    [Tooltip("When the player grabs onto a ladder from the top, this bool prevents the player from moving and slides them into position smoothly with a lerp.")] public bool topDownGrabbing;
    [Tooltip("Whether or not the player is allowed to grab the ladder below them.")] private bool canGrabDown;
    [Tooltip("If the player is currently inside the hitbox for the topmost part of a ladder.")] public bool touchingLadderCap;

    private float tempTimerMax = 0.25f;

    // Fally girl.
    [Header("Falling and Fall Damage")]
    [Tooltip("Whether the player is currently touching the ground or not. Turns false after a very brief moment of not touching the ground.")] public bool grounded;
    [Tooltip("Health of the player. Connects to the gameController's BattleTransition script.")] private int health;
    //[Tooltip("How long the player should pause on the ground after falling before getting up and being able to move again.")] public float getUpTime;
    [Tooltip("Starting position when the player starts to fall.")] private Vector2 startPos;
    [Tooltip("End position when the player finishes falling.")] private Vector2 endPos;
    [Tooltip("The Text component of the player's world-space child canvas. Displays fall and overworld damage.")] private Text damText;
    [Tooltip("How long should the floating text for damage taken in the overworld last for?")] public float flDamageTextTime;
    [Tooltip("The amount of health that the player loses after falling. Could theoretically be used for hazards...")] private int lostHealth;
    [Tooltip("The distance at which fall damage starts. The player will still play a landing animation past two blocks regardless, but likely won't take damage, depending.")] public float damStartDist = 3;
    [Tooltip("What percent of health should the player lose upon falling initially?")] public float fallDamPercent = 5;

    public enum DamageType { LinearDamage, ExponentialDamage, FibonacciDamage };
    [Tooltip("The type of damage the player takes when falling. Linear = Damage * Distance (5, 10, 15, 20, 25, etc);\n Expo = Doubles each block fallen (5, 10, 20, 40, etc);\n Fibonacci: Damage is equal to the past two values added to each other (5, 5, 10, 15, 25, etc);")] public DamageType damageType;

    private bool landingAnim;

    public void PseudoStart () {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        playerAnim = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();

        intTrigger = GetComponentInChildren<BoxCollider2D>();
        intTriggerBaseOffset = intTrigger.offset;

        health = gameController.GetComponent<BattleTransitions>().playerHealth;
        damText = GetComponentInChildren<Canvas>().GetComponentInChildren<Text>();

        tempMoveSpeed = moveSpeed;
        playColl = GetComponent<CapsuleCollider2D>();
        baseCollOffset = playColl.offset;
        baseCollSize = playColl.size;

        ladder.Clear();
    }
	
	void Update () {

        // If our interaction trigger is null, fetch it. Set the base offset, which we call when we move to flip it.
        if (intTrigger.Equals(null) || intTrigger == null)
        {
            intTrigger = GetComponentInChildren<BoxCollider2D>();
            intTriggerBaseOffset = intTrigger.offset;
        }

        // Previously used !jumping rather than !onLadder. We walk!
        if (!gameController.GetComponent<BattleTransitions>().battling) { 
            if (Input.GetKey(KeyCode.RightArrow) && !onLadder && canMove && grounded) {
                transform.Translate(Vector3.right * moveSpeed / 100);
                playerAnim.speed = 1f;
                playerAnim.SetBool("Walking", true);
                playerSprite.flipX = false;
                intTrigger.offset = intTriggerBaseOffset;
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && !onLadder && canMove && grounded) {
                transform.Translate(Vector3.left * moveSpeed / 100);
                playerAnim.speed = 1f;
                playerAnim.SetBool("Walking", true);
                playerSprite.flipX = true;
                intTrigger.offset = intTriggerBaseOffset * new Vector2(-1, 1);
            }
            else if (!onLadder && !grounded)
            {
                playerAnim.SetBool("Walking", false);
                playerAnim.SetBool("Climbing", false);
                
            }
            else if (!onLadder)
            {
                playerAnim.SetBool("Walking", false);
                playerAnim.SetBool("Climbing", false);
                playerAnim.SetBool("Falling", false);
            }

            // Crouch? Crouch. But we need to be able to move. And not be on a ladder. And need to be grounded. Plus a couple other things but those are mostly standard. We'll determine the crouching status first, and then if we are crouching, apply proper affects.
            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (canMove && !onLadder && grounded && !topDownGrabbing)
                {
                    crouching = true;
                }
            }
            if (!Input.GetKey(KeyCode.DownArrow) || onLadder || !canMove || !grounded || topDownGrabbing)
            {
                crouching = false;
            }


            // Here's what actually happens when we crouch! We cut our movespeed a bit, and adjust the player collider. Lucas, you can set some animation triggers here as need be.
            if (crouching)
            {
                moveSpeed = tempMoveSpeed / crouchSpeedDivider;
                playColl.size = crouchCollSize;
                playColl.offset = crouchCollOffset;
                GetComponent<Animator>().SetBool("Crouching",true);//anim here
            }

            if (!crouching)
            {
                moveSpeed = tempMoveSpeed;
                playColl.size = baseCollSize;
                playColl.offset = baseCollOffset;
                GetComponent<Animator>().SetBool("Crouching", false);//anim here
            }


            // While on the ladder we just make sure we keep climbing and gravity remains off! We also lerp to the center of the ladder to make it a smoother transition.
            if (onLadder)
            {
                GetComponent<Rigidbody2D>().gravityScale = 0;
                playerAnim.SetBool("Climbing", true);
                playerAnim.SetBool("Falling", false);
                LadderClimb();

                transform.position = Vector2.Lerp(transform.position, new Vector2(tempXPos, transform.position.y), 0.3f);
            }

            

            // If we grab the uppermost ladder from the top, we do a short translate until our y position matches the first tile down, essentially.
            if (topDownGrabbing)
            {
                transform.position = Vector2.Lerp(transform.position, new Vector2(tempXPos, tempYPos - 0.75f), 0.15f);
            }

            // If we leave the uppermost ladder or jump off ourselves, disconnect from the ladder.
            if ((Input.GetKeyDown(KeyCode.Space) && onLadder) || (onLadder && !inLadderHitBox && !topDownGrabbing))
            {
                playerAnim.speed = 1f;
                playerAnim.SetBool("Climbing", false);
                LadderJump();
            }



            // If the player is slipping due to a hazard, we translate the player in the direction they last moved.
            if (slipping)
            {
                if (slipLeft)
                {
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        transform.Translate(Vector3.left * moveSpeed / 150);
                    }
                    else
                        transform.Translate(Vector3.left * moveSpeed / 75);
                }
                else if (slipRight)
                {
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        transform.Translate(Vector3.right * moveSpeed / 150);
                    }
                    else
                        transform.Translate(Vector3.right * moveSpeed / 75);
                }
            }



            /*
            // Set our starting position for fall damage whenever we are considered to be grounded.
            if (grounded)
                startPos = gameObject.transform.position;*/



            /*if (Input.GetKeyDown(KeyCode.UpArrow) && !jumping && canMove) {
                StartCoroutine(Jump());
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && !jumping && transform.position.y > floorHeight && canMove) {
                StartCoroutine(Drop());
            }*/
            /*if (Input.GetKeyDown(KeyCode.Space) && lastTouched != null && canMove) {
                lastTouched.GetComponent<Interactable>().Interact();                            //this is in the interactable script now
            }
            */
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "LadderCap")
            touchingLadderCap = true;
    }

    // Ladder stuff! If we're overlapping a ladder and hit up or down, we grab it! Translate onto its center, disable gravity, and begin climbing! We also disable the "floor" of the ladder, too.
    // We keep a list of all the ladders that the player has been on, so that if multiple are stacked, the edge colliders for each of them are reenabled after leaving them.
    private void OnTriggerStay2D(Collider2D other)
    {
        // While inside a ladder trigger, we are considered inside a ladder hitbox. If we hold up or down, we get on the ladder.
        // While on the ladder, we check our list; if this ladder isn't on it, add it.
        // Create a for loop; we keep an integer x, which gets set to the index of the ladder we're currently on.
        if (other.gameObject.tag == "Ladder")
            inLadderHitBox = true;


        // If we start at the top of a ladder, we want to essentially drop to player height beneath the ladder's end. We grap a temp value for our y, and start a timer until we can move again. We add the relevant ladder to our ladder list.
        // We can only do this if we are not touching the floor!
        if (other.gameObject.tag == "LadderCap" && Input.GetKeyDown(KeyCode.DownArrow) && !onLadder && !topDownGrabbing && canGrabDown)
        {
            tempYPos = gameObject.transform.position.y;
            tempXPos = other.transform.position.x;

            StopCoroutine(TopDownTimer());
            StartCoroutine(TopDownTimer());

            GetComponent<Rigidbody2D>().gravityScale = 0;
            topDownGrabbing = true;

            if (!ladder.Contains(other.transform.parent.gameObject))
            {
                ladder.Add(other.transform.parent.gameObject);
            }

            for (int i = 0; i < ladder.Count; i++)
            {
                if (ladder[i] == other.transform.parent.gameObject)
                {
                    x = i;
                }

            }
            onLadder = true;
            other.transform.parent.gameObject.GetComponent<EdgeCollider2D>().enabled = false;
        }

        // If this is the uppermost ladder in a stack of ladders, and we hit its cap, then we just climb up and "surface"
        if (other.gameObject.tag == "LadderCap" && Input.GetKey(KeyCode.UpArrow) && onLadder)
        {
            transform.position = new Vector2(other.transform.position.x, other.transform.position.y + other.offset.y);
            LadderJump();
        }



        // To explain a few of these checks: The other object needs to be tagged ladder, but can't be its edge collider! We also need to be able to move. If we are already on a ladder, this allows us to add ladders in a chain to our aforementioned list. Otherwise we need to be holding either up or down.
        if (((other.gameObject.tag == "Ladder" && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))) || (other.gameObject.tag == "Ladder" && onLadder == true)) && canMove && other.GetType() != typeof(EdgeCollider2D) && !touchingLadderCap)
        {
            onLadder = true;

            if (!ladder.Contains(other.gameObject))
            {
                ladder.Add(other.gameObject);
            }

            for (int i = 0; i < ladder.Count; i++)
            {
                if (ladder[i] == other.gameObject)
                {
                    x = i;
                }

            }
            
            // No matter what ladder we're on, disable gravity and their edge collider while warping the player to the center of the ladder.
            other.gameObject.GetComponent<EdgeCollider2D>().enabled = false;
            GetComponent<Rigidbody2D>().gravityScale = 0;

            //transform.position = new Vector2(other.transform.position.x, transform.position.y);
            tempXPos = other.transform.position.x;
        }
    }

    // A simple timer that forces the player to wait before they can move after starting to climb down from the top of a ladder.
    private IEnumerator TopDownTimer()
    {
        gameObject.GetComponent<CapsuleCollider2D>().isTrigger = true;
        yield return new WaitForSeconds(tempTimerMax);
        gameObject.GetComponent<CapsuleCollider2D>().isTrigger = false;
        topDownGrabbing = false;
    }

    // When we exit a ladder trigger: if the ladder we were on shares the x index we set previously, we are no longer on a ladder; however, if we are still on another ladder trigger, that'll become x's index instead, so this won't happen!
    // This basically prevents us from falling off the ladder every floor.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ladder" && ladder.Count > 0 && ladder[x] == other.gameObject)
            inLadderHitBox = false;

        if (other.gameObject.tag == "LadderCap")
            touchingLadderCap= false;
    }

    

    // Check floor collision! We'll set the grounded check here, but for now it gets us off the ladder if we're touching the ground.
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Floor" && onLadder)
        {
            LadderJump();
        }

        // Secondary function: if we were not grounded and then touch the floor, end position is now here and we calculate fall damage.
        if (!grounded && (other.gameObject.tag == "Floor" || other.gameObject.tag == "Ladder"))
        {
            endPos = transform.position;
            FallOwchies(startPos, endPos);
        }

        // Woah, tertiary function! If we touch the floor, we can no longer grab the top of a ladder by pressing down.
        if (other.gameObject.tag == "Floor")
        {
            canGrabDown = false;
        }

        if (other.gameObject.tag == "Floor" || other.gameObject.tag == "Ladder")
            StopCoroutine(GroundTimer());
    }

    // If we're holding onto the ladder and we touch the floor while also holding the down key, then we remove ourselves.
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Floor" && onLadder && Input.GetKeyDown(KeyCode.DownArrow))
        {
            LadderJump();
        }

        // Set grounded to true if we are colliding with the floor. Set gravity to normal.
        if ((other.gameObject.tag == "Floor" || other.gameObject.tag == "Ladder" ) && !onLadder)
        {
            GetComponent<Rigidbody2D>().gravityScale = 1;
            grounded = true;
            //playerAnim.SetBool("Falling", false);
            StopCoroutine(GroundTimer());
        }

        
    }

    // If we leave the floor, start a coroutine that passes a miniscule amount of time before we're considered grounded.
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Floor" || other.gameObject.tag == "Ladder")
        {
            StopCoroutine(GroundTimer());
            StartCoroutine(GroundTimer());
            //Debug.Log("Fell!");
        }

        // Secondary function! If we leave the floor, we can now grab the top of a ladder by pressing down.
        if (other.gameObject.tag == "Floor")
        {
            canGrabDown = true;
        }
    }

    // Climby girl.
    private void LadderClimb()
    {
        if (Input.GetKey(KeyCode.UpArrow) && canMove && !topDownGrabbing)
        {
            transform.Translate(Vector2.up * climbSpeed / 100);
            playerAnim.speed = 1f;
            playerAnim.SetBool("ClimbingUp", true);
        }
        else if (Input.GetKey(KeyCode.DownArrow) && canMove && !topDownGrabbing)
        {
            transform.Translate(Vector2.down * climbSpeed / 100);
            playerAnim.speed = 1f;
            playerAnim.SetBool("ClimbingUp", false);
        } else
        {
            playerAnim.speed = 0f;
        }
    }

    // The method called to remove self from the ladder. Reenable the edge colliders on all the ladders touched, and that x value. We begin the ground check timer to make us fall to the floor.
    public void LadderJump()
    {
        onLadder = false;
        
        for (int i = 0; i < ladder.Count; i++)
        {
            ladder[i].GetComponent<EdgeCollider2D>().enabled = true;
            x = 0;
            
        }
        GetComponent<Rigidbody2D>().gravityScale = 1;
        StartCoroutine(GroundTimer());
    }

    // This coroutine is just for the grounded check; after this time passes and we haven't connected with the ground again, we are no longer grounded.
    private IEnumerator GroundTimer()
    {
        startPos = gameController.transform.position;
        GetComponent<Rigidbody2D>().gravityScale = 5;
        yield return new WaitForSeconds(0.05f);
        grounded = false;
        startPos = transform.position;
        yield return new WaitForSeconds(0.05f);
        if (!grounded)
        {
            if (playerAnim.GetBool("Climbing") == false)
            {
                playerAnim.SetBool("Falling", true);
            }
        }
        
    }


    // Calculate fall distance! We take our start position and end position as inputs, and if distance is great enough, take fall damage and create a small waiting window until the player can move again. Distance is measured in half blocks.
    private void FallOwchies(Vector2 startPos, Vector2 endPos)
    {
        // Finding distance is easy, but we wanna round(sorta) to half-units. So we mod distance by one to get the decimal remainder, and round it ourselves manually. Shit sucks.
        float distance = startPos.y - endPos.y;
        float deciDistance = distance % 1;

        if (deciDistance < 0.3f)
            distance = Mathf.Floor(distance);
        else if (deciDistance >= 0.7f)
            distance = Mathf.Ceil(distance);
        else
            distance = Mathf.Floor(distance) + 0.5f;

        // At a distance of 2 or greater, we ""take fall damage,"" even if it's 0.
        if (distance >= 2)
        {
            if (damageType != DamageType.LinearDamage)
                distance = Mathf.Round(distance);

            FallDamage(distance - damStartDist + 1);
        }
            

    }

    // Now we actually do what the above thing was meant to do since in hindsight is just gets distance. I could probably combine the scripts tbh. But eh, it works.
    private void FallDamage(float dist)
    {
        //Debug.Log("Distance: " + dist);


        float damage = 0;
        if (dist > 0)
        {
            switch (damageType)
            {
                case DamageType.LinearDamage:
                    damage = dist * fallDamPercent;
                    break;
                case DamageType.ExponentialDamage:
                    damage = fallDamPercent * Mathf.Pow(2, dist - 1);
                    break;
                case DamageType.FibonacciDamage:
                    float a = fallDamPercent;
                    float b = fallDamPercent;
                    for (int i = 0; i < dist; i++)
                    {
                        float temp = a;
                        a = b;
                        b = temp + b;
                    }
                    damage = a;
                    break;
            }

            //Debug.Log("Fall damage = " + damage + "%");
        }
        playerAnim.SetInteger("Damage", Mathf.RoundToInt(damage));
        int trueDamage = (int)((damage / gameController.GetComponent<BattleTransitions>().ph.playerHealthMax) * 100);
        StartCoroutine(FloatingDamage(trueDamage));
        StopCoroutine(GetUpWait());
        StartCoroutine(GetUpWait());

    }

    // This controls floating text and the damage the player takes from falling and hazards. If no damage is taken, white text says much. Otherwise it's red. Play a special message upon death.
    public IEnumerator FloatingDamage(int damage)
    {
        Color tempColor = damText.color;
        
        health -= damage;


        float tempVal = -1;
        if (health > 0)
            damText.text = ("-" + damage);
        else
            tempVal = Random.Range(0, 10);

        if (tempVal >= 0 && tempVal < 2)
            damText.text = "THIS REALLY HURTS";
        if (tempVal >= 2 && tempVal < 4)
            damText.text = "OW";
        if (tempVal >= 4 && tempVal < 6)
            damText.text = ". . .";
        if (tempVal >= 6 && tempVal < 8)
            damText.text = "CAN YOU NOT";
        if (tempVal >= 8 && tempVal <= 10)
            damText.text = "DEAR GOD WHY";

        if (damage == 0)
        {
            damText.color = Color.white;
        }

        damText.enabled = true;
        
        Debug.Log("Health: " + health);
        
        yield return new WaitForSeconds(flDamageTextTime);
        damText.enabled = false;
        damText.color = tempColor;

        if (health <= 0)
            OverworldDeath();
    }

    // How long does the player need to wait before recovering from falling?
    private IEnumerator GetUpWait()
    {
        canMove = false;
        yield return new WaitUntil(() => landingAnim == true);
        yield return new WaitForSeconds(0.3f);
        landingAnim = false;
        canMove = true;
    }

    // set from animation event
    public void landingFlag ()
    {
        landingAnim = true;
    }

    private void OverworldDeath()
    {
        Destroy(gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // I-Frame timer!
    public IEnumerator InvulnTimer()
    {
        playerSprite.color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(invulnTime);
        playerSprite.color = new Color(1, 1, 1, 1);
        invuln = false;
    }

    // Called from Hazard.cs to dictate how long the player loses control from slipping for.
    public IEnumerator SlipTimer(float time)
    {
        yield return new WaitForSeconds(time);
        slipping = false;
    }

        // ######## wOAH OLD CODe ######### //

        /*private void OnCollisionEnter2D(Collision2D other) {
            lastTouched = other.gameObject;                 
            Debug.Log("Touched " + other.gameObject.name);
        }
                                                                            // these two are also in the interactable script now
        private void OnCollisionExit2D(Collision2D other) {
            if(other.gameObject == lastTouched)
                lastTouched = null;
        }*/

        // Say hello to the old jumping script!
        /*public IEnumerator Jump() {
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
        }*/
    
}