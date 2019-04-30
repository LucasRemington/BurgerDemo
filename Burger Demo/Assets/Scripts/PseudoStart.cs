using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PseudoStart : MonoBehaviour {

    [Header("Set In Inspector")]
    //[Tooltip("The identity of the room for narrative purposes. Set in inspector. (0 = OriginFreezer, 1 = Dennis office, 2 = Training Hallway, 3 = Training Room, 4 = Kitchen. Menu doesn't have any, set it to -1.)")] public int Identity;
    [Tooltip("Minimum bounds for camera movement within the given scene. Input in the inspector.")] public Vector3 minCameraPos;
    [Tooltip("Maximum bounds for camera movement within the given scene. Input in the inspector.")] public Vector3 maxCameraPos;

    [Header("Player")]
    [Tooltip("The player holder, of which the player is a child of even when disabled. Found via script.")] public GameObject playerHolder;
    [Tooltip("The player object, found via script.")] public GameObject player;
    [Tooltip("The OverworldMovement script attached to the player. Found via script.")] public OverworldMovement ovm;

    [Header("Camera")]
    [Tooltip("The camera within the scene. Found in script.")] public GameObject MainCamera;
    [Tooltip("The FollowPlayer script that is attached to the main camera. Found in script.")] public FollowPlayer fp;

    [Header("Controllers")]
    [Tooltip("GameController object, found via script.")] public GameObject GameController;
    [Tooltip("The NarrativeManager, attached to the main camera. Found in script.")] public NarrativeManager nm;
    [Tooltip("The NarrativeScript1 script, attached to the Main Camera. Found via script.")] public NarrativeScript1 ns1;
    [Tooltip("DialogueHolder script attached to the Main Camera. Found via script.")] public DialogueHolder dh;
    [Tooltip("The Battle Transitions script, attached to the GameController. Found in script.")] public BattleTransitions bt;    
    [Tooltip("The BurgerComponentInstantiator (BCI) script attached to the BurgerSpawner game object. Found via script via transforms.")] public BurgerComponentInstantiator BCI;

    void Start ()
    {
        Cursor.visible = false; // Disable cursor immediately.

        // Find our base objects of the scene that other objects derive from: MainCamera, PlayerHolder, GameController. 
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        playerHolder = GameObject.FindGameObjectWithTag("Player");
        GameController = GameObject.FindWithTag("GameController");

        // Variables that derive from MainCam.
        nm = MainCamera.GetComponent<NarrativeManager>();
        ns1 = MainCamera.GetComponent<NarrativeScript1>();
        dh = MainCamera.GetComponent<DialogueHolder>();
        fp = MainCamera.GetComponent<FollowPlayer>();
        BCI = MainCamera.transform.Find("Canvas").Find("CombatUI").Find("BurgerSpawner").GetComponent<BurgerComponentInstantiator>();

        // Player-derived variables.
        player = playerHolder.transform.Find("OverworldPlayer").gameObject;
        ovm = player.GetComponent<OverworldMovement>();

        // Miscellaneous variables.
        bt = GameController.GetComponent<BattleTransitions>();

        // Camera stuff. Set min and max bounds in FollowPlayer, and then set the default position of the camera for the main menu.
        fp.minCameraPos = minCameraPos;
        fp.maxCameraPos = maxCameraPos;
        MainCamera.transform.position = new Vector3(-2f, -1f, -5);


        // Call the pseudostart functions within each script. This essentially allows the pseudostart function to act as a Start function in every new room.
        nm.PseudoStart();
        ns1.PseudoStart();
        dh.PseudoStart();
        fp.PseudoStart();
        bt.PseudoStart();

        nm.room = SceneManager.GetActiveScene().buildIndex - 1; // Now grab build index minus one; so the starting freezer is room 0 and main menu is room -1.

        //nm.room = Identity; // Our previous temp solution.
        ovm.PseudoStart();
        StartCoroutine(BCI.StartStuff());
    }
}
