using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private SaveLoad saveLoad;
    //[Tooltip("Makes sure that we aren't saving every frame we're inside the checkpoint.")] public int index;

	void Start ()
    {
        saveLoad = GameObject.FindGameObjectWithTag("Base").GetComponent<SaveLoad>();
        
	}

    // Called as the player enters its trigger.
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Overworld" && other.gameObject.GetComponent<OverworldMovement>().canMove)
        {
            // On collision with the player, save the game without adjusting ingredients or such.
            StartCoroutine(saveLoad.SaveGame());
            Destroy(gameObject);
        }
    }

}
