﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackLaunch : MonoBehaviour {

    //launches attack and triggers death through animation

    public GameObject burgerSpawner; //This and BCI reference the static script
    public BurgerComponentInstantiator BCI;
    private bool didDie;

    void Start()
    {
        StartCoroutine(PseudoStart());
    }

    void LaunchAttack () {
        BCI.LaunchBurger();
	}

    void Die ()
    {
        if (didDie == false)
        {
            didDie = true;
            BCI.UponDeath();
        }
    }

    public IEnumerator PseudoStart() {
        while (BCI == null)
        {
            yield return new WaitForEndOfFrame();
            BCI = this.gameObject.GetComponentInParent<PlayerHealth>().BCI;
        }
    }
}
