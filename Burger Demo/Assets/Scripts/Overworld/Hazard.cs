using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Hazard : MonoBehaviour
{
    public enum HazardType { Pitfall, Slip, Knockback };
    //public enum DamageType { percent, flat };
    //[Tooltip("The type of damage the player takes; is it flat damage, or a percent of their health?")] public DamageType damageType;

    [Header("Type")]
    [Tooltip("The base type of hazard this specific hazard falls under!")] public HazardType hazardType;

    [Header("Universal")]
    [Tooltip("Is this a one-time hazard or no?")] public bool destroyOnUse;
    [Tooltip("Does this hazard have a period where it turns off on a timer?")] public bool timed;

    [Header("Timers")]
    [Tooltip("How long does it take for the player to be able to move again? Make sure this is less than the player's own invulnerability timer!")] public float knockbackTime;
    [Tooltip("For how long does the player slip for?")] public float slipTime;
    [Tooltip("When the scene starts, it will take this amount of time for the hazard to become active.")] public float delayTime;
    private bool delayDone; // If the initial delay for something to activate has finished.
    [Tooltip("If this is a timed hazard, how long is its active phase?")] public float activeTime;
    [Tooltip("If this is a timed hazard, how long is its inactive phase?")] public float inactiveTime;

    [Header("Forces")]
    [Tooltip("The amount of damage that this hazard does. Set at or above 100% to be lethal.")] public int damage;
    [Tooltip("How much force is applied to knock the player away from the hazard if it's a knockback hazard?")] public float knockbackForce;

    [Header("Hidden Values")]
    private GameObject player; // Overworld player.
    private OverworldMovement ovwMove; // The Overworld Movement script attached to the player.
    private bool playerFalling; // Whether the player is currently falling, used for pitfalls to activate.
    private bool playerHere; // Whether the player has entered the damagebox or not; once they leave, we can disable the colliders.
    private PolygonCollider2D polyColl; // The collider that the player stands on for greasetraps; part of the child.
    private BoxCollider2D triggerbox; // The triggerbox that actually damages the player in the greasetrap; part of the parent.
    private int layer; // The sorting order layer of the greasetrap.
    private int playerLayer; // The sorting layer of the player.

    private void Start()
    {
        StartCoroutine(Timer());

        // At start, if this is a greasetrap, grab the relevant colliders.
        if (hazardType == HazardType.Pitfall)
        {
            triggerbox = gameObject.GetComponent<BoxCollider2D>();
            if (transform.GetChild(0).gameObject.GetComponent<PolygonCollider2D>())
            {
                polyColl = transform.GetChild(0).gameObject.GetComponent<PolygonCollider2D>();
            }
        }

        // Grab the player, either way.
        player = GameObject.FindGameObjectWithTag("Overworld");
        if (player != null)
        {
            ovwMove = player.GetComponent<OverworldMovement>();
            playerLayer = player.GetComponent<SpriteRenderer>().sortingOrder;
        }

        layer = 0; // We initialize layer as not the sorting layer, so that we can use it as a constant rather than have layer itself change.
        layer = GetComponent<SpriteRenderer>().sortingOrder;
    }

    // Update is used to determine if the player is falling for the sake of pitfall traps. If they are, we activate the colliders. Deactivate them if the player is not falling and they've also left the colliders.
    private void Update()
    {
        if (hazardType == HazardType.Pitfall)
        {
            if (!ovwMove.grounded)
                playerFalling = true;
            else
                playerFalling = false;

            if (playerFalling)
            {
                polyColl.enabled = true;
                triggerbox.enabled = true;
                GetComponent<SpriteRenderer>().sortingOrder = layer;
            }
            else if (!playerFalling && !playerHere)
            {
                polyColl.enabled = false;
                triggerbox.enabled = false;
                GetComponent<SpriteRenderer>().sortingOrder = playerLayer - 1;
            }
        }
    }

    private IEnumerator Timer()
    {
        if (!delayDone)
        {
            yield return new WaitForSeconds(delayTime);
            delayDone = true;
        }

        while (timed)
        {
            yield return new WaitForSeconds(activeTime);
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;

            yield return new WaitForSeconds(inactiveTime);
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<Collider2D>().enabled = true;
        }

        yield return null;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Overworld")
        {
            bool activated = false;
            playerHere = true; // When the player collides, they activate this bool to signal such.

            switch (hazardType)
            {
                case HazardType.Pitfall:
                    if (!ovwMove.invuln)
                    {
                        if (GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleTransitions>().playerHealth > 0)
                        {
                            StartCoroutine(ovwMove.FloatingDamage(damage));
                            StartCoroutine(ovwMove.InvulnTimer());
                            ovwMove.invuln = true;
                            activated = true;
                        }                        
                    }
                    break;
                case HazardType.Slip:
                    if (!ovwMove.slipping && !ovwMove.crouching)
                    {
                        if (Input.GetKey(KeyCode.LeftArrow))
                        {
                            ovwMove.slipRight = false;
                            ovwMove.slipLeft = true;
                        }
                        else if (Input.GetKey(KeyCode.RightArrow))
                        {
                            ovwMove.slipRight = true;
                            ovwMove.slipLeft = false;
                        }
                    }


                    if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) && !ovwMove.crouching)
                    {
                        ovwMove.slipping = true;
                        StopCoroutine(ovwMove.SlipTimer(slipTime));
                        StartCoroutine(ovwMove.SlipTimer(slipTime));
                        activated = true;
                    }
                       
                    break;
                case HazardType.Knockback:
                    if (!ovwMove.invuln && GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleTransitions>().playerHealth > 0)
                    {
                        player.GetComponent<Animator>().SetTrigger("Burned");
                        StartCoroutine(ovwMove.FloatingDamage(damage));
                        StartCoroutine(ovwMove.InvulnTimer());
                        Vector2 direction = other.gameObject.transform.position - gameObject.transform.position;

                        if (direction.x > 0)
                        {
                            other.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.right * knockbackForce, ForceMode2D.Impulse);
                        }
                        else if (direction.x < 0)
                        {
                            other.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.left * knockbackForce, ForceMode2D.Impulse);
                        }
                        else
                        {
                            if (ovwMove.playerSprite.flipX)
                            {
                                other.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.left * knockbackForce, ForceMode2D.Impulse);
                            }
                            else
                                other.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.right * knockbackForce, ForceMode2D.Impulse);
                        }
                        ovwMove.invuln = true;
                        ovwMove.canMove = false;
                        ovwMove.LadderJump();

                        StopCoroutine(KnockbackTimer(ovwMove));
                        StartCoroutine(KnockbackTimer(ovwMove));

                        activated = true;
                    }

                    break;
            }

            if (destroyOnUse && activated)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // When the player exits our collider, deactivate the relevant bool. Mostly just used for greasetraps.
        if (other.gameObject.tag == "Overworld")
        {
            playerHere = false;
        }
    }

    private IEnumerator KnockbackTimer(OverworldMovement owMove)
    {
        yield return new WaitForSeconds(knockbackTime);
        owMove.canMove = true;
    }
}
