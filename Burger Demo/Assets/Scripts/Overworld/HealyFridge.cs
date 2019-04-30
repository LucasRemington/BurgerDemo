using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealyFridge : MonoBehaviour
{
    private BattleTransitions battTran; // Battle transitions, where player health and ingredients are stored.
    private NarrativeManager narrMan; // Narrative manager, used for dialogue shenanigans.
    private DialogueHolder diaHold; // Dialogue holder, see above.

    [Tooltip("Whether this fridge has already been used or not. Accessed by InteractDia.")] public bool used;
    [Tooltip("The amount of Meat that the locker should heal for.")] public int amountToHeal;
    [Tooltip("Ingredients restored are given in a random range; this is the minimum.")] public int ingMin;
    [Tooltip("Ingredients restored are given in a random range; this is the maximum.")] public int ingMax;
    [Tooltip("The cap of ingredients that should be given back; ingredients cannot go over this value via these lockers.")] public int ingCap;

    [Tooltip("The dialogue used for refusing to use the fridge.")] public Dialogue refuseDialogue;


    void Start ()
    {
        battTran = GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleTransitions>();
        narrMan = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<NarrativeManager>();
        diaHold = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DialogueHolder>();
	}

    // Called whenever we interact with a fridge! Restore ingredients slightly and restore health slightly.
    public IEnumerator Fridge(Dialogue dia)
    {
        // Wait until we've selected a choice. This is whether or not we use the meat locker to begin with. 1 = yes, 2 = no.
        yield return new WaitUntil(() => narrMan.choiceMade);
        if (narrMan.choiceSelected == 1)
        {
            Debug.Log("Yes to fridge");
            // Heal us a bit, but don't go over our max.
            battTran.playerHealth += amountToHeal;
            if (battTran.playerHealth > battTran.playerHealthMax)
                battTran.playerHealth = battTran.playerHealthMax;

            // Add back to our ingredients! Don't go over our cap.
            for (int i = 0; i < battTran.ingredients.Length; i++)
            {
                if (battTran.ingUnlocked[i])
                {
                    int rand = Random.Range(ingMin, ingMax + 1); // Having this as a separate line forces the random range to be an integer as opposed to a float. Max is exclusive so we add one to it.
                    battTran.ingredients[i] += rand;

                    if (battTran.ingredients[i] > ingCap)
                        battTran.ingredients[i] = ingCap;
                }
            }

            used = true;
        }
        else if (narrMan.choiceSelected == 2)
        {
            Debug.Log("No to fridge");
            // We cancel the current dialogue and replace it with a new "You refused" piece of dialogue.
            diaHold.CancelDialogue(false);
            yield return new WaitUntil(() => narrMan.dbChoiceSS);
            diaHold.StartCoroutine(diaHold.GenericInteractableNew(refuseDialogue, this.gameObject, false));
            narrMan.choiceSelected = 0;
            gameObject.GetComponent<InteractDia>().i -= 1; // If we refuse, instead of going to the "this fridge is empty" dialogue on use, reset the dialogue tracker.
        }
        else
        {
            Debug.Log("Invalid choice number!");
        }

        /* // This is all stuff assuming we have a second option; we do not, which makes this simple.
        // Wait until we've selected a second choice. This determines fast travel. Alternatively, skip this if we need to end now.
        yield return new WaitUntil(() => !narrMan.choiceMade || end);
        yield return new WaitUntil(() => narrMan.choiceMade || end);
        if (narrMan.choiceSelected == 1)
        {

        }
        else if (narrMan.choiceSelected == 2)
        {
            diaHold.CancelDialogue(true);
            yield return new WaitUntil(() => narrMan.dbChoiceSS);
            //diaHold.StartCoroutine(diaHold.GenericInteractableNew(refuseDialogue));
        }
        */
        yield return null;
    }
}
