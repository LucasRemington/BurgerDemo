using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {
    private bool playerTouch = false;
    public GameObject player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (playerTouch && Input.GetKeyDown(KeyCode.Space)) {
            Interact();
        }
	}

    public void Interact() {
        // here i'll put a bunch of if statements that check if certain interactable scripts are on the same object. stuff like signs, doors, buttons and such
        if (GetComponent<Sign>() != null && !GetComponent<Sign>().reading)
        {
            StartCoroutine(GetComponent<Sign>().ShowDialogue());
        }
        else if (GetComponent<SceneSwitcher>() != null)
        {
            GetComponent<SceneSwitcher>().ToTheNextScene();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "OverworldPlayer")
        {
            playerTouch = true;
            player = other.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "OverworldPlayer") playerTouch = false;
    }
}
