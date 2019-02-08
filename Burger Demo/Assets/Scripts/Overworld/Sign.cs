using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sign : MonoBehaviour {

    public GameObject player;           // make sure the player is named this
    public bool reading = false;
    private bool scrolling = false;
    public float timeBetween = 0.05f;      // the amount of time between each letter
    private float between;
    public Text textBox;
    public string[] dialogueLines;      // this is where you write lines of text, each part in the array is a seperate line

    // Use this for initialization
    void Start () {
        player = GameObject.Find("OverworldPlayer");
        between = timeBetween;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space) && scrolling) {         // if the player hits space while the text is still scrolling, it will speed up
            between = 0;
        }
	}

    public IEnumerator ShowDialogue(/*string[] dialogue*/) {
        if (player != null)
        {
            player.GetComponent<OverworldMovement>().canMove = false;
        }
        reading = true;
        for (int i = 0; i < dialogueLines.Length; i++)           // this loop cycles every time you press space
        {
            scrolling = true;
            for (int j = 0; j < dialogueLines[i].Length; j++)    // this loop cycles every "timeBetween" seconds and puts a letter down
            {
                yield return new WaitForSeconds(between);
                textBox.text = textBox.text + dialogueLines[i][j];
            }
            scrolling = false;
            between = timeBetween;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            textBox.text = "";
        }
        reading = false;
        if (player != null)
        {
            player.GetComponent<OverworldMovement>().canMove = true;
        }
    }
}
