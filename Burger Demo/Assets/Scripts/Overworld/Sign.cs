using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sign : MonoBehaviour {

    public string signText;
    public Text textBox;
    public bool reading = false;

    public float timeBetween = 1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public IEnumerator DisplayText(GameObject player) {
        player.GetComponent<OverworldMovement>().canMove = false;
        reading = true;
        for (int i = 0; i < signText.Length; i++) {
            yield return new WaitForSeconds(timeBetween);
            textBox.text = textBox.text + signText[i];
        }
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        textBox.text = "";
        reading = false;
        player.GetComponent<OverworldMovement>().canMove = true;
    }
}
