using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DennisAnimEvent : MonoBehaviour {

    public GameObject MainCamera;
    public NarrativeManager nm;
    public DialogHolder dh;
    public GameObject player;
    public Animator playerAnim;

    private AudioSource audio;
    public AudioClip handshakeSound;
    public AudioClip buttonPressSound;

    public bool did;
    public GameObject[] kill;
    private int i;
    private bool Started;

    void Start () {
        audio = GetComponent<AudioSource>();
        StartCoroutine(lateStart());
    }

    private IEnumerator lateStart ()
    {
        Debug.Log("late start tried ");
        if (Started == false)
        {
            Debug.Log(this.gameObject.name.ToString());
            Started = true;
            yield return new WaitForEndOfFrame();
            MainCamera = GameObject.FindWithTag("MainCamera");
            nm = MainCamera.GetComponent<NarrativeManager>();
            player = GameObject.FindWithTag("Player");
            player = player.transform.Find("OverworldPlayer").gameObject;
            playerAnim = player.GetComponent<Animator>();
        }
    }

    private void OnTriggerStay2D(Collider2D collision) //detects player
    {
        if (collision.gameObject == player && did == false)
        {
            nm.ns1.animationFlag = true;
            did = true;
        }
    }

    public void triggerAnim ()
    {
        Debug.Log("triggerAnim");
        nm.ns1.animationFlag = true;
    }

    public void killObject ()
    {
        kill[i].SetActive(false);
        i++;
    }

    void Handshake()
    {
        audio.clip = handshakeSound;
        audio.Play();
    }

    void ButtonPress()
    {
        audio.clip = buttonPressSound;
        audio.Play();
    }

    void AppearSound() // for the master
    {
        GetComponent<AudioSource>().Play();
    }
}
