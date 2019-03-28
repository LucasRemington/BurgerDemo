﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PseudoStart : MonoBehaviour {

    public BattleTransitions bt;
    public FollowPlayer fp;
    public OverworldMovement ovm;
    public NarrativeManager nm;
    public NarrativeScript1 ns1;
    public DialogueHolder dh;
    public GameObject GameController;
    public GameObject playerHolder;
    public GameObject player;
    public GameObject MainCamera;
    public BurgerComponentInstantiator BCI;
    public Vector3 minCameraPos;
    public Vector3 maxCameraPos;

    public int Identity;

	// Use this for initialization
	void Start () {
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        nm = MainCamera.GetComponent<NarrativeManager>();
        ns1 = MainCamera.GetComponent<NarrativeScript1>();
        dh = MainCamera.GetComponent<DialogueHolder>();
        fp = MainCamera.GetComponent<FollowPlayer>();
        fp.minCameraPos = minCameraPos;
        fp.maxCameraPos = maxCameraPos;
        playerHolder = GameObject.FindGameObjectWithTag("Player");
        player = playerHolder.transform.Find("OverworldPlayer").gameObject;
        ovm = player.GetComponent<OverworldMovement>();
        GameController = GameObject.FindWithTag("GameController");
        bt = GameController.GetComponent<BattleTransitions>();
        BCI = MainCamera.transform.Find("Canvas").Find("CombatUI").Find("BurgerSpawner").GetComponent<BurgerComponentInstantiator>();
        nm.PseudoStart();
        ns1.PseudoStart();
        dh.PseudoStart();
        fp.PseudoStart();
        bt.PseudoStart();
        MainCamera.transform.position = new Vector3(-2f, -1f, -5);
        nm.room = Identity; //temp solution
        ovm.PseudoStart();
        StartCoroutine(BCI.StartStuff());
    }
	
}
