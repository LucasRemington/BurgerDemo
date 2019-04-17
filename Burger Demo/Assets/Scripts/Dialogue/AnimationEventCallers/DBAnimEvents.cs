using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBAnimEvents : MonoBehaviour {

    public GameObject MainCamera;
    public NarrativeManager nm;
    public GameObject IB;
    public GameObject Player;
    public AudioSource audio;
    public AudioClip shortBubble;
    public AudioClip longBubble;
    public AudioClip sploosh;
    public AudioClip openDoor;
    public AudioClip Cough;
    public ActionSelector actSelect;

    void Awake () {
        MainCamera = GameObject.FindWithTag("MainCamera");
        nm = MainCamera.GetComponent<NarrativeManager>();
        audio = GetComponent<AudioSource>();
        actSelect = GameObject.FindGameObjectWithTag("GameController").GetComponent<ActionSelector>();
    }

    void ReadyUp() // Called from the new Combat UI; Forces ActionSelector to wait until the item box closes before it can start up the BCI sections once a combo is selected.
    {
        actSelect.readyUp = true;
    }

    void BackHome() // Called from ingredients now actually.
    {
        actSelect.backHome = true;
    }

    void IsReady() // Called when we finish opening certain menus or when ingredients spawn in, allowing us to navigate and select things.
    {
        actSelect.isReady = true;
    }

    void Unready() // Called when ingredients start to spawn in or disappear, and when certain menus open, preventing us from selecting things.
    {
        actSelect.isReady = false;
    }
	
	void StartText ()
    {
        nm.dbStartStop = true;
	}

    void StopText ()
    {
        Debug.Log("startstop");
        nm.dbStartStop = true;
    }

    void enableBackgrounds ()
    {
        IB.SetActive(true);
    }

    void enablePlayer()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Player = Player.transform.GetChild(0).gameObject;
        Player.SetActive(true);
        Player.GetComponent<OverworldMovement>().canMove = true;
    }

    void Choice()
    {
        nm.dbChoiceSS = true;
    }

    void Splash()
    {
        audio.pitch = Random.Range(0.75f, 1.25f);
        audio.clip = sploosh;
        audio.Play();
        
    }

    void BubbleLong()
    {
        audio.pitch = 1;
        audio.clip = longBubble;
        audio.Play();
    }

    void BubbleShort()
    {
        audio.pitch = 1;
        audio.clip = shortBubble;
        audio.Play();
    }

    void DoorOpen()
    {
        audio.pitch = 1;
        audio.clip = openDoor;
        audio.Play();
    }

    void CoughCough() {
        audio.pitch = 1;
        audio.clip = Cough;
        audio.Play();
    }
}
