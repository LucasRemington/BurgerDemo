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
    private bool delayDone;
    [Tooltip("If this is a timed hazard, how long is its active phase?")] public float activeTime;
    [Tooltip("If this is a timed hazard, how long is its inactive phase?")] public float inactiveTime;

    [Header("Forces")]
    [Tooltip("The amount of damage that this hazard does. Set at or above 100% to be lethal.")] public int damage;
    [Tooltip("How much force is applied to knock the player away from the hazard if it's a knockback hazard?")] public float knockbackForce;

    private void Start()
    {
        StartCoroutine(Timer());
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
            OverworldMovement owMove = other.GetComponent<OverworldMovement>();
            switch (hazardType)
            {
                case HazardType.Pitfall:
                    if (!owMove.invuln)
                    {
                        StartCoroutine(owMove.FloatingDamage(damage));
                        StartCoroutine(owMove.InvulnTimer());
                        owMove.invuln = true;
                        activated = true;
                    }
                    break;
                case HazardType.Slip:
                    if (!owMove.slipping && !owMove.crouching)
                    {
                        if (Input.GetKey(KeyCode.LeftArrow))
                        {
                            owMove.slipRight = false;
                            owMove.slipLeft = true;
                        }
                        else if (Input.GetKey(KeyCode.RightArrow))
                        {
                            owMove.slipRight = true;
                            owMove.slipLeft = false;
                        }
                    }


                    if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) && !owMove.crouching)
                    {
                        owMove.slipping = true;
                        StopCoroutine(owMove.SlipTimer(slipTime));
                        StartCoroutine(owMove.SlipTimer(slipTime));
                        activated = true;
                    }
                       
                    break;
                case HazardType.Knockback:
                    if (!owMove.invuln)
                    {
                        StartCoroutine(owMove.FloatingDamage(damage));
                        StartCoroutine(owMove.InvulnTimer());
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
                            if (owMove.playerSprite.flipX)
                            {
                                other.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.left * knockbackForce, ForceMode2D.Impulse);
                            }
                            else
                                other.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.right * knockbackForce, ForceMode2D.Impulse);
                        }
                        owMove.invuln = true;
                        owMove.canMove = false;
                        owMove.LadderJump();

                        StopCoroutine(KnockbackTimer(owMove));
                        StartCoroutine(KnockbackTimer(owMove));

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

    private IEnumerator KnockbackTimer(OverworldMovement owMove)
    {
        yield return new WaitForSeconds(knockbackTime);
        owMove.canMove = true;
    }
}
