using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBAnimEvents : MonoBehaviour {

    public GameObject MainCamera;
    public NarrativeManager nm;
    public GameObject IB;
    public AudioSource audio;
    public AudioClip shortBubble;
    public AudioClip longBubble;
    public AudioClip sploosh;
    public AudioClip openDoor;
    public AudioClip Cough;

    void Awake () {
        MainCamera = GameObject.FindWithTag("MainCamera");
        nm = MainCamera.GetComponent<NarrativeManager>();
        audio = GetComponent<AudioSource>();
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
