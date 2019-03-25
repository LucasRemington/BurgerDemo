using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LouNoteTrigger : MonoBehaviour
{
    // This script causes its parent object to drop from where it is and fall to the floor and bounce a few times before stopping. 
    // However, due to how scripting with collision works, both parent and child need the script with slightly different values; the bool here checks for that.

    private GameObject note; // The parent object, the note. 
    public Rigidbody2D rb; // The parent's rigidbody2D component.
    [Tooltip("How many times the note should bounce on the ground before becoming static.")] public int bounceCount = 3;
    [HideInInspector] public bool isChild = true; // Whether or not this item is the hitbox for the player walking through or not.
    [HideInInspector] public bool read; // Whether or not this particular note has been read. 
    [HideInInspector] public InteractDia intDia; // The interact dialogue component.
    [HideInInspector] public Dialogue dia; // The first dialogue object inside the intDia component, used in reference by other scripts.

	void Start ()
    {
        StartCoroutine(CoStart()); // We essentially put all of our start functionality into a coroutine, that way we can set the parent's values after adding the script to it without the parent trying to access them first.
	}

    private IEnumerator CoStart()
    {
        yield return new WaitForSeconds(0.05f);

        if (isChild)
        {
            // If this is the child object, set we instantiate this script onto the parent and adjust its values accordingly.
            note = gameObject.transform.parent.gameObject;
            note.AddComponent<LouNoteTrigger>();

            LouNoteTrigger lnt = note.GetComponent<LouNoteTrigger>();
            lnt.isChild = false;
            lnt.bounceCount = bounceCount;

            intDia = note.GetComponentInChildren<InteractDia>();
            dia = intDia.dialogueList[0];
        }
        else
        {
            note = gameObject;
            intDia = note.GetComponentInChildren<InteractDia>();
            dia = intDia.dialogueList[0];
        }

        rb = note.GetComponent<Rigidbody2D>();
    }
    

    // FUNCTION: Parent-only, theoretically could work in both but is not needed in child. Grab the InteractDia component in the child (done in CoStart), and each frame check if it's been read and adjust a value to be read from LouNoteHolder.
    private void Update()
    {
        if (!isChild && !read && intDia != null)
        {
            // We check the interactable dialogue script to see if its index equals or exceeds 
            if (intDia.i >= intDia.dialogueList.Count)
            {
                Debug.Log("Note read! Adjusting...");

                read = true;
                LouNoteHolder holder = GetComponentInParent<LouNoteHolder>();

                holder.ListUpdate(dia);
            }

        }
    }


    // FUNCTION: Child-only. Detect if the player walks under the note, and then have it fall. The parent should not be able to collide with the player anyhow. Disable the trigger.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Overworld" && other.gameObject.GetComponent<OverworldMovement>().canMove)
        {
            rb.gravityScale = 2;
            if (isChild)
                GetComponent<Collider2D>().enabled = false;
        } 
    }

    // FUNCTION: Parent-only. Check if this is the parent object; if so, every time we collide with the floor, subtract one from bounce; once it drops below 0, stop movement and disable its collider and in rb change it to be static rather than dynamic.
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!isChild && (other.gameObject.tag == "Floor" || other.gameObject.tag == "Ladder"))
        {
            if (bounceCount >= 0)
            {
                bounceCount--;
            }
            else
            {
                note.GetComponent<Collider2D>().enabled = false;
                rb.bodyType = RigidbodyType2D.Static;
            }
        }
    }
}
