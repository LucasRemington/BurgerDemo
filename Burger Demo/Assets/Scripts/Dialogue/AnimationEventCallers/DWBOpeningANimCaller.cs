using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DWBOpeningANimCaller : MonoBehaviour {

    public Image black; //A black screen -- for the main menu, this will fade out.
    [HideInInspector] public GameObject openingLogo; //This gameobject, since the script is always on the logo itself.
    [HideInInspector] public NarrativeManager nm; //Narrative manager.
    [HideInInspector] public GameObject MainCamera; //Main camera
    public Vector3 endPosition; //The end position - what the logo will move TO
    [HideInInspector] public Vector3 startPosition; //The start position - what the logo will move FROM
    [HideInInspector] Vector3 currentPosition; //temp variable used to track current position
    public Vector3 startSize; //The starting size of the object - what the logo will grow FROM
    public Vector3 endSize; //The ending size of the object - what the logo will grow TO
    private float Tick; //Used to move with a coroutine instead of relying on update.
    [HideInInspector] public GameObject optionMenu; // the esc menu. During the mainmenu scene, that menu doubles as the start menu.
    [HideInInspector] public MenuManager mm; // the esc menu script.
    [HideInInspector] public AudioSource audS; //The audio source used to play some sounds: on the object.

    void Start () //Grabs all the relevant scripts and objects. black still needs to be set in inspector - it's the black screen that will fade out.
    {
        openingLogo = this.gameObject;
        MainCamera = GameObject.FindWithTag("MainCamera");
        nm = MainCamera.GetComponent<NarrativeManager>();
        optionMenu = GameObject.FindWithTag("OptionsMenu");
        mm = optionMenu.GetComponent<MenuManager>();
        audS = GetComponent<AudioSource>();
        black = GameObject.FindGameObjectWithTag("BlackScreen").GetComponent<Image>();
    }
	
	void Shrink () //called from an animation event, this calls LerpOver when the animation finishes.
    {
        if (nm.room == 0)
        {
            startPosition = this.transform.localPosition;
            StartCoroutine(lerpOver());
        }
	}

    void Pulse() //A simple animation event that calls some audio when it triggers.
    {
        if (nm.room == 0)
        {
            audS.Play();
        }
    }

    IEnumerator lerpOver () //this moves the logo over to the end position when called, and shinks it down to endSize.
    {
        transform.localPosition = Vector3.Lerp(startPosition, endPosition, (Tick / 25f));
        transform.localScale = Vector3.Lerp(startSize, endSize, (Tick / 25f));
        currentPosition = this.transform.localPosition;
        yield return new WaitForSeconds(0.01f);
        if (currentPosition != endPosition)
        {
            Tick++;
            StartCoroutine(lerpOver());
        }
        else
        {
            Tick = 0;
            StartCoroutine(nm.bci.FadeImageToZeroAlpha(1, black));
            yield return new WaitForSeconds(1f);
            openingLogo.GetComponentInParent<Canvas>().sortingOrder = -2;
            mm.animFlag = true;
        }
    }

}
