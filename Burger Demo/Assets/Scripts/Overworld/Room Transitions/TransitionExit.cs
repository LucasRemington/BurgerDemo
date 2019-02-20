using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionExit : MonoBehaviour
{
    [Tooltip("The target scene that this exit leads to.")] public SceneField sceneToLoad;
    [Tooltip("The index of the target exit to warp to in the chosen scene, as determined by the target scene's TransitionTracker object.")] public int exitIndex;
    [Tooltip("The Transition Manager that can be found under Don'tDestroyOnLoad. There isn't much to say about this. Set via script! Do not touch. Or do. It shouldn't actually matter, and I don't think you -can- in any but the initial scene.")] public TransitionManager transitionManager;
    [Tooltip("Whether or not this is something you need to interact with to cause a room transition, otherwise the player simply walks into the trigger.")] public bool activateToUse;



    // Grab the transition manager if it isn't already set.
	void Start ()
    {
        if (transitionManager == null)
        {
            transitionManager = GameObject.FindGameObjectWithTag("Transition Manager").GetComponent<TransitionManager>();
        }

        
	}

    // Ew, nested if-statements. We check: Is this the player colliding? If so, is this movement of their own accord and not scripted? If so, then we check by our "Activate to use" boolean and call the manager's warp.
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Overworld")
        {
            if (other.gameObject.GetComponent<OverworldMovement>().canMove)
            {
                if (activateToUse && Input.GetKeyDown(KeyCode.Space))
                {
                    other.gameObject.GetComponent<OverworldMovement>().canMove = false;
                    transitionManager.Warp(exitIndex, sceneToLoad, other.GetComponent<OverworldMovement>().playerSprite.flipX);
                   
                }
                else if (!activateToUse)
                {
                    other.gameObject.GetComponent<OverworldMovement>().canMove = false;
                    transitionManager.Warp(exitIndex, sceneToLoad, other.GetComponent<OverworldMovement>().playerSprite.flipX);
                    
                }
            }
        }

        
    }
}
