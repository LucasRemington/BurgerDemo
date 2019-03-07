using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionExit : MonoBehaviour
{
    [Header("Transition")]
    [Tooltip("The target scene that this exit leads to.")] public SceneField sceneToLoad;
    [Tooltip("The index of the target exit to warp to in the chosen scene, as determined by the target scene's TransitionTracker object.")] public int exitIndex;
    [Tooltip("The Transition Manager that can be found under Don'tDestroyOnLoad. There isn't much to say about this. Set via script! Do not touch. Or do. It shouldn't actually matter, and I don't think you -can- in any but the initial scene.")] public TransitionManager transitionManager;
    [Tooltip("Whether or not this is something you need to interact with to cause a room transition, otherwise the player simply walks into the trigger.")] public bool activateToUse;
    [Tooltip("Is the player currently moving between scenes? If so, we can't trigger the warp again.")] public bool warping;

    [Header("Narrative")]
    [Tooltip("Narrative Script 1. I'll try and get this via script.")] public NarrativeManager narrMan;
    [Tooltip("The narrative script wants a specific room index for event scripting. This will be what that gets set to. Idk how it works, ask Lucas or Matt what it needs to be.")] public int narrativeIndex;
    [Tooltip("Whether or not this is scripted movement; do not check this at the same time as 'activate to use.'")] public bool scripted;

    



    // Grab the transition manager and narrative script if it isn't already set.
    void Start ()
    {
        if (transitionManager == null)
        {
            transitionManager = GameObject.FindGameObjectWithTag("Transition Manager").GetComponent<TransitionManager>();
        }

        if (narrMan == null && transitionManager != null)
        {
            narrMan = transitionManager.GetComponentInParent<NarrativeManager>();
        }

        
	}

    // Ew, nested if-statements. We check: Is this the player colliding? If so, is this movement of their own accord and not scripted? If so, then we check by our "Activate to use" boolean and call the manager's warp.
    // If it's a scripted exit, then we don't auto-save.
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Overworld")
        {
            if (other.gameObject.GetComponent<OverworldMovement>().canMove && !scripted)
            {
                if (activateToUse && Input.GetKeyDown(KeyCode.Space) && !warping && this.enabled)
                {
                    warping = true;
                    Debug.Log("Door!");
                    other.gameObject.GetComponent<OverworldMovement>().canMove = false;
                    narrMan.room = narrativeIndex;
                    transitionManager.Warp(exitIndex, sceneToLoad, other.GetComponent<OverworldMovement>().playerSprite.flipX, true);

                }
                else if (!activateToUse && !warping)
                {
                    warping = true;
                    other.gameObject.GetComponent<OverworldMovement>().canMove = false;
                    narrMan.room = narrativeIndex;
                    transitionManager.Warp(exitIndex, sceneToLoad, other.GetComponent<OverworldMovement>().playerSprite.flipX, true);

                }
            }
            else if (!other.gameObject.GetComponent<OverworldMovement>().canMove && scripted && !warping)
            {
                warping = true;
                other.gameObject.GetComponent<OverworldMovement>().canMove = false;
                narrMan.room = narrativeIndex;
                transitionManager.Warp(exitIndex, sceneToLoad, other.GetComponent<OverworldMovement>().playerSprite.flipX, false);
            }
        }

        
    }
}
