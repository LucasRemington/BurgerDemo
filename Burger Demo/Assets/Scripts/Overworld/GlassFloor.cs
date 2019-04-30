using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassFloor : MonoBehaviour
{
    private BoxCollider2D triggerbox; // The box collider trigger attached to this object.
    private EdgeCollider2D edgecoll; // The edge collider attached to this object, which acts as the floor.
    private OverworldMovement playerMove; // The overworld movement script attached to the player within the scene.
    private Animator anim; // The animator attached to this object.
    private SpriteRenderer sprRen; // The sprite renderer attached to the object.
    private GameObject audioSource; // The audio source that sounds should come from!
    public bool isBroken; // Whether or not this floor has already been broken by the player.

    private bool costarted;
    private bool timerdone;
    private bool playercolliding;

    private void Start()
    {
        // Fetch the components attached to this object.
        triggerbox = GetComponent<BoxCollider2D>();
        edgecoll = GetComponent<EdgeCollider2D>();
        anim = GetComponent<Animator>();
        sprRen = GetComponent<SpriteRenderer>();
        // Fetch the audioSource here! //
        triggerbox.isTrigger = true; // Set our triggerbox to be a trigger at start, just in case it isn't already.

        // Flip a coin. If heads, flip the X of the sprite, just to get a little bit of variation.
        if (Random.Range(1, 3) == 1)
        {
            sprRen.flipX = true;
        }
    }

    private void Update()
    {
        if (playerMove == null) // Has our movement script been set yet?
        {
            if (GameObject.FindGameObjectWithTag("Overworld").activeInHierarchy) // Is it currently active in the scene?
                playerMove = GameObject.FindGameObjectWithTag("Overworld").GetComponent<OverworldMovement>(); // Set it.
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Overworld")
        {
            Debug.Log("Player colliding");
            if (!Input.GetKey(KeyCode.DownArrow) && edgecoll.enabled) // While the player is in the triggerbox on this object, if they aren't crouching, then the floor breaks. Do this only if the edge collider is not already disabled to save processing.
            {
                Debug.Log("Player not crouching and collider enabled, break!");
                if (!isBroken) // If the floor hasn't broken yet, break it and play the animation.
                {
                    isBroken = true;
                    anim.SetTrigger("Break");
                    anim.SetBool("IsBroken", true);
                    // Play a sound, here! //
                }

                edgecoll.enabled = false; // Otherwise, turn the edgecollider off either way.
            }
            else if (Input.GetKey(KeyCode.DownArrow) && !edgecoll.enabled) // Similarly, only reenable the edge collider if the player is crouching and it's currently disabled.
            {
                Debug.Log("Player crouching and collider was missing, restore it!");
                edgecoll.enabled = true;
            }
        }
    }


    /*private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Overworld") // Check if this is the player, first.
        {

            if (!costarted)
            {
                StartCoroutine(CheckFloor()); // We'll handle most of our checks through a coroutine. 
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Overworld")
            playercolliding = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Overworld")
            playercolliding = true;
    }

    private IEnumerator CheckFloor() // This handles our floor check; if the player is in our triggerbox but not crouching, the glass breaks. Otherwise they can walk on it.
    {
        costarted = true;
        Debug.Log("Start CheckFloor coroutine for " + gameObject);
        timerdone = false;
        StartCoroutine(BriefTimer());
        yield return new WaitUntil(() => timerdone || !playercolliding); // Wait for either a brief amount of time to pass, or for the player to stop being in our triggerbox.

        if (playercolliding) // If the player has left the bounds of our box, stop doing what we're doing.
        {
            Debug.Log("Player is still colliding at " + gameObject + "!");
            if (!playerMove.crouching && edgecoll.enabled) // While the player is in the triggerbox on this object, if they aren't crouching, then the floor breaks. Do this only if the edge collider is not already disabled to save processing.
            {
                Debug.Log("Player is not crouching, floor should break at " + gameObject);
                if (!isBroken) // If the floor hasn't broken yet, break it and play the animation.
                {
                    isBroken = true;
                    anim.SetTrigger("Break");
                    anim.SetBool("IsBroken", true);
                    // Play a sound, here! //
                }

                edgecoll.enabled = false; // Otherwise, turn the edgecollider off either way.
            }
            else if (playerMove.crouching && !edgecoll.enabled) // Similarly, only reenable the edge collider if the player is crouching and it's currently disabled.
            {
                edgecoll.enabled = true;
            }
        }

        costarted = false;
    }

    private IEnumerator BriefTimer()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Timer done at " + gameObject);
        timerdone = true;
    }*/
}
