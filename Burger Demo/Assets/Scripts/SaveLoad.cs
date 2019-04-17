using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveLoad : MonoBehaviour
{
    //[Tooltip("All scripts that need to be saved; these should all be found within Don't Destroy On Load, and should be assigned in the editor.")] public List<MonoBehaviour> scripts;
    private string dataPath; // Where the save file is kept locally.
    private string currentScene; // The scene currently loaded within savedata.


    [Header("Save Data")]
    [Tooltip("The serializeable save data for the game. Do NOT touch this; this is for reference purposes only! Or debugging if your name is Matt, James, or I guess theoretically anyone if I'm dropping this egotistical facade...")] public SaveData saveData; // The serializable save data for the game.

    [Header("Components and Objects")]
    [HideInInspector] [Tooltip("The main camera, which other components are derived from. If this is empty at runtime, that is an issue.")] public GameObject mainCam;
    private NarrativeManager narrMan;
    private DialogueHolder diaHold;
    private GameObject gameController;
    private BattleTransitions battTran;

    [Header("UI")]
    [Tooltip("The icon used to indicate that the game is currently saving. This is what rotates.")] public Image saveIcon;
    [Tooltip("The icon used to indicate that the game is currently saving. This is what stays static.")] public Image saveIconText;
    [Tooltip("The black screen UI image, used in fading in and out. We may not use it that much. Set via script.")] public Image blackScreen;
    [Tooltip("Checks if we are ready to fade again.")] public bool readyForFade;
    [Tooltip("Checks if the image has fully faded out or not.")] public bool faded;
    private bool saving = false;

    [Header("Player References")]
    [HideInInspector] [Tooltip("The player holder, which other components are derived from. Set via script. If this is empty at runtime, that is an issue.")] public GameObject playerHolder;
    private GameObject player;
    private Animator playerAnim;

    [Tooltip("The dialogue used for refusing to use the meat locker.")] public Dialogue refuseDialogue;

    //[Tooltip("Used for debugging. diaIndex and convoIndex respectively. Duntouch.")] public int dI, cI;

        // These were previously in just SaveData, but it'll probably work better if these are edited and tracked here rather than directly touching savedata.
    [HideInInspector] public List<string> meatLockerList; // A list of scene names that the player has visited the Meat Locker of. As new meat lockers are discovered, their scene is stored at an index that can be read off of later.
    [HideInInspector] public int meatLockerIndex; // Tracks the last meat locker the player visited by its index in the above list, so they can respawn there via menu.
    [HideInInspector] public Vector2 meatLockerPos; // Similarly, tracks the transform of the last meat locker visited within the scene.

    [Header("Lou Notes")]
    [Tooltip("Each Lou Note present in the game. You should only need to add the first dialogue object that appears each time you read the relevant note; ie, if interacting with the note yields a different dialogue each time, only add the first.")] public List<Dialogue> louNotes; // A list of all louNotes in the game; must be added to manually in the inspector. Its indeces correspond to those in louNotesSeen.
    [Tooltip("Whether each of the above notes have been read; you shouldn't need to touch this.")] public List<bool> louNotesSeen; // A list of bools that tracks which of the above Lou Notes have been read by the player.
    [HideInInspector] public bool louDone; // Whether or not to bother checking and updating Lou notes in the first place.
    private GameObject noteHolder;


    // Makes our foreach loops for Lou stuff more efficient CPU-wise.
    private string lastScene = ""; // The scene the player was in last frame. Refreshed at the end of Update. Prevents us from constantly running through a foreach loop every frame.
    private bool canCheckNewValues; // After a brief timer upon loading into a new scene, this value gets refreshed; this gives objects time to load. 


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

        //Debug.Log(saveIcon);

        if (saveIcon == null)
        {
            saveIcon = GameObject.FindGameObjectsWithTag("SaveIcon")[0].GetComponent<Image>();
            saveIcon.enabled = false;
        }
        if (saveIconText == null)
        {
            saveIconText = GameObject.FindGameObjectsWithTag("SaveIcon")[1].GetComponent<Image>();
            saveIconText.enabled = false;
        }

        // Quickly make sure that our louNotesSeen list has the same length as our manually set list.
        if (louNotesSeen.Count != louNotes.Count)
        {
            for (int i = louNotesSeen.Count; i < louNotes.Count; i++)
            {
                louNotesSeen.Add(false);
            }
        }

        StartCoroutine(SaveSpin());
    }

    public void Update()
    {
        // Used in conjunction with our SaveSpin coroutine; while the icon is enabled, it spins. A while loop was previously used but it could not spin for waitforseconds while simultaneously while saving, which is only for a single frame, actually.
        if (saveIcon.enabled)
        {
            saveIcon.transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * 360);
        }

        // Check if we're in a new scene at the end of update and perform relevant functions.
        if (lastScene != SceneManager.GetActiveScene().name)
        {
            if (!canCheckNewValues)
                StartCoroutine(CanCheckRefreshTimer());

            StartCoroutine(LouNoteCheck(louDone));

            lastScene = SceneManager.GetActiveScene().name;
        }
        
    }

    // Called on each new scene as long as we haven't finished the Lou segment. 
    public IEnumerator LouNoteCheck(bool louDone)
    {
        yield return new WaitForSeconds(0.25f);

        // First, make sure this scene HAS a Lou Note Holder. Then we can do everything.
        if (GameObject.FindGameObjectWithTag("LouNote"))
        { 
            noteHolder = GameObject.FindGameObjectWithTag("LouNote");
            LouNoteHolder noteHolderScript = noteHolder.GetComponent<LouNoteHolder>();

            // A for loop that goes through all of our dialogue components in louNotes. We check to see if a dialogue has been read, and if so, disable the relevant note.
            for (int i = 0; i < louNotesSeen.Count; i++)
            {
                if (louNotesSeen[i]) // So, if Note[i] has been read...
                {
                    Dialogue dia = louNotes[i]; // Set a dialogue variable to Note[i].

                    foreach (GameObject note in noteHolderScript.louNoteList) // Create a foreach loop that iterates through the louNoteList in the note holder.
                    {
                        if (note.GetComponent<LouNoteTrigger>().dia == dia) // Now, if the dialogue of Note[i] is the same as the dialogue in our note holder...
                        {
                            note.SetActive(false); // Disable that note and break the loop!
                            break;
                        }
                    }
                }
            }
        }

        yield return null;
    }

    public IEnumerator CanCheckRefreshTimer()
    {
        yield return new WaitForSeconds(0.75f);
        canCheckNewValues = true;
    }

    // Used in conjunction with a rotation line in Update. Here, we enable the icon, wait for a minimum amount of time, and then disable the icon. This way the player will always see the saving icon, as currently the game saves within a single frame.
    public IEnumerator SaveSpin()
    {
        yield return new WaitUntil(() => saving);
        saveIcon.enabled = true;
        saveIconText.enabled = true;

        yield return new WaitForSeconds(1f);

        yield return new WaitUntil(() => !saving);
        saveIcon.enabled = false;
        saveIconText.enabled = false;

        StartCoroutine(SaveSpin());
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
                if (battTran.ingUnlocked[i] && battTran.ingredients[i] == 0)
                {
                    battTran.ingredients[i] = 1;
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
            diaHold.StartCoroutine(diaHold.GenericInteractableNew(refuseDialogue, this.gameObject, false));
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
        saving = true;
        Debug.Log("Saving game!");
        bool done = false;
        done = UpdateSaveData();
        yield return new WaitUntil(() => done);
        string jsonString = JsonUtility.ToJson(saveData);

        using (StreamWriter streamWriter = File.CreateText(dataPath))
        {
            streamWriter.Write(jsonString);
        }

        saving = false;
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

        narrMan.owm.playerAnim.SetInteger("Health", battTran.playerHealth); // Set a quick animation value so we aren't in a state of slurried meat when we respawn.
        narrMan.owm.playerAnim.SetTrigger("ResetIdle");

        StartCoroutine(FadeImageToZeroAlpha(1.5f, blackScreen));
        yield return new WaitUntil(() => blackScreen.color.a <= 0);
        
        narrMan.owm.canMove = true;
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
        saveData.ingUnlocked = battTran.ingUnlocked;


        saveData.meatLockerList = meatLockerList;
        saveData.meatLockerIndex = meatLockerIndex;
        saveData.meatLockerPos = meatLockerPos;
        saveData.gameStarted = narrMan.gameStarted;

        saveData.narrManEventNo = narrMan.ev;
        saveData.narrManRoomNo = narrMan.room;

        saveData.louNotesSeen = louNotesSeen;
        saveData.louDone = louDone;

        saveData.combosUnlocked = battTran.combosUnlocked;
        saveData.itemsUnlocked = battTran.itemsUnlocked;
        saveData.itemCount = battTran.itemCount;

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
        battTran.ingUnlocked = saveData.ingUnlocked;

        meatLockerList = saveData.meatLockerList;
        meatLockerIndex = saveData.meatLockerIndex;
        meatLockerPos = saveData.meatLockerPos;
        narrMan.gameStarted = saveData.gameStarted;

        narrMan.ev = saveData.narrManEventNo;
        narrMan.room = saveData.narrManRoomNo;

        louNotesSeen = saveData.louNotesSeen;
        louDone = saveData.louDone;

        battTran.combosUnlocked = saveData.combosUnlocked;
        battTran.itemsUnlocked = saveData.itemsUnlocked;
        battTran.itemCount = saveData.itemCount;

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
