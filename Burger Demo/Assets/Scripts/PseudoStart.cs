using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PseudoStart : MonoBehaviour {

    public BattleTransitions bt;
    public FollowPlayer fp;
    public OverworldMovement ovm;
    public NarrativeManager nm;
    public NarrativeScript1 ns1;
    public DialogHolder dh;
    public GameObject GameController;
    public GameObject playerHolder;
    public GameObject player;
    public GameObject MainCamera;

    public int Identity;

	// Use this for initialization
	void Start () {
        MainCamera = GameObject.FindWithTag("MainCamera");
        nm = MainCamera.GetComponent<NarrativeManager>();
        ns1 = MainCamera.GetComponent<NarrativeScript1>();
        dh = MainCamera.GetComponent<DialogHolder>();
        fp = MainCamera.GetComponent<FollowPlayer>();
        playerHolder = GameObject.FindWithTag("Player");
        player = playerHolder.transform.Find("OverworldPlayer").gameObject;
        ovm = player.GetComponent<OverworldMovement>();
        GameController = GameObject.FindWithTag("GameController");
        bt = GameController.GetComponent<BattleTransitions>();
        nm.PseudoStart();
        ns1.PseudoStart();
        dh.PseudoStart();
        fp.PseudoStart();
        ovm.PseudoStart();
        bt.PseudoStart();
        MainCamera.transform.position = new Vector3(-2f, -1f, -5);
        nm.room = Identity; //temp solution
    }
	
}
