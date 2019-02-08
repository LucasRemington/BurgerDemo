using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractDia : MonoBehaviour {

    //critical game objects
    public GameObject MainCamera;
    public NarrativeManager nm;
    public DialogHolder dh;
    public GameObject player;

    public int identity;
    public bool canInteract;
    public int identity2;
    public bool repeatChange;
    public bool didInteract;

    private void Start()
    {
        MainCamera = GameObject.FindWithTag("MainCamera");
        nm = MainCamera.GetComponent<NarrativeManager>();
        canInteract = true;
    }

    private void OnTriggerStay2D(Collider2D collision) //detects player
    {
        if (collision.gameObject == player && Input.GetKeyDown(KeyCode.Space) && canInteract == true && repeatChange == true && didInteract == true)
        {
            StartCoroutine(dh.GenericInteractable(identity2));
            StartCoroutine(interactTimer());
        }
        else if (collision.gameObject == player && Input.GetKeyDown(KeyCode.Space) && canInteract == true)
        {
            StartCoroutine(dh.GenericInteractable(identity));
            StartCoroutine(interactTimer());
            
        } 
    }

    IEnumerator interactTimer ()
    {
        canInteract = false;
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => nm.owm.canMove == true);
        didInteract = true;
        canInteract = true;
    }
}
