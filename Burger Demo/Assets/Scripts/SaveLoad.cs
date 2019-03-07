using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveLoad : MonoBehaviour
{
    [Tooltip("All scripts that need to be saved; these should all be found within Don't Destroy On Load, and should be assigned in the editor.")] public List<MonoBehaviour> scripts;
    private string dataPath; // Where the save file is kept locally.
    private string currentScene; // The scene currently loaded within savedata.

    [HideInInspector] [Tooltip("The main camera, which other components are derived from. If this is empty at runtime, that is an issue.")] public GameObject mainCam;
    private NarrativeManager narrMan;
    private DialogueHolder diaHold;
    private GameObject gameController;
    private BattleTransitions battTran;

    [HideInInspector] [Tooltip("The player holder, which other components are derived from. Set via script. If this is empty at runtime, that is an issue.")] public GameObject playerHolder;
    private GameObject player;
    private Animator playerAnim;

    [HideInInspector] public SaveData saveData; // The serializable save data for the game.

    [Tooltip("The black screen UI image, used in fading in and out. We may not use it that much. Set via script.")] public Image blackScreen;
    [Tooltip("Checks if we are ready to fade again.")] public bool readyForFade;
    [Tooltip("Checks if the image has fully faded out or not.")] public bool faded;

    [Tooltip("The dialogue used for refusing to use the meat locker.")] public Dialogue refuseDialogue;

    [Tooltip("Used for debugging. diaIndex and convoIndex respectively. Duntouch.")] public int dI, cI;

        // These were previously in just SaveData, but it'll probably work better if these are edited and tracked here rather than directly touching savedata.
    public List<string> meatLockerList; // A list of scene names that the player has visited the Meat Locker of. As new meat lockers are discovered, their scene is stored at an index that can be read off of later.
    [HideInInspector] public int meatLockerIndex; // Tracks the last meat locker the player visited by its index in the above list, so they can respawn there via menu.
    [HideInInspector] public Vector2 meatLockerPos; // Similarly, tracks the transform of the last meat locker visited within the scene.


    void Start()
    {
        //Get ready to assign some shit
        mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        playerHolder = GameObject.FindGameObjectWithTag("Player");
        gameController = GameObject.FindGameObjectWithTag("GameController");
        battTran = gameController.GetComponent<BattleTransitions>();
        /*for (int i = 0; i < scripts.Count; i++)
        {
            dataPaths.Add( Path.Combine(Application.persistentDataPath, "porygon_" + scripts[i].name + ".txt") );
        }*/
        dataPath = Path.Combine(Application.persistentDataPath, "porygon.txt");

        narrMan = mainCam.GetComponent<NarrativeManager>();
        diaHold = mainCam.GetComponent<DialogueHolder>();

        player = playerHolder.transform.GetChild(0).gameObject;
        playerAnim = player.GetComponent<Animator>();

        blackScreen = GameObject.FindGameObjectWithTag("BlackScreen").GetComponent<Image>();

    }

    public void Update()
    {
        // Using this to test Loading in particular.
        if (Input.GetKeyDown(KeyCode.L) && player.GetComponent<OverworldMovement>().canMove)
        {
            Debug.Log("Loading from last checkpoint.");
            StartCoroutine(LoadGame(false));
        }

        if (Input.GetKeyDown(KeyCode.K) && player.GetComponent<OverworldMovement>().canMove)
        {
            Debug.Log("Loading from last meat locker.");
            StartCoroutine(LoadGame(true));
        }
    }

    // Called whenever we enter a meatlocker. Determines conversation choice order, saves the game, refills health, and offers fast travel...eventually.
    public IEnumerator MeatLockerEvent(Dialogue dia)
    {
        bool end = false;

        // Wait until we've selected a choice. This is whether or not we use the meat locker to begin with. 1 = yes, 2 = no.
        yield return new WaitUntil(() => narrMan.choiceMade);
        if (narrMan.choiceSelected == 1)
        {
            // Before saving the game, restore our health. Then, check our meat locker list; if this meat locker has not yet been used, add it to the list. Either way, adjust our last visited meat locker to its index.
            battTran.playerHealth = battTran.playerHealthMax;

            if (!meatLockerList.Contains(SceneManager.GetActiveScene().name))
            {
                meatLockerList.Add(SceneManager.GetActiveScene().name);
            }

            for (int i = 0; i < meatLockerList.Count; i++)
            {
                if (meatLockerList[i] == SceneManager.GetActiveScene().name)
                {
                    meatLockerIndex = i;
                    meatLockerPos = player.transform.position;
                    i = meatLockerList.Count;
                }
            }

            // Add ingredients we're missing here. Break the loop if we don't have ingredients past a certain point unlocked.
            for (int i = 0; i < battTran.ingredients.Length; i++)
            {
                if (battTran.ingredients[i] == 0)
                {
                    battTran.ingredients[i] = 1;
                }

                if (!battTran.ingRowTwoUnlocked && i > 3)
                {
                    break;
                }

                if (!battTran.ingRowThreeUnlocked && i > 6)
                {
                    break;
                }

            }

            // We want to play the animation for climbing in here; We'll need to make it so that dialogue can't progress until the animation ends and saving is done.

            //Okay, actually save once we're inside the meat locker.
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
            StartCoroutine(LoadGame(false));
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
        Debug.Log("Saving game!");
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

    public void LoadGameFunc(bool gotoLocker)
    {
        StartCoroutine(LoadGame(gotoLocker));
    }

    public IEnumerator LoadGame(bool gotoLocker)
    {
        // Fade to black first and make sure we can't move.
        player.GetComponent<OverworldMovement>().canMove = false;
        StartCoroutine(FadeImageToFullAlpha(1.5f, blackScreen));
        yield return new WaitUntil(() => faded);

        bool done = false;

        // Actually load data!
        Debug.Log("Loading...");
        using (StreamReader streamReader = File.OpenText(dataPath))
        {
            string jsonString = streamReader.ReadToEnd();
            saveData = JsonUtility.FromJson<SaveData>(jsonString);
            done = LoadData();
        }
        yield return new WaitUntil(() => done);

        // Now, if this loads us at our last meat locker, we need to load the scene at the last meat locker's index, and grab its transform.
        if (gotoLocker)
        {
            SceneManager.LoadScene(saveData.meatLockerList[saveData.meatLockerIndex]);
            player.transform.position = saveData.meatLockerPos;
        }

        // Fade back in. We wait until it's done doing so before we can move; we can change this to an animation trigger instead later on.

        player.SetActive(true);
        StartCoroutine(FadeImageToZeroAlpha(1.5f, blackScreen));
        yield return new WaitUntil(() => blackScreen.color.a <= 0);
        
        player.GetComponent<OverworldMovement>().canMove = true;
    }

    // Update the information in our save data.
    public bool UpdateSaveData()
    {
        saveData.currentScene = SceneManager.GetActiveScene().name;

        saveData.currentHealth = battTran.playerHealth;
        saveData.healthMax = battTran.playerHealthMax;
        saveData.convoDone = diaHold.scriptedConvoDone;
        saveData.currentPos = player.transform.position;

        saveData.ingredients = battTran.ingredients;
        saveData.ingRowTwoUnlocked = battTran.ingRowTwoUnlocked;
        saveData.ingRowThreeUnlocked = battTran.ingRowThreeUnlocked;


        saveData.meatLockerList = meatLockerList;
        saveData.meatLockerIndex = meatLockerIndex;
        saveData.meatLockerPos = meatLockerPos;
        saveData.gameStarted = narrMan.gameStarted;
        

        return true;
    }

    public bool LoadData()
    {
        if (SceneManager.GetActiveScene().name != saveData.currentScene)
        {
            SceneManager.LoadScene(saveData.currentScene);
        }

        battTran.playerHealth = saveData.currentHealth;
        battTran.playerHealthMax = saveData.healthMax;
        diaHold.scriptedConvoDone = saveData.convoDone;
        player.transform.position = saveData.currentPos;

        battTran.ingredients = saveData.ingredients;
        battTran.ingRowThreeUnlocked = saveData.ingRowThreeUnlocked;
        battTran.ingRowTwoUnlocked = saveData.ingRowTwoUnlocked;

        meatLockerList = saveData.meatLockerList;
        meatLockerIndex = saveData.meatLockerIndex;
        meatLockerPos = saveData.meatLockerPos;
        narrMan.gameStarted = saveData.gameStarted;

        return true;
    }

    public IEnumerator FadeImageToFullAlpha(float t, Image i) // Used to fade out the screen and mask loading data.
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        faded = false;
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
        if (i.color.a >= 1.0f && faded == false)
        {
            //StartCoroutine(whenBlackScreen());
            faded = true;
            yield return null;
        }
    }

    public IEnumerator FadeImageToZeroAlpha(float t, Image i)
    {
        yield return new WaitForSeconds(1f);
        //yield return new WaitUntil(() => readyForFade == true);
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }

    }

    /*
    public IEnumerator CheckpointLoad()
    {
        pHealth.playerHealth = saveData.currentHealth;
        pHealth.playerHealthMax = saveData.healthMax;
        diaHold.scriptedConvoDone = saveData.convoDone;
        player.transform.position = saveData.currentPos;

        battTran.ingredients = saveData.ingredients;
        battTran.ingRowThreeUnlocked = saveData.ingRowThreeUnlocked;
        battTran.ingRowTwoUnlocked = saveData.ingRowTwoUnlocked;
        yield return null;
    }

    public IEnumerator CheckpointSave()
    {
        saveData.currentHealth = pHealth.playerHealth;
        saveData.healthMax = pHealth.playerHealthMax;
        saveData.convoDone = diaHold.scriptedConvoDone;
        saveData.currentPos = player.transform.position;

        saveData.ingredients = battTran.ingredients;
        saveData.ingRowTwoUnlocked = battTran.ingRowTwoUnlocked;
        saveData.ingRowThreeUnlocked = battTran.ingRowThreeUnlocked;

        yield return null;
    }*/
}
