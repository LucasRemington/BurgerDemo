using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTransitions : MonoBehaviour {

    [Header("Found via Script")]
    [Tooltip("The battle prefab, found every time a new room loads.")] public GameObject battlePrefab;
    [Tooltip("Any overworld objects in the scene; originally hidden when battle started. No longer the case. Redundant?")] public GameObject[] OverworldObjects;
    [Tooltip("The main camera within the scene.")] public GameObject MainCamera;
    [Tooltip("The narrative manager within the scene.")] public NarrativeManager nm;
    [Tooltip("Dialogue box.")] public GameObject db; // dialogue box
    [Tooltip("Used to track the position where the enemy should start attacking from in combat.")] public GameObject enemyStart;
    [Tooltip("The burger component instantiator script found within the scene.")] public BurgerComponentInstantiator bci;
    private GameObject burgSpawner;

    [Header("Found in Battle")]
    [Tooltip("The enemy used in the argument for starting battle.")] public GameObject currentEnemy;
    private bool isCurrentEnemyScripted;
    [Tooltip("The current battle scene.")] public GameObject battle;
    [Tooltip("The combat player, found when battle starts.")] public GameObject player;
    [Tooltip("The player health script attached to the combat player.")] public PlayerHealth ph;
    [Tooltip("The text component for floating text relevant to the player.")] public Text floatingPlayerText;
    [Tooltip("The text component for floating text relevant to the enemy.")] public Text floatingEnemyText;

    [Header("Player Stats & Progression")]
    [Tooltip("The player's current health. When it reaches 0, the player dies and respawns at either the last checkpoint or the last meat locker they visited.")] public int playerHealth = 60;
    [Tooltip("The player's maximum health. The player will gain more as they progress through the game.")] public int playerHealthMax = 60;
    [Tooltip("The ingredients that the player currently has in stock. Set in the editor for the player starting a new game.\nIdentities: 0:Blank; 1:Tomato; 2:Lettuce; 3:Onion; 4:Bacon; 5:Sauce; 6:Pickles; 7:Ketchup; 8:Mustard; 9:Cheese; 10:Patty")] public int[] ingredients;
    [Tooltip("Ingredients unlocked. Check above tooltip for what each index corresponds to. Also add 11:Trash.")] public bool[] ingUnlocked;
    [Tooltip("Combos unlocked by the player. For now, size 21. Combos unlock after performing them at least once, except for the Classic Combo which is unlocked immediately.")] public bool[] combosUnlocked;
    [Tooltip("Items that the player has unlocked.")] public bool[] itemsUnlocked;
    [Tooltip("Items that the player currently has.")] public int[] itemCount;
    
    [Header("Set in Editor")]
    [Tooltip("The battle intro animation! Set via editor, as if this header didn't tell you so already. Why are you still reading this. Stop that.")] public GameObject battleIntro;

    public Image BlackScreen;
    

    [HideInInspector] public bool battling = false;

    // Use this for initialization; called in every new room.
    public void PseudoStart () {
        Debug.Log("Pseudostart: BattleTransitions.cs");
        battlePrefab = GameObject.FindWithTag("BattlePrefab");
        battlePrefab = battlePrefab.transform.Find("FullBattlePrefab").gameObject;
        OverworldObjects = GameObject.FindGameObjectsWithTag("Overworld");
        MainCamera = GameObject.FindWithTag("MainCamera");
        nm = MainCamera.GetComponent<NarrativeManager>();
        db = GameObject.Find("DialogBox");
        enemyStart = battlePrefab.transform.Find("EnemyStart").gameObject;
        burgSpawner = MainCamera.transform.Find("Canvas").gameObject;
        burgSpawner = burgSpawner.transform.Find("CombatUI").gameObject;
        burgSpawner = burgSpawner.transform.Find("BurgerSpawner").gameObject;
        bci = burgSpawner.GetComponent<BurgerComponentInstantiator>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Delete))
            Destroy(battle);

        if (playerHealth > playerHealthMax)
        {
            playerHealth = playerHealthMax;
        }
	}

    public IEnumerator StartBattle(GameObject enemy, bool isScripted) {
        bci.ingredientINV = ingredients;
        currentEnemy = enemy;
        isCurrentEnemyScripted = isScripted;
        battleIntro.SetActive(true);
        battleIntro.GetComponent<Animator>().Play("Intro1");
        StartCoroutine(nm.combatUIOn());
        yield return new WaitForSeconds(0.3f);
        battling = true;
        yield return new WaitForSeconds(0.1f);
        battlePrefab.SetActive(true);
        //battlePrefab.transform.parent = null;
        //GetComponent<ActionSelector>().isReady = true; // This line changed in my version! Consult Matt!
        if (enemy.GetComponent<Enemy>())
        {
            battle = Instantiate(enemy.GetComponent<Enemy>().battlePrefab, enemyStart.transform.position, new Quaternion(0, 0, 0, 0), this.transform);
        }
        else
        {
            battle = Instantiate(enemy, enemyStart.transform.position, new Quaternion(0, 0, 0, 0), this.transform);
        }
        for (int i = 0; i < OverworldObjects.Length; i++) {
            //OverworldObjects[i].SetActive(false);
        }
        yield return new WaitForSeconds(0.1f);
        
        //Instantiate(battlePrefab, this.gameObject.transform);
        //battle = GameObject.Find("FullBattlePrefab(Clone)");
        //battle.transform.parent = null;
        
        player = GameObject.Find("FullBattlePrefab").transform.GetChild(0).gameObject;
        ph = player.GetComponent<PlayerHealth>();
        //Instantiate(enemy.GetComponent<Enemy>().battlePrefab, this.transform);
        battling = true;
        yield return new WaitForSeconds(0.4f);
        Debug.Log("battle intro should be gone");
        battleIntro.SetActive(false);
        yield return new WaitUntil(()=> bci.gameObject.activeInHierarchy);
        StartCoroutine(bci.StartStuff());
    }

    public IEnumerator EndOfTutorialBattle(bool win)            // Called by narrativescript1.
    {
        bci.ResetBattleSystem();
        yield return new WaitUntil(() => nm.ns1.blackScreen.color.a >= 1); // Wait until we fully fade to black.
        //yield return new WaitForSeconds(1.5f);

        ingredients = bci.ingredientINV; // Reset our ingredient inventory.

        /*for (int i = 0; i < OverworldObjects.Length; i++) {
            OverworldObjects[i].SetActive(true);
        }*/

        yield return new WaitForSeconds(1); // Wait a moment, then move camera back, restore player health, and allow the player to continue.

        //ph.healthUpdate();
        battlePrefab.transform.SetParent(MainCamera.transform);
        player = GameObject.Find("FullBattlePrefab").transform.GetChild(0).gameObject;
        ph = player.GetComponent<PlayerHealth>();
        playerHealth = ph.playerHealth = ph.playerHealthMax;
        if (playerHealth > playerHealthMax)
            playerHealth = playerHealthMax;
        nm.ns1.winLossText.text = "Press the Action Button to continue...";
        /*for (int i = 0; i < 101; i++)
        {
            nm.ns1.blackScreen.color = new Color(0, 0, 0, nm.ns1.blackScreen.color.a - 0.01f);
            yield return new WaitForEndOfFrame();
        }*/
        yield return new WaitUntil(() => Input.GetButtonDown("Submit") && !nm.ns1.waitForScript); // Wait until the player hits Space and we aren't waiting for script.

        Destroy(battle.gameObject);
        
        //GameObject thing = GameObject.Find("FullBattlePrefab");
        battlePrefab.SetActive(false);
        battling = false;
        nm.combatUI.SetActive(false);
        currentEnemy = null;
        yield return new WaitForSeconds(0.2f);
        db.GetComponent<Animator>().ResetTrigger("Popdown");
    }

    public IEnumerator EndOfBattle(bool win)            //this gets called by the enemy's death in enemyBehavior
    {
        bci.ResetBattleSystem();
        yield return new WaitForSeconds(1.5f);
        ingredients = bci.ingredientINV;
        for (int i = 0; i < OverworldObjects.Length; i++)
        {
            OverworldObjects[i].SetActive(true);
        }
        BlackScreen = GameObject.FindGameObjectWithTag("BlackScreen").GetComponent<Image>();
        StartCoroutine(nm.bci.FadeImageToFullAlpha(1.5f, BlackScreen));
        BlackScreen.color = Color.black;
        yield return new WaitForSeconds(1);
        //ph.healthUpdate();
        battling = false;
        battlePrefab.transform.SetParent(MainCamera.transform);
        player = GameObject.Find("FullBattlePrefab").transform.GetChild(0).gameObject;
        ph = player.GetComponent<PlayerHealth>();
        playerHealth = ph.playerHealth + 20;
        if (playerHealth > playerHealthMax)
            playerHealth = playerHealthMax;
        nm.ns1.winLossText.text = "";

        if (win)
            nm.BattleWon = true;
        else
            nm.BattleWon = false;

        nm.BattleDone = true;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) /*&& !nm.owm.canMove*/);
        nm.owm.canMove = true;
        if (win && !isCurrentEnemyScripted)
        {
            Destroy(currentEnemy.gameObject);
        }
        else {
            SaveLoad saveLoad = GameObject.FindGameObjectWithTag("Base").GetComponent<SaveLoad>();
            saveLoad.StartCoroutine(saveLoad.LoadGame(true));
        }
        battle.SetActive(false);                         // this is the line that breaks it
        //GameObject thing = GameObject.Find("FullBattlePrefab");
        battlePrefab.SetActive(false);
        nm.combatUI.SetActive(false);
        currentEnemy = null;
        StartCoroutine(nm.bci.FadeImageToZeroAlpha(1.5f, BlackScreen));
        BlackScreen.color = new Color(0, 0, 0, 0);
        yield return new WaitForSeconds(0.2f);
        db.GetComponent<Animator>().ResetTrigger("Popdown");
    }
}
