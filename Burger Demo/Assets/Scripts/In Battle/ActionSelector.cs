using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionSelector : MonoBehaviour
{
    [Header("Scripts and Objects")]
    [Tooltip("NarrativeScript1, attached to the MainCamera. Attached via script.")] public NarrativeScript1 ns1;
    [Tooltip("The burger spawner gameobject, of which bci is attached to. Set in inspector.")] public GameObject burgerSpawner;
    [Tooltip("BurgerComponentInstantiator, attached to the BurgerSpawner object. Set via script.")] BurgerComponentInstantiator bci;
    [Tooltip("Pwayer, for combat. Found via script.")] public GameObject Player;
    [Tooltip("The GameObject that the combat ui is attached to. Set in the inspector.")] public GameObject combatUI;
    [Tooltip("The Animator component for the combat ui. Found via script.")] public Animator uiAnim;

    [Header("Burger Plate")]
    [Tooltip("The burger plate gameobject, set via inspector.")] public GameObject burgerPlate;
    [Tooltip("The empty transform object for the burger plate when it's active and turned on.")] public Transform plateActivePos;
    [Tooltip("The empty transform object for the burger plate when it's been turned off and is inactive.")] public Transform plateOffPos;
    [Tooltip("Active when the plate has moved to its necessary state.")] public bool plateReady;
    [Tooltip("How long the burger plate should take to move to its new position. Set in the inspector. Value between 0 and 1, with 1 being immediate.")] public float plateMoveTime;

    [Header("Command Menu")]
    [Tooltip("The indicator game object, used in selecting commands in battle.")] public GameObject Indicator;
    [Tooltip("Whether or not the Command indicator should be turned on or off.")] public bool indicatorOn;
    [Tooltip("Commands currently available.")] public bool[] commandsAvailable;
    [Tooltip("An array for where the indicator moves as each option is selected.")] public Vector3[] optionMovements;
    [Tooltip("The Text gameobjects for each command in battle.")] public GameObject[] choiceText;

    [Header("Info Panel Text")]
    [Tooltip("The UI Text component that the info text should be fed into. Set in inspector.")] public Text infoTextSlot;
    [Tooltip("The UI Text component that the combo name should be fed into. Set in inspector.")] public Text comboTextSlot;
    [Tooltip("The two transforms that the combo text should move to. 0 = Combo menu, 1 = BCI Menu")] public Transform[] comboTextSlotLocations;
    [Tooltip("The four string components that each command should apply to the info text slot during tutorial; 0 = Serve, 1 = Combo, 2 = Item, 3 = Run")] public string[] tutCommandInfo;
    [Tooltip("The four string components that each command should apply to the info text slot once the player has truly gained free will.")] public string[] mainCommandInfo;
    [Tooltip("Tooltips displayed for each ingredient during the tutorial fight.")] public StringArray[] tutIngredientInfo;
    [Tooltip("Tooltips displayed for each ingredient outside of the tutorial fight.")] public StringArray[] mainIngredientInfo;
    [Tooltip("Tooltips displayed for each combo. The one in the tutorial is manually set.")] public StringArray[] comboInfoText;
    [Tooltip("The name of each combo that appears in the small panel beneath the list of combos, and upon completing a burger combo. Single-dimension array.")] public string[] comboNameText;
    [Tooltip("Instructional tooltip displayed when a combo is selected.")] public StringArray[] comboOrderText;
    [Tooltip("Tooltips displayed for each item.")] public StringArray[] itemInfoText;

    [Header("Sound")]
    [Tooltip("Menu sound for selecting an option.")] public AudioClip menuConfirm;
    [Tooltip("Menu sound for retreating into a previous menu.")] public AudioClip menuBack;
    [Tooltip("Menu sound for selecting an unselectable option.")] public AudioClip menuError;
    [Tooltip("Menu sound for highlighting a new option.")] public AudioClip menuMove;

    public enum CommandMenus { None, Fight, Combo, Item };
    [Header("Menu Navigation")]
    [Tooltip("The current state of the menu. Behave differently if certain menus are open.")] public CommandMenus commandOpen;
    [Tooltip("Whether any input can be made. Declared by animation events (namely, ingredients being  as well as by narrative scripts.")] public bool isReady = false;
    [Tooltip("Whether or not to display the additional combo panel when the BCI menu is pulled up. On by default, off if tutorial and turned on mid-fight.")] public bool displayBCIComboWindow = true;
    [Tooltip("The column and row that is selected in BCI, item, and combo menus.")] public int col, row; // Col = x, row = y
    [Tooltip("The row that is selected in the command menu.")] public int option;
    [Tooltip("The positions of each ingredient when BCI is pulled up.")] public Vector2Array[] ingredientPositions;
    [Tooltip("The positions of each item and combo when their relevant menus are pulled up. Shared between both unless we have drastically less combos. Appear in a 7x3 grid, so size: 3, then location size: 7.")] public Vector2Array[] comboPositions;
    [Tooltip("Whether or not a combo has been selected; used as an override.")] public bool comboActive;
    [Tooltip("Called from DBAnimEvents, which is attached to the combat UI. Toggles on whenever the combo menu closes, in order to force a brief wait until the BCI menu can open in order to prevent potential menu errors.")] public bool readyUp;
    [Tooltip("Called from DBAnimEvents, which is attached to the combat UI. Toggles on whenever the UI finishes opening the home menu. Prevents us from breaking the ingredients menu.")] public bool backHome = true;
    public bool enemyReset;

    [Header("Sub-Menu Holders")]
    [Tooltip("An array of each Ingredient that appears in the BCI window. Assign them according to their relevant indexes that appear in Battle Transitions.")] public GameObject[] ingredientHolder;
    [Tooltip("The parent object for the combo UI objects.")] public GameObject comboHolder;
    [Tooltip("The parent object for the item UI objects.")] public GameObject itemHolder;

    [Header("Deprecated")]
    [Tooltip("Determines if the commands should be in an 'active' state or if they should be grayed out. Deprecated.")] public bool commandReady = false; // To be deprecated; commandsAvailable now replaces this on a per-command basis.
    [Tooltip("Whether the player can flee from a given fight. Deprecated.")] public bool canRun = false; // Also handled through commandsAvailable.

    private IEnumerator plateMoveInst; // Keeps track of the current instance of the moving burger plate coroutine; allows us to stop the previous one and create a new one so that they don't fight each other over where the plate should be.






    void Start()
    {
        infoTextSlot.text = ""; // Empty our info panel immediately.
        backHome = true;
        ns1 = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<NarrativeScript1>();
        if (!burgerSpawner)
            burgerSpawner = GameObject.FindGameObjectWithTag("BurgerSpawner");
        bci = burgerSpawner.GetComponent<BurgerComponentInstantiator>();
        bci.actSelect = this;
        Player = GameObject.FindGameObjectWithTag("BattlePlayer");
        uiAnim = combatUI.GetComponent<Animator>();

        // At start, we also want to turn off all the Combo and Item icons and their text and outlines.
        ToggleText(comboHolder, false, true);
        ToggleText(itemHolder, false, false);

        

        StartCoroutine(StartStuff());

        enemyReset = true;

        //isReady = true;
        //BCI = GameObject.FindGameObjectWithTag("BurgerSpawner");
    }

    private IEnumerator StartStuff()
    {
        yield return new WaitUntil(() => bci.startupDone); // Refrain from starting up ActionSelector in full until BCI is done setting up.
        if (bci.isTutorial)
        {
            option = -1;
            choiceText[0].GetComponent<Text>().text = "Serve";
            displayBCIComboWindow = false;
        }
        else
        {
            option = 0;
            choiceText[0].GetComponent<Text>().text = "Fight";
        }

        StartCoroutine(MenuNavigation());
        StartCoroutine(Back());
        yield return null;
    }


    void Update()
    {
        // This segment enables any commands that are considered "on" by the array.
        for (int i = 0; i < commandsAvailable.Length; i++)
        {
            if (commandsAvailable[i])
            {
                choiceText[i].GetComponent<Text>().color = Color.white;
            }
            else
            {
                choiceText[i].GetComponent<Text>().color = Color.gray;
            }
        }

        // This segment hides the indicator if it should be off, or turns it back on if the reverse is true.
        if (!indicatorOn)
        {
            Indicator.SetActive(false);
        }
        else
        {
            Indicator.SetActive(true);
        }

    }

    public IEnumerator MenuNavigation()
    {
        yield return new WaitUntil (() => isReady); // Only navigate through menus if we are ready to do so and not in an inbetween state.

        switch (commandOpen)
        {
            case CommandMenus.None: // When the menu is in its base state.

               /* uiAnim.SetBool("ItemCombo", false); // Set all animation bools to false here to return the menu to its base state; the menu must move through here in order to go to other states. Except for from combos to BCI.
                uiAnim.SetBool("ComboInfo", false);
                uiAnim.SetBool("BCI", false);*/

                // Move through the Command menu vertically or select a highlighted option.
                if (Input.GetKeyDown(KeyCode.DownArrow) && option < 3 && ns1.waitForScript == false)
                {
                    option++;
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow) && option > 0 && ns1.waitForScript == false)
                {
                    option--;
                }
                else if (Input.GetButtonDown("Submit") && enemyReset)
                {
                    StartCoroutine(Confirm(option));
                }

                // Reflect the highlighted position with the indicator and change relevant Info panel text. Indicator disables if Option is out of range or if it isn't active in the scene.
                if (option >= 0 && Indicator.activeInHierarchy)
                {
                    Indicator.GetComponent<Image>().enabled = true;
                    Indicator.transform.localPosition = optionMovements[option];

                    if (bci.isTutorial)
                    {
                        infoTextSlot.text = tutCommandInfo[option];
                    }
                    else
                    {
                        infoTextSlot.text = mainCommandInfo[option];
                    }
                }
                else
                {
                    Indicator.GetComponent<Image>().enabled = false;
                }

                // Hide combo text if we pass through here.
                comboTextSlot.text = "";

                break;

            case CommandMenus.Fight:

                BattleTransitions batTran = gameObject.GetComponent<BattleTransitions>();
                int lastRow = row;
                int lastCol = col;

                // Reminder: Row refers to y coordinate, and col refers to x coordinate.
                // Move vertically through the BCI menu. 
                if (Input.GetKeyDown(KeyCode.DownArrow) && row > 0)
                {
                    row--;

                    // I know this looks like a lot, but basically we convert where we are in our grid to ingredients unlocked through a separate function. So if we are hovering over something we don't have unlocked, increment again; if that's out of range or also not unlocked, return to where we were.
                    if (!batTran.ingUnlocked[GridConversion(row, col)] && row > 0 && batTran.ingUnlocked[GridConversion(row - 1, col)])
                    {
                        row--;
                    }
                    else if (!batTran.ingUnlocked[GridConversion(row, col)])
                    {
                        row = lastRow;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow) && row < 3)
                {
                    row++;

                    if (!batTran.ingUnlocked[GridConversion(row, col)] && row < 3 && batTran.ingUnlocked[GridConversion(row + 1, col)])
                    {
                        row++;
                    }
                    else if (!batTran.ingUnlocked[GridConversion(row, col)])
                    {
                        row = lastRow;
                    }
                }

                // Move horizontally through the BCI menu.
                if (Input.GetKeyDown(KeyCode.LeftArrow) && col > 0)
                {
                    col--;

                    if (!batTran.ingUnlocked[GridConversion(row, col)] && col > 0 && batTran.ingUnlocked[GridConversion(row, col - 1)])
                    {
                        col--;
                    }
                    else if (!batTran.ingUnlocked[GridConversion(row, col)])
                    {
                        col = lastCol;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) && col < 2)
                {
                    col++;

                    if (!batTran.ingUnlocked[GridConversion(row, col)] && col < 2 && batTran.ingUnlocked[GridConversion(row, col + 1)])
                    {
                        col++;
                    }
                    else if (!batTran.ingUnlocked[GridConversion(row, col)])
                    {
                        col = lastCol;
                    }
                }


                // After movement has been determined, refresh the information panel as well as the indicator position. 
                if (!bci.isTutorial && !comboActive) // Our default battle case. If we're not in the tutorial fight, use our normal options. Don't ever do this if we currently have a combo selected.
                {
                    infoTextSlot.text = mainIngredientInfo[col][row];
                }
                else if (!comboActive) // If we ARE in the tutorial fight, however, you know how we do.
                {
                    infoTextSlot.text = tutIngredientInfo[col][row];
                }

                // Move the indicator to its corresponding ingredient.
                Indicator.transform.localPosition = ingredientPositions[col][row];

                // Move the combo text slot over to its BCI location.
                comboTextSlot.transform.position = comboTextSlotLocations[1].position;

                break;

            case CommandMenus.Combo: // Our combo menu. Will display combos available; selecting one will replace the standard information in the Info Panel with instructions for the current Combo.

                batTran = gameObject.GetComponent<BattleTransitions>();

                // Reminder: Row refers to y coordinate, and col refers to x coordinate.
                // Move vertically through the Combo
                if (Input.GetKeyDown(KeyCode.DownArrow) && row > 0)
                {
                    row--;
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow) && row < 2)
                {
                    row++;
                }

                // Move horizontally through the Item menu.
                if (Input.GetKeyDown(KeyCode.LeftArrow) && col > 0)
                {
                    col--;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) && col < 6)
                {
                    col++;
                }

                // If we have a combo unlocked, we can then display its information in the relevant panels. 
                int index = 0;

                // First, determine the index in a single-dimension array of what we have selected. It's fairly simple, thankfully, so a simple switch statement is all that's necessary.
                switch (row)
                {
                    case 0:
                        index = 14 + col;
                        break;
                    case 1:
                        index = 7 + col;
                        break;
                    case 2:
                        index = col;
                        break;
                }

                if (batTran.combosUnlocked[index])
                {
                    infoTextSlot.text = comboInfoText[row][col];

                    if (displayBCIComboWindow)
                        comboTextSlot.text = comboNameText[index];
                    else
                        comboTextSlot.text = "";
                }
                else
                {
                    comboTextSlot.text = "";
                    infoTextSlot.text = "Much like someone you know, there's nothing of substance here.";
                }

                // Move the combo text slot over to its Combo Menu location.
                comboTextSlot.transform.position = comboTextSlotLocations[0].position;

                // Move the indicator to its corresponding combo.
                Indicator.transform.localPosition = comboPositions[row][col];

                if (Input.GetButtonDown("Submit"))
                {
                    StartCoroutine(Confirm(index));
                }

                break;


            case CommandMenus.Item:
                batTran = gameObject.GetComponent<BattleTransitions>();

                // Reminder: Row refers to y coordinate, and col refers to x coordinate.
                // Move vertically through the Combo
                if (Input.GetKeyDown(KeyCode.DownArrow) && row > 0)
                {
                    row--;
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow) && row < 2)
                {
                    row++;
                }

                // Move horizontally through the Item menu.
                if (Input.GetKeyDown(KeyCode.LeftArrow) && col > 0)
                {
                    col--;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) && col < 6)
                {
                    col++;
                }

                // If we have a combo unlocked, we can then display its information in the relevant panels. 
                index = 0;

                // First, determine the index in a single-dimension array of what we have selected. It's fairly simple, thankfully, so a simple switch statement is all that's necessary.
                switch (row)
                {
                    case 0:
                        index = 14 + col;
                        break;
                    case 1:
                        index = 7 + col;
                        break;
                    case 2:
                        index = col;
                        break;
                }

                if (batTran.itemsUnlocked[index])
                {
                    infoTextSlot.text = itemInfoText[row][col];
                }
                else
                {
                    infoTextSlot.text = "Much like someone you know, there's nothing of substance here.";
                }

                // Move the indicator to its corresponding item.
                Indicator.transform.localPosition = comboPositions[row][col];
                break;
        }


        StartCoroutine(MenuNavigation()); // As this is a recursive function, call itself. It should be stopped whenever a new menu state is introduced. 
    }

    public void ToggleText(GameObject parent, bool onOff, bool comboMenu) // Copied from BattleMenuIngCaller. This, however, loops through a parent object and turns things on or off as need be. Checks if things should be on or off based on if we're in the combo or item menu.
    {

        for (int x = 0; x < parent.transform.childCount; x++) // Iterate through the children of the parent object. This should read the base rows of the item and combo menus.
        {
            Transform childLevelOne = parent.transform.GetChild(x);
            for (int y = 0; y < childLevelOne.childCount; y++) // Iterate through each child of the row. This should read individual combos/items.
            {
                GameObject item = childLevelOne.GetChild(y).gameObject;

                Text[] txt = item.GetComponentsInChildren<Text>();
                Outline[] otl = item.GetComponentsInChildren<Outline>(); // Grab our Text and Outline components

                int ind = (x * 7) + y; // Due to how the rows are set in the heirarchy, we actually progress linearly, making this a very simple calculation to find our index in a single-dimension array!


                if (onOff) // If onOff, toggle on. Also check to see if we're in the combo menu or item menu specifically to double check if something is unlocked before displaying it.
                {
                    if (comboMenu)
                    {
                        for (int i = 0; i < txt.Length; i++)
                        {
                            if (txt[i] && gameObject.GetComponent<BattleTransitions>().combosUnlocked[ind])
                                txt[i].enabled = true;
                            if (otl[i] && gameObject.GetComponent<BattleTransitions>().combosUnlocked[ind])
                                otl[i].enabled = true;
                        }

                        if (gameObject.GetComponent<BattleTransitions>().combosUnlocked[ind])
                        {
                            item.GetComponent<Image>().enabled = true;
                        }

                    }
                    else
                    {
                        for (int i = 0; i < txt.Length; i++)
                        {
                            if (txt[i] && gameObject.GetComponent<BattleTransitions>().itemsUnlocked[ind])
                                txt[i].enabled = true;
                            if (otl[i] && gameObject.GetComponent<BattleTransitions>().itemsUnlocked[ind])
                                otl[i].enabled = true;
                        }
                    }
                    
                }
                else // If !onOff, toggle off. No need to bother checking for if we're item or combo and what we have unlocked if it's getting disabled anyway.
                {
                    for (int i = 0; i < txt.Length; i++)
                    {
                        txt[i].enabled = false;
                        otl[i].enabled = false;
                    }

                    item.GetComponent<Image>().enabled = false;
                }


            }


        }
    }

    private int GridConversion(int row, int col) // Converts our current row and column into their respective ingredient values of a linear list.
    {
        /* So, Grid format: Bear in mind row is our y value and col is our x value.
         * 
         * 0 1 2
         * -----
         * 4 5 6 - Row 3
         * 1 2 3 - Row 2
         * 7 8 9 - Row 1
         * 10 11 0 - Row 0*/

        switch (row)
        {
            case 0: // The fuckiest row for sure. Because it isn't sequential, we need to use a second switch case.
                switch (col)
                {
                    case 0:
                        return 10;
                    case 1:
                        return 11;
                    case 2:
                        return 0;
                }
                break;
            case 1:
                return 7 + col;
            case 2:
                return 1 + col;
            case 3:
                return 4 + col;
        }


        return 0;
    }

    public IEnumerator BurgerPlateMove(bool onOff)
    {
        plateReady = false;

        if (onOff) // If the plate is moving onscreen.
        {
            while (burgerPlate.transform.position != plateActivePos.position)
            {
                burgerPlate.transform.position = Vector2.Lerp(burgerPlate.transform.position, plateActivePos.position, plateMoveTime);
                burgerPlate.transform.localScale = Vector3.Lerp(burgerPlate.transform.localScale, plateActivePos.localScale, plateMoveTime);
                yield return null;
            }
        }

        else if (!onOff) // If the plate is moving offscreen.
        {
            while (burgerPlate.transform.position != plateOffPos.position)
            {
                burgerPlate.transform.position = Vector2.Lerp(burgerPlate.transform.position, plateOffPos.position, plateMoveTime);
                burgerPlate.transform.localScale = Vector3.Lerp(burgerPlate.transform.localScale, plateOffPos.localScale, plateMoveTime);
                yield return null;
            }
        }

        plateReady = true;
    }

    public IEnumerator Back()
    {
        yield return new WaitUntil(() => ((Input.GetKeyDown(KeyCode.LeftControl) && bci.batTran.ingUnlocked[11]) || bci.bciDone) && isReady); // Wait until Left Control is pressed
        bci.bciDone = false;

        // If our open menu is BCI specifically, do BCI specific things. Likewise for other menus.
        if (commandOpen == CommandMenus.Fight)
        {
            if (plateMoveInst != null)
                StopCoroutine(plateMoveInst);

            plateMoveInst = BurgerPlateMove(false);
            StartCoroutine(plateMoveInst);

            for (int i = 0; i < ingredientHolder.Length; i++)
            {
                if (gameObject.GetComponent<BattleTransitions>().ingUnlocked[i])
                    ingredientHolder[i].GetComponent<Animator>().SetTrigger("OnOff");
            }

            comboActive = false;

        }

        if (commandOpen != CommandMenus.None) // If we aren't in the base command menu, go back into the command menu.
        {

            uiAnim.SetBool("BCI", false); // Reset our animator.
            uiAnim.SetBool("ItemCombo", false);
            uiAnim.SetBool("ComboInfo", false);

            ToggleText(comboHolder, false, true);
            ToggleText(itemHolder, false, false);

            commandOpen = CommandMenus.None;
        }


        StartCoroutine(Back()); // Recursive, call itself.
    }

    public IEnumerator Confirm(int choice)
    {
        Debug.Log("Confirm choice!");
        yield return new WaitForSeconds(0);

        if (!ns1.waitForScript) // As long as we're not waiting for the narrative script, we can confirm an option.
        {
            switch (commandOpen) // Base how Confirm behaves on which menu we're in.
            {
                case CommandMenus.None:
                    if (!backHome || !isReady) // If we haven't returned to the main menu or are otherwise not ready, don't do anything.
                        break;

                    switch (choice) // If we're in the base command menu, select our command.
                    {
                        case 0: // Fight
                            if (commandsAvailable[0])
                            {
                                Debug.Log("Fight selected and available!");
                                commandOpen = CommandMenus.Fight;

                                backHome = false;

                                uiAnim.SetBool("BCI", true); // Turn on BCI. If we're not in a tutorial or midway through, display the combo panel as well.
                                if (displayBCIComboWindow)
                                    uiAnim.SetBool("ComboInfo", true);

                                yield return new WaitUntil(() => readyUp && isReady);
                                readyUp = false;
                                isReady = false;

                                row = 2; // Default to hovering over Lettuce in the BCI menu.
                                col = 1;

                                // Turn on all ingredients within our Ingredient holder, but only if we have them unlocked.
                                for (int i = 0; i < ingredientHolder.Length; i++)
                                {
                                    if (gameObject.GetComponent<BattleTransitions>().ingUnlocked[i])
                                        ingredientHolder[i].GetComponent<Animator>().SetTrigger("OnOff");
                                }

                                // Slide our burger plate in. We create an instance of the coroutine that may be cancelled at any point.

                                if (plateMoveInst != null)
                                    StopCoroutine(plateMoveInst);

                                plateMoveInst = BurgerPlateMove(true);
                                StartCoroutine(plateMoveInst);

                                
                                burgerSpawner.SetActive(true);
                                bci.spawnReset = false; // Reset spawnreset.

                                //Indicator.SetActive(false);

                                //choiceText.SetActive(false);

                                StartCoroutine(bci.ComponentSpawn(KeyCode.Space, 3, bci.bottomBun, 0)); // Drop the bottom bun initially, and start up bci.
                                bci.serveStart = true;
                                StartCoroutine(bci.StartStuff());
                                StartCoroutine(bci.enableCheats());

                                yield return new WaitUntil(() => bci.bciDone || !burgerSpawner.activeInHierarchy); // Wait until bci deactivates itself by serving a burger or the burger is cancelled, and then we go back to selecting menu options.
                                bci.bciDone = false;
                                Debug.Log("Go back!");

                                //choiceText.SetActive(true);
                            }
                            else
                            {
                                Debug.Log("Fight selected, but unavailable.");
                                // Play error sound here.
                            }

                            break;

                        case 1: // Combo
                                // Bring up Combo UI here.

                            if (!isReady) // If we aren't allowed to input anything, stop doing what we're doing.
                                break;

                            commandOpen = CommandMenus.Combo;
                            uiAnim.SetBool("ItemCombo", true); // Turn on our Combo menu.
                            if (displayBCIComboWindow)
                                uiAnim.SetBool("ComboInfo", true);


                            yield return new WaitUntil(() => readyUp);
                            readyUp = false;

                            row = 2; // Default to hovering over Classic Combo.
                            col = 0;

                            ToggleText(comboHolder, true, true);

                            break;

                        case 2: // Items
                            commandOpen = CommandMenus.Item;


                            yield return new WaitUntil(() => readyUp);
                            readyUp = false;

                            uiAnim.SetBool("ItemCombo", true); // Turn on our Item menu.

                            row = 2; // Default to hovering over the item in our first slot. 
                            col = 0;

                            ToggleText(itemHolder, true, false);

                            break;

                        case 3: // Run

                            if (commandsAvailable[option])
                            {
                                isReady = false;
                                Player.GetComponent<Animator>().SetTrigger("Run");
                                yield return new WaitForSeconds(1);
                                StartCoroutine(GetComponent<BattleTransitions>().EndOfBattle(false));
                            }
                            else
                            {
                                isReady = false;
                                bci.comboText.text = "Can't run from this battle";
                                yield return new WaitForSeconds(1);
                                yield return new WaitUntil(() => Input.GetButtonDown("Submit"));
                                bci.comboText.text = "";
                                isReady = true;
                            }
                            break;
                    } // Base command options. Fight, Combo, Item, Run. Accepts Option as its choice.

                    
                    break;
                case CommandMenus.Fight: // Nothing should happen confirming anything in the fight menu. This stays empty.
                    break;
                case CommandMenus.Combo: // Selects a combo to appear in the info panel, locks it in, and then moves to BCI. Only if the selected combo is unlocked. Takes Index as its choice.
                    BattleTransitions batTran = gameObject.GetComponent<BattleTransitions>();

                    if (batTran.combosUnlocked[choice])
                    {
                        isReady = false;
                        comboActive = true;
                        string temp = comboOrderText[row][col]; // Store our directions temporarily, and until we're in the bci menu, hide the info text.
                        infoTextSlot.text = "";
                        comboTextSlot.text = "";
                        commandOpen = CommandMenus.Fight;
                        uiAnim.SetBool("BCI", true);
                        uiAnim.SetBool("ItemCombo", false);

                        yield return new WaitUntil(() => readyUp); // Wait until a bool is called from an animation event, dictating that we're now out of the combo menu.
                        readyUp = false;

                        ToggleText(comboHolder, false, true);

                        yield return new WaitUntil(() => readyUp); // Wait until a bool is called from an animation event, dictating that we're now in the BCI menu.
                        readyUp = false;

                        infoTextSlot.text = temp;



                        // And then insert standard BCI stuff.
                        row = 2; // Default to hovering over Lettuce in the BCI menu.
                        col = 1;

                        // Turn on all ingredients within our Ingredient holder, but only if we have them unlocked.
                        for (int i = 0; i < ingredientHolder.Length; i++)
                        {
                            if (gameObject.GetComponent<BattleTransitions>().ingUnlocked[i])
                                ingredientHolder[i].GetComponent<Animator>().SetTrigger("OnOff");
                        }

                        // Slide our burger plate in.

                        if (plateMoveInst != null)
                            StopCoroutine(plateMoveInst);

                        plateMoveInst = BurgerPlateMove(true);
                        StartCoroutine(plateMoveInst);

                        isReady = false; // We are no longer ready, so we activate bci and deactivate our indicator.
                        burgerSpawner.SetActive(true);
                        bci.spawnReset = false; // Reset spawnreset.

                        //Indicator.SetActive(false);

                        //choiceText.SetActive(false);

                        StartCoroutine(bci.ComponentSpawn(KeyCode.Space, 3, bci.bottomBun, 0)); // Drop the bottom bun initially, and start up bci.
                        bci.serveStart = true;
                        StartCoroutine(bci.StartStuff());
                        StartCoroutine(bci.enableCheats());

                        yield return new WaitUntil(() => bci.bciDone || !burgerSpawner.activeInHierarchy); // Wait until bci deactivates itself by serving a burger or the burger is cancelled, and then we go back to selecting menu options.
                        bci.bciDone = false;
                        Debug.Log("Go back!");

                        //choiceText.SetActive(true);
                        //isReady = true;
                    }

                    break;
                case CommandMenus.Item: // Consume an item. Currently has no function.
                    break;
            }
            
        }
        else
        {
            //Debug.Log("wait for script");
        }
    }


}