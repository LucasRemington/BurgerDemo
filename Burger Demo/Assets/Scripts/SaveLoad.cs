using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveLoad : MonoBehaviour
{
    [Tooltip("All scripts that need to be saved; these should all be found within Don't Destroy On Load, and should be assigned in the editor.")] public List<MonoBehaviour> scripts;
    private string dataPath, sceneDataPath;
    private string currentScene;

    [Tooltip("The main camera, which other components are derived from. If this is empty at runtime, that is an issue.")] public GameObject mainCam;
    private NarrativeManager narrMan;
    private DialogHolder diaHold;

    [Tooltip("The player holder, which other components are derived from. Set via script. If this is empty at runtime, that is an issue.")] public GameObject playerHolder;
    private GameObject player;
    private Animator playerAnim;

    [Tooltip("The black screen UI image, used in fading in and out. We may not use it that much. Set in editor. I'll try to make it so we don't need to later.")] public Image blackScreen;

    [Tooltip("The dialogue used for refusing to use the meat locker.")] public Dialogue refuseDialogue;

    [Tooltip("Used for debugging. diaIndex and convoIndex respectively.")] public int dI, cI;
    //public string name;




    void Start()
    {
        mainCam = gameObject;
        playerHolder = GameObject.FindGameObjectWithTag("Player");


        dataPath = Path.Combine(Application.persistentDataPath, "porygon.txt");
        sceneDataPath = Path.Combine(Application.persistentDataPath, "porygon_2.txt");

        narrMan = mainCam.GetComponent<NarrativeManager>();
        diaHold = mainCam.GetComponent<DialogHolder>();

        player = playerHolder.transform.GetChild(0).gameObject;
        playerAnim = player.GetComponent<Animator>();
    }


    void Update()
    {
        if (currentScene != SceneManager.GetActiveScene().name)
            currentScene = SceneManager.GetActiveScene().name;
    }

    public IEnumerator MeatLockerEvent(Dialogue dia)
    {
        bool end = false;
        // Wait until we've selected a choice. This is whether or not we use the meat locker to begin with. 1 = yes, 2 = no.
        yield return new WaitUntil(() => narrMan.choiceMade);
        if (narrMan.choiceSelected == 1)
        {
            //Debug.Log("Get in meat locker! Do stuff here!");
        }
        else if (narrMan.choiceSelected == 2)
        {
            //Debug.Log("How about we don't use the meat locker, then?");
            diaHold.CancelDialogue(false);
            //player.GetComponent<OverworldMovement>().canMove = false;
            //narrMan.dbChoiceSS = true;
            yield return new WaitUntil(() => narrMan.dbChoiceSS);
            diaHold.StartCoroutine(diaHold.GenericInteractableNew(refuseDialogue));
            end = true;
            narrMan.choiceSelected = 0;
        }

        // Wait until we've selected a second choice. This determines fast travel.
        yield return new WaitUntil(() => !narrMan.choiceMade || end);
        yield return new WaitUntil(() => narrMan.choiceMade || end);
        if (narrMan.choiceSelected == 1)
        {
            //Debug.Log("We can't actually fast travel yet. Choice 1.");
            //diaHold.CancelDialogue();
        }
        else if (narrMan.choiceSelected == 2)
        {
            //Debug.Log("We can't actually fast travel yet. Choice 2.");
            diaHold.CancelDialogue(true);
            //diaHold.StartCoroutine(diaHold.GenericInteractableNew(refuseDialogue));
        }
        yield return null;
    }
}
