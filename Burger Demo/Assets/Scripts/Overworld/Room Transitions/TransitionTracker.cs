using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionTracker : MonoBehaviour
{
    [Tooltip("A list of every exit location in this scene. Must be manually set. Set 'size' to however many exits there are in this room.")] public List<Transform> exitList;
    [Tooltip("The player object in each scene. Grabbed via script, you don't need to touch this! Except in the freezer. Please do that. The starting freezer. Y'know.")] public GameObject player;

	void Start ()
    {
        if (player == null)
        {
            GameObject tempPlayer = GameObject.FindGameObjectWithTag("Player");
            player = tempPlayer.GetComponentInChildren<OverworldMovement>().gameObject;
        }
    }

	void Update () {
		
	}
}
