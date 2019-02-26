﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShudderSlam : MonoBehaviour {

    public Animator anim;
    public GameObject player;
    public AudioClip clip;


    void Start () {
        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        player = player.transform.Find("OverworldPlayer").gameObject;
    }

    private void OnTriggerStay2D(Collider2D collision) //detects player
    {
        if (collision.gameObject == player)
        {
            anim.SetTrigger("Slam");
        }
    }

    void Sound()
    {
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();

    }
}
