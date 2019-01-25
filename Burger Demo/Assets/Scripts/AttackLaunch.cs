using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackLaunch : MonoBehaviour {

    public GameObject burgerSpawner; //This and BCI reference the static script
    public BurgerComponentInstantiator BCI;

    void Start()
    {
        burgerSpawner = GameObject.Find("BurgerSpawner");
        BCI = burgerSpawner.GetComponent<BurgerComponentInstantiator>();
    }

    void LaunchAttack () {
        BCI.LaunchBurger();
	}
	
}
