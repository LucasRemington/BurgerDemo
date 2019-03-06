using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private SaveLoad saveLoad;
    //[Tooltip("Makes sure that we aren't saving every frame we're inside the checkpoint.")] public int index;
    private bool canCheckpoint = true;

	void Start ()
    {
        saveLoad = GameObject.FindGameObjectWithTag("Base").GetComponent<SaveLoad>();
        
	}

    // Called as the player enters its trigger.
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Overworld" && canCheckpoint && other.gameObject.GetComponent<OverworldMovement>().canMove)
        {
            // On collision with the player, save the game without adjusting ingredients or such.
            StartCoroutine(saveLoad.SaveGame());
            canCheckpoint = false;
            StartCoroutine(Reactivate());
        }
    }

    private IEnumerator Reactivate()
    {
        yield return new WaitForSeconds(3.0f);
        canCheckpoint = true;
    }
}
