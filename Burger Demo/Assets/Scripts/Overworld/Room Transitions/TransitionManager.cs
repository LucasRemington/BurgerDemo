using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// =============== DOCUMENTATION ===================
/*
Alright, here's what you need to know about this script, TransitionTracker, and TransitionExit:

First, we have a TransitionManager, which is Don't Destroy On Load.

In individual scenes, let's have their own independent TransitionTrackers; these have a List of various Transforms, which level designers can set the size of with an int.
	Transforms should be set manually via drag and drop; each of these represent somewhere the player can enter the room from. In addition, these should be children of the TransitionTracker.
	Each Transform should possess a box trigger on it (but displaced, so NOT centered), and a small script that calls a Warp function TransitionManager.
		This function call should use an integer set in the inspector, which corresponds to the transform index in the other scene, as well as the scene in question.
	In the TransitionManager script, the Warp function should be public and accept an integer and a scene.
		The Warp function makes the screen fade to black, loads the called scene, sets the player's position to the transform at the called index, and then fades back in. Probably fiddle with canMove.
		To do this, the TransitionManager must find the TransitionTracker in the loaded scene, cross reference the player and relevant transform from it, and then do so. 
		If canMove is false, however, we do NOT warp, and instead call on the narrative script to handle that.

	Make sure to also link the player in each world to the TransitionTracker.

So for the level designers: 
TransitionManager: Contains the warp function. You shouldn't need to touch this, and if it's put in each room, it shouldn't matter, as long as it's wherever the player starts.
TransitionTracker: One in each scene. Link to the player manually, and place each Exit in its List of exits.
Exit: Place at every place the player can enter or leave a room from. If it's one-way, disable the triggerbox at its entrance.
	Set SceneToLoad to the scene you are moving to. Set the index integer to the index of the Exit in the TransitionTracker's List that you previously set.
	If you toggle InteractWarp: Place the trigger over the door or object in question. The transform can be here as well.
	If you do NOT toggle InteractWarp: Place the trigger away from the center of the object so the player doesn't start inside of it when they load in.
*/
// ==================================================


public class TransitionManager : MonoBehaviour
{
    [HideInInspector] [Tooltip("Used in the screen transition from fading to black and back in. Will do this later.")] public bool isFading;
    [HideInInspector] public bool readyForFade = true;
    [Tooltip("The TransitionTracker in the current scene. Updates every time the scene changes, you shouldn't need to set this.")] public TransitionTracker currentTracker;
    [Tooltip("The player! Found via script, don't even worry.")]  public GameObject player;
    [Tooltip("The black UI image in canvas that's used to fade out and back in.")] public Image fadeOutScreen;

    [Tooltip("The camera that follows the player. We need to move it after warping.")] public GameObject mainCam;

    private SaveLoad saveLoad;

    [SerializeField] private bool coCalled;
	
    // If our transition tracker isn't set, which happens upon loading a new scene, re set it! This will keep trying until, effectively, the scene is loaded!
	void Update ()
    {
        if (currentTracker == null)
        {
            GameObject currTrackObj = GameObject.FindGameObjectWithTag("Transition Tracker");
            if (currTrackObj != null)
            {
                currentTracker = currTrackObj.GetComponent<TransitionTracker>();
            }
            
        }

        if (mainCam == null)
        {
            mainCam = GetComponentInParent<NarrativeManager>().gameObject;
        }

        if (saveLoad == null)
        {
            saveLoad = GameObject.FindGameObjectWithTag("Base").GetComponent<SaveLoad>();
        }

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Overworld");
	}

    public void Warp(int index, SceneField scene, bool flip, bool autoSave)
    {
        player.GetComponent<OverworldMovement>().canMove = false;

        fadeOutScreen.enabled = true;
        StopCoroutine(WaitForLoad(index, scene, flip, autoSave));
        StopCoroutine(FadeImageToFullAlpha(0.5f, fadeOutScreen));
        StartCoroutine(FadeImageToFullAlpha(0.5f, fadeOutScreen));
        StartCoroutine(WaitForLoad(index, scene, flip, autoSave));
    }

    // We have an enumerator with a waituntil for when the transition tracker isn't set, and once it is, we pretty much know the scene is loaded, so. 
    // So we grab the player and move them to the indicated transform.

    // We wait until we've fully faded to black before we go back through and load everything.
    private IEnumerator WaitForLoad(int index, SceneField scene, bool flip, bool autoSave)
    {
        yield return new WaitUntil(() => coCalled);
        SceneManager.LoadScene(scene);
        currentTracker = null;

        yield return new WaitUntil(() => currentTracker != null);
        Debug.Log("We are now in a new scene!!");            

        player.GetComponent<OverworldMovement>().canMove = false;
        player.GetComponent<OverworldMovement>().playerSprite.flipX = flip;
        player.transform.position = currentTracker.exitList[index].position;
        mainCam.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1, -5);

        // Auto-save!
        if (autoSave)
        {
            StartCoroutine(saveLoad.SaveGame());
        }



        StopCoroutine(FadeImageToZeroAlpha(0.5f, fadeOutScreen));
        StartCoroutine(FadeImageToZeroAlpha(0.5f, fadeOutScreen));

        yield return new WaitUntil(() => fadeOutScreen.color.a <= 0);
        player.GetComponent<OverworldMovement>().canMove = true;
    }

    // Oh hey this is borrowed from bci script!

    public IEnumerator FadeImageToFullAlpha(float t, Image i) //used to fade in and fade out scene, and controls UI for death
    {
        /*foreach (Transform exit in currentTracker.GetComponent<TransitionTracker>().exitList) {
            exit.gameObject.GetComponent<TransitionExit>().warping = false;
        }*/
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        coCalled = false;
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            //Debug.Log("Fading in: " + SceneManager.GetActiveScene().name);
            yield return null;
        }
        if (i.color.a >= 1.0f && coCalled == false)
        {
            //StartCoroutine(whenBlackScreen());
            coCalled = true;
            yield return null;
        }
    }

    public IEnumerator FadeImageToZeroAlpha(float t, Image i)
    {
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => readyForFade == true);
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
        
    }
}
