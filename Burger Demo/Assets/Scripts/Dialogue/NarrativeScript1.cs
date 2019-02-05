using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeScript1 : MonoBehaviour {

    public NarrativeManager nm;

    public Dialogue[] dennis1; //each 'dialogue' is a different conversation, each element is a different dialog box. 1 indicates that this takes place during the first event only.
    public int dennisConvo1; //used to keep track of the point in conversation one would be at.
    public bool dennisConvo1Start; //true when the convo has started
    public bool dennisConvo1Done; //used to track which conversations have finished

    //public Dialogue[] master1; //each 'dialogue' is a different conversation, each element is a different dialog box. 1 indicates that this takes place during the first event only.
    public int masterConvo1; //used to keep track of the point in conversation one would be at.
    public bool masterConvo1Start; //true when the convo has started
    public bool masterConvo1Done; //used to track which conversations have finished

    public int sizeOfList; //set equal to the number of items in a dialogue element

    private void Start()
    {
        nm = GetComponent<NarrativeManager>();
    }

    public IEnumerator eventOne()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(dennisFirstConvo());
        StartCoroutine(masterCombatStarter());
        yield return new WaitUntil(() => nm.room == 1);
        nm.ev++;
        nm.CheckEvent();
    }

    IEnumerator dennisFirstConvo()
    {
        if (dennisConvo1Start == false)
        {
            dennisConvo1Start = true;
            StartCoroutine(nm.dialogueBox(true));
            sizeOfList = dennis1[0].DialogItems.Count;
        }
        nm.setTalksprite(dennis1[0], dennisConvo1);
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(nm.AnimateText(dennis1[0], dennisConvo1));
        dennisConvo1++;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) && nm.canAdvance == true);
        if (dennisConvo1 >= sizeOfList)
        {
            StartCoroutine(nm.dialogueEnd());
            dennisConvo1Done = true;
            nm.combat = true;
        }
        else
        {
            Debug.Log("began coroutine again");
            StartCoroutine(dennisFirstConvo());
        }
    }

    IEnumerator masterCombatStarter ()
    {
        yield return new WaitUntil(() => nm.combat == true);
        yield return new WaitForSeconds(2f);
        StartCoroutine(masterCombatConvo2());
    }

    IEnumerator masterCombatConvo2 ()
    {

        if (masterConvo1Start == false)
        {
            masterConvo1Start = true;
            sizeOfList = dennis1[1].DialogItems.Count;
        }
        nm.setTalksprite(dennis1[1], masterConvo1);
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(nm.AnimateText(dennis1[1], masterConvo1));
        masterConvo1++;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) && nm.canAdvance == true);
        if (masterConvo1 >= sizeOfList)
        {
            StartCoroutine(nm.dialogueEnd());
            masterConvo1Done = true;
        }
        else
        {
            StartCoroutine(masterCombatConvo2());
        }
    }

}
