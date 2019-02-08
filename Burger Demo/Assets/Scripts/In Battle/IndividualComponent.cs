using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndividualComponent : MonoBehaviour {

    public GameObject burgerSpawner; //This and BCI reference the static script
    public BurgerComponentInstantiator BCI;
    public Vector3 restingPosition; //initial position for bounce
    public Vector3 bouncingPosition;
    int prevTrigger; //all variables used to trigger the global bounce
    float ticks = 0;
    bool tickDown;
    public bool dontDespawn; //publically checked for items that bounce but don't destroy themselves
    private Image sprite;
    public Animator anim;

    public bool isTopBun;

    void Awake () //sets variables for other scripts, mostly
    {
        burgerSpawner = GameObject.Find("BurgerSpawner");
        BCI = burgerSpawner.GetComponent<BurgerComponentInstantiator>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<Image>();
        this.transform.SetParent(BCI.CombatUI.transform, false);
        restingPosition = new Vector3(transform.position.x, transform.position.y, 0);
        bouncingPosition = new Vector3(transform.position.x, transform.position.y - 0.1f, 0);
        prevTrigger = BCI.bounceTrigger;
        StartCoroutine(Bounce());
        if (dontDespawn == false)
        {
            StartCoroutine(Despawn());
        }
    }

    public void layerSort ()
    {
        return;
    }

    public void componentStatic() //called when the static animation begins
    {
        BCI.canSpawn = true;
    }

    IEnumerator Despawn() //called when BCI reaches the component cap or space is pressed. This will eventually trigger the 'end of attack' animations
    {
        yield return new WaitUntil(() => BCI.spawnReset == true);
        anim.SetTrigger("Vanish");
    }

    public void KillThis() //called at end of vanish with event
    {
        Destroy(this.gameObject);
    }

    //all below functions are used for bouncing
    public void triggerBounce () //this passes the individual bounce to the global script, which is recognized by all bouncy objects in the scene...
    {
        BCI.bounceTrigger++;
    }

    IEnumerator Bounce() //...with this coroutine, checking if that global number is ever changed.
    {
        yield return new WaitUntil(() => BCI.bounceTrigger != prevTrigger);
        StartCoroutine(BounceTicks());
        prevTrigger = BCI.bounceTrigger;
    }

    IEnumerator BounceTicks () //This is an inefficent way to lerp, but it's better(?) than an update function
    {
        yield return new WaitForSeconds(0.01f);
        if (ticks != 5 && tickDown == false)
        {
            ticks++;
            transform.position = Vector3.Lerp(restingPosition, bouncingPosition, (0.2f * ticks));
            StartCoroutine(BounceTicks());
        } else if (ticks == 5 || tickDown == true && ticks > 0)
        {
            tickDown = true;
            ticks--;
            transform.position = Vector3.Lerp(restingPosition, bouncingPosition, (0.2f * ticks));
            StartCoroutine(BounceTicks());
        } else
        {
            StartCoroutine(Bounce());
            tickDown = false;
            ticks = 0;
        }
        
    }
}
