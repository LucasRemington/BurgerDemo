using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractDia : MonoBehaviour {

    //critical game objects
    public GameObject MainCamera;
    public NarrativeManager nm;
    public DialogHolder dh;
    public GameObject player;
    public Animator playerAnim;
    public GenericSounds gs;

    public int identity;
    public bool canInteract;
    public int identity2;
    public bool repeatChange;
    public bool didInteract;

    private void Start()
    {
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        nm = MainCamera.GetComponent<NarrativeManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        player = player.transform.Find("OverworldPlayer").gameObject;
        playerAnim = player.GetComponent<Animator>();
        canInteract = true;
        dh = MainCamera.GetComponent<DialogHolder>();
    }

    private void OnTriggerStay2D(Collider2D collision) //detects player
    {
        if (collision.gameObject == player && Input.GetKeyDown(KeyCode.Space) && canInteract == true && repeatChange == true && didInteract == true && !nm.bt.battling)
        {
            StartCoroutine(dh.GenericInteractable(identity2));
            StartCoroutine(interactTimer());
        }
        else if (collision.gameObject == player && Input.GetKeyDown(KeyCode.Space) && canInteract == true && !nm.bt.battling)
        {
            StartCoroutine(dh.GenericInteractable(identity));
            StartCoroutine(interactTimer());
            
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
        didInteract = true;
        canInteract = true;
    }
}
