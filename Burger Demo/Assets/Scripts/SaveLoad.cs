using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveLoad : MonoBehaviour
{
    [Tooltip("All scripts that need to be saved; these should all be found within Don't Destroy On Load, and should be assigned in the editor.")] public List<MonoBehaviour> scripts;
    //private List<string> dataPaths;
    private string dataPath, sceneDataPath;
    private string currentScene;

    [HideInInspector] [Tooltip("The main camera, which other components are derived from. If this is empty at runtime, that is an issue.")] public GameObject mainCam;
    private NarrativeManager narrMan;
    private DialogueHolder diaHold;
    private GameObject gameController;
    private PlayerHealth pHealth;

    [HideInInspector] [Tooltip("The player holder, which other components are derived from. Set via script. If this is empty at runtime, that is an issue.")] public GameObject playerHolder;
    private GameObject player;
    private Animator playerAnim;

    [HideInInspector] public SaveData saveData;

    [Tooltip("The black screen UI image, used in fading in and out. We may not use it that much. Set in editor. I'll try to make it so we don't need to later.")] public Image blackScreen;

    [Tooltip("The dialogue used for refusing to use the meat locker.")] public Dialogue refuseDialogue;

    [Tooltip("Used for debugging. diaIndex and convoIndex respectively.")] public int dI, cI;
    //public string name;




    void Start()
    {
        //Get ready to assign some shit
        mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        playerHolder = GameObject.FindGameObjectWithTag("Player");
        gameController = GameObject.FindGameObjectWithTag("GameController");
        pHealth = gameController.GetComponent<BattleTransitions>().ph;

        /*for (int i = 0; i < scripts.Count; i++)
        {
            dataPaths.Add( Path.Combine(Application.persistentDataPath, "porygon_" + scripts[i].name + ".txt") );
        }*/
        dataPath = Path.Combine(Application.persistentDataPath, "porygon.txt");
        sceneDataPath = Path.Combine(Application.persistentDataPath, "porygon2.txt");

        narrMan = mainCam.GetComponent<NarrativeManager>();
        diaHold = mainCam.GetComponent<DialogueHolder>();

        player = playerHolder.transform.GetChild(0).gameObject;
        playerAnim = player.GetComponent<Animator>();
    }

    // Called whenever we enter a meatlocker. Determines conversation choice order, saves the game, refills health, and offers fast travel...eventually.
    public IEnumerator MeatLockerEvent(Dialogue dia)
    {
        bool end = false;

        // Wait until we've selected a choice. This is whether or not we use the meat locker to begin with. 1 = yes, 2 = no.
        yield return new WaitUntil(() => narrMan.choiceMade);
        if (narrMan.choiceSelected == 1)
        {
            //Save our game, restore health, and reset our checkpoint. If an ingredient has run out, add one to it. 
            pHealth.playerHealth = (int)pHealth.playerHealthMax;
            saveData.checkpointIndex = 0;

            // We want to play the animation for climbing in here; We'll need to make it so that dialogue can't progress until the animation ends and saving is done.

            StartCoroutine(SaveGame());
        }
        else if (narrMan.choiceSelected == 2)
        {
            // We cancel the current dialogue and replace it with a new "You refused" piece of dialogue.
            diaHold.CancelDialogue(false);
            yield return new WaitUntil(() => narrMan.dbChoiceSS);
            diaHold.StartCoroutine(diaHold.GenericInteractableNew(refuseDialogue));
            end = true;
            narrMan.choiceSelected = 0;
        }

        // Wait until we've selected a second choice. This determines fast travel. Alternatively, skip this if we need to end now.
        yield return new WaitUntil(() => !narrMan.choiceMade || end);
        yield return new WaitUntil(() => narrMan.choiceMade || end);
        if (narrMan.choiceSelected == 1)
        {
            // For now, this loads our game.
            StartCoroutine(LoadGame());
        }
        else if (narrMan.choiceSelected == 2)
        {
            diaHold.CancelDialogue(true);
            yield return new WaitUntil(() => narrMan.dbChoiceSS);
            //diaHold.StartCoroutine(diaHold.GenericInteractableNew(refuseDialogue));
        }
        yield return null;
    }

    public IEnumerator SaveGame()
    {
        bool done = false;
        done = UpdateSaveData();
        yield return new WaitUntil(() => done);
        string jsonString = JsonUtility.ToJson(saveData);

        using (StreamWriter streamWriter = File.CreateText(dataPath))
        {
            streamWriter.Write(jsonString);
        }

        yield return null;
    }

    public IEnumerator CheckpointSave()
    {
        saveData.currentHealth = pHealth.playerHealth;
        saveData.healthMax = pHealth.playerHealthMax;
        saveData.convoDone = diaHold.scriptedConvoDone;
        saveData.currentPos = player.transform.position;

        yield return null;
    }

    public IEnumerator LoadGame()
    { 
        using (StreamReader streamReader = File.OpenText(dataPath))
        {
            string jsonString = streamReader.ReadToEnd();
            saveData = JsonUtility.FromJson<SaveData>(jsonString);
            bool done = false;
            done = AssignSaveData();
            yield return new WaitUntil(() => done);
        }
        yield return null;
    }

    public bool UpdateSaveData()
    {
        saveData.currentScene = SceneManager.GetActiveScene().name;

        saveData.currentHealth = pHealth.playerHealth;
        saveData.healthMax = pHealth.playerHealthMax;
        saveData.convoDone = diaHold.scriptedConvoDone;
        saveData.currentPos = player.transform.position;

        return true;
    }

    public bool AssignSaveData()
    {
        //SceneManager.LoadScene(saveData.currentScene);

        pHealth.playerHealth = saveData.currentHealth;
        pHealth.playerHealthMax = saveData.healthMax;
        diaHold.scriptedConvoDone = saveData.convoDone;
        player.transform.position = saveData.currentPos;
        return true;
    }

    public IEnumerator CheckpointLoad()
    {
        yield return null;
    }
}
