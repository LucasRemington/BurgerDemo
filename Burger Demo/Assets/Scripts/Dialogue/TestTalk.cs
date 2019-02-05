using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class TestTalk : MonoBehaviour
{
    public Dialogue tut;
    public Text text;

    // Use this for initialization
    void Start () {

        text.text = tut.DialogItems[0].DialogueText.ToString();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
