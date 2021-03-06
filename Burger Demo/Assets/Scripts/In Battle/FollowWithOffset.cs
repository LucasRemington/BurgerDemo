﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWithOffset : MonoBehaviour {

    public Transform target;
    public float xOff;
    public float yOff;
    public TextMesh tm;
    public bool notText;

	void Start () {
        
        StartCoroutine(LockToTarget());
        tm = GetComponent<TextMesh>();
    }
	
	IEnumerator LockToTarget () {
        yield return new WaitForSeconds(0.2f);
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("BattleEnemy").transform;
        }
        if (tm.text == "" || notText == true)
        {
            this.transform.position = new Vector2(target.position.x + xOff, target.position.y + yOff);
        }
        StartCoroutine(LockToTarget());
	}
}
