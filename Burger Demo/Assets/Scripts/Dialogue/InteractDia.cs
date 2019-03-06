using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractDia : MonoBehaviour {

    //critical game objects
    private GameObject MainCamera;
    private NarrativeManager nm;
    private DialogueHolder dh;
    private GameObject player;
    private Animator playerAnim;
    private GenericSounds gs;
    private bool canInteract = true;

    /*[Tooltip("The index of the first dialogue in the dialogue holder's interactable list.")] public int identity;
    
    [Tooltip("The index of the second dialogue in the dialogue holder's interactable list. Only occurs after the player has interacted with this object before, and repeatChange is ticked.")] public int identity2;
    [Tooltip("Does interacting with this a second time change the dialogue?")] public bool repeatChange;
    private bool didInteract;*/

    [Tooltip("Each dialogue that this object should bring up. Each interaction increments the index by one, but won't exceed length. So effectively, only add as many dialogues as you want the player to see, and the last will be repeated. Keep size to 1 for a single interaction that never changes.")] public List<Dialogue> dialogueList;
    //private int listLength;
    private int i = 0;


    private void Start()
    {
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        nm = MainCamera.GetComponent<NarrativeManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        player = player.transform.Find("OverworldPlayer").gameObject;
        playerAnim = player.GetComponent<Animator>();
        dh = MainCamera.GetComponent<DialogueHolder>();
        gs = player.GetComponent<GenericSounds>();
    }

    private void OnTriggerStay2D(Collider2D collision) // Detects when the player is present and can interact.
    {
        /*if (collision.gameObject == player && Input.GetKeyDown(KeyCode.Space) && canInteract == true && repeatChange == true && didInteract == true && !nm.bt.battling)
        {
            StartCoroutine(dh.GenericInteractable(identity2));
            StartCoroutine(interactTimer());
        }
        else if (collision.gameObject == player && Input.GetKeyDown(KeyCode.Space) && canInteract == true && !nm.bt.battling)
        {
            StartCoroutine(dh.GenericInteractable(identity));
            StartCoroutine(interactTimer());
        } */

        if (collision.tag == "IntTrigger" && Input.GetKeyDown(KeyCode.Space) && canInteract == true && !nm.bt.battling && player.GetComponent<OverworldMovement>().canMove)
        {
            StartCoroutine(dh.GenericInteractableNew( dialogueList[i] ));
            StartCoroutine(interactTimer());
            if (gameObject.tag == "MeatLocker")
            {
                StartCoroutine(MainCamera.GetComponentInParent<SaveLoad>().MeatLockerEvent(dialogueList[i]));
            }
        }
    }

    IEnumerator interactTimer ()
    {
        canInteract = false;
        gs.Step();
        yield return new WaitForSeconds(0.5f);
        playerAnim.SetBool("Thinking", true);
        yield return new WaitUntil(() => nm.owm.canMove == true);
        gs.Step();
        playerAnim.SetBool("Thinking", false);
        canInteract = true;

        if (i < dialogueList.Count - 1)
        {
            i++;
        }
    }
}
