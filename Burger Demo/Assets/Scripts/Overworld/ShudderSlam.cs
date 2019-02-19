using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShudderSlam : MonoBehaviour {

    public Animator anim;
    public GameObject player;

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
}
