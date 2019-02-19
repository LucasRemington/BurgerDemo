using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningAnimEvent : MonoBehaviour {

    public GameObject tutEnemy;
    public TutorialEnemy te;

    void Start ()
    {
        tutEnemy = GameObject.Find("HoloMaster");
        te = tutEnemy.GetComponent<TutorialEnemy>();
    }

	void StartTalking () {
		
	}
}
