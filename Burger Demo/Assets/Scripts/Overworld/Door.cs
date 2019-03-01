using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour {

    public GameObject gameController;
    public int RoomComingFrom;
    public string RoomGoingTo;
    public int roomToIdentity;
    public int dialogueIdentity;

    public GameObject MainCamera;
    public NarrativeManager nm;
    public DialogHolder dh;
    public GameObject player;
    public Animator playerAnim;

    public bool canInteract;
    public bool loadedScene;
    public bool automatic;

    private void Start()
    {
        MainCamera = GameObject.FindWithTag("MainCamera");
        nm = MainCamera.GetComponent<NarrativeManager>();
        player = GameObject.FindWithTag("Player");
        player = player.transform.Find("OverworldPlayer").gameObject;
        //playerAnim = player.GetComponent<Animator>();
        canInteract = true;
    }

    private void OnTriggerStay2D(Collider2D collision) //detects player
    {
        if (collision.gameObject == player && Input.GetKeyDown(KeyCode.Space) && canInteract == true || collision.gameObject == player && automatic)
        {
            if (loadedScene == false)
            {
                nm.room = roomToIdentity;
                SceneManager.LoadSceneAsync(RoomGoingTo);
                loadedScene = true;
            } else
            {
                Debug.Log("tried to load scene");
            }
            
        }
        else if (collision.gameObject == player && Input.GetKeyDown(KeyCode.Space) && canInteract == false)
        {
            //StartCoroutine(dh.GenericInteractable(dialogueIdentity));
        }
    }

}
