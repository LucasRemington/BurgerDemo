using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeManager : MonoBehaviour {

    public NarrativeScript1 ns1;

    public int ev; //integer that determines what can happen. Each 'event' to occur should increase it by one.

    public int area; //used to identify area
    public int room; //used to identify room
    public Text text; // text box text
    public bool textActive; //wait for text
    public Animator popUp;

    void Start ()
    {
        ns1 = GetComponent<NarrativeScript1>();
        CheckEvent();
        StartCoroutine(waitForText());
    }
	
    public void CheckEvent ()
    {
        switch (ev)
        {
            case 0:
                StartCoroutine(eventZero());
                break;
            case 1:
                StartCoroutine(ns1.eventOne());
                break;
        }
    }

    IEnumerator waitForText ()
    {
        yield return new WaitUntil(() => textActive == true);

    }

	IEnumerator eventZero ()
    {
        //yield return new WaitUntil(() => room == 1);
        yield return new WaitForSeconds(0.5f);
        ev++;
        CheckEvent();
    }



}
