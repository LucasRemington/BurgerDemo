using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningAnimEvent : MonoBehaviour {

    public GameObject tutEnemy;
    public TutorialEnemy te;

    void Start ()
    {
        tutEnemy = transform.parent.gameObject;
        te = tutEnemy.GetComponent<TutorialEnemy>();
    }

	void StartTalking () {
        te.ph.DealDamage(5);
	}
}
