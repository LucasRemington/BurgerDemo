using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMenuIngCaller : MonoBehaviour
{

    bool toggle;
    public bool isTutorial;

    void BackHome()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<ActionSelector>().backHome = true;
    }

    void IsReady()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<ActionSelector>().isReady = true;
    }

    public void ToggleText()
    {
        Text[] txt = GetComponentsInChildren<Text>();
        Outline[] otl = GetComponentsInChildren<Outline>();

        if (toggle) // Search all children for any Text and Outline components. If they exist, toggle them between enabled and disabled. Called via animation.
        {
            for (int i = 0; i < txt.Length; i++)
            {
                if (txt[i])
                    txt[i].enabled = false;
                if (otl[i])
                    otl[i].enabled = false;
            }

            toggle = false;
        }
        else
        {
            for (int i = 0; i < txt.Length; i++) // When we toggle text on, don't do so for the control text if it's not during a tutorial fight.
            {
                if (txt[i] && ((txt[i].gameObject.name == "ControlText" /*&& isTutorial*/) || txt[i].gameObject.name != "ControlText")) // Ergo, we make sure that the text component exists: in addition to that, in order to be enabled it has to either not be control text or needs to be control text during the tutorial.
                    txt[i].enabled = true;
                if (otl[i] && ((otl[i].gameObject.name == "ControlText" /*&& isTutorial*/) || otl[i].gameObject.name != "ControlText"))
                    otl[i].enabled = true;
            }

            toggle = true;
        }
    }

    void Unready() // Called when ingredients start to spawn in or disappear, and when certain menus open, preventing us from selecting things.
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<ActionSelector>().isReady = false;
    }
}
