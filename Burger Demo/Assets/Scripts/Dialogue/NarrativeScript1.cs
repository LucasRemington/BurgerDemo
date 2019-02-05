using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeScript1 : MonoBehaviour {

    public NarrativeManager nm;

    public Dialogue[] dennis1; //each 'dialogue' is a different conversation, each element is a different dialog box. 1 indicates that this takes place during the first event only.
    public int dennisConvo1; //used to keep track of the point in conversation one would be at.
    public bool dennisConvo1Done; //used to track which conversations have finished

    private void Start()
    {
        nm = GetComponent<NarrativeManager>();
    }

    public IEnumerator eventOne()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(dennisFirstConvo());
        yield return new WaitUntil(() => nm.room == 1);
        nm.ev++;
        nm.CheckEvent();
    }

    IEnumerator dennisFirstConvo()
    {
        int sizeOfList;
        sizeOfList = dennis1[0].DialogItems.Count;
        yield return new WaitForSeconds(0.1f);
        nm.text.text = dennis1[0].DialogItems[dennisConvo1].DialogueText.ToString();
        dennisConvo1++;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        if (dennisConvo1 >= sizeOfList)
        {
            yield return new WaitForEndOfFrame();
        }
        else
        {
            StartCoroutine(dennisFirstConvo());
        }
    }

}
