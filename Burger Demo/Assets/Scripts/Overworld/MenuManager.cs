using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    [HideInInspector] public GameObject player; //the player.
    [HideInInspector] public OverworldMovement ovm; //Movement script on the player
    [HideInInspector] public bool MenuOpen; //This bool is true when the menu is open.
    [Header("Images and Text")] 
    [Tooltip("Menu dialogue box; set in inspector.")] public Image menuBox; //this image is the menu itself. THIS MUST BE SET IN THE INSPECTOR-- sceneload triggers before start, so this will break stuff if you don't do this!
    [Tooltip("Second menu for ingredients; set in inspector.")] public Image menuBox2; //this is the secondary menu showing ingredients and HP that appears in game.
    [Tooltip("Images for the ingredients in the second menu; set in inspector.")] public Image[] ingredientImg; //the images of burger components in menu 2
    [Tooltip("Text for the ingredients in the second menu; set in inspector.")] public Text[] ingredientText; // the text below the images of burger components in menu2
    [HideInInspector] public Animator boxUI; //The animation /on/ the menu.
    [HideInInspector] public Animator boxUI2; //The animation on menu2.
    [HideInInspector] public int optionSelected; //The currently selected option: Either 0, 1, or 2 as of now. Checked alongside row to determine what event gets called.
    [HideInInspector] public int optionRow; //The currently selected row: Either 0, or as of now. Checked alongside optionSelected to determine what event gets called.
    [Tooltip("Each of the text boxes for the various options; should be three. Set in inspector.")] public Text[] optionText; //The three text objects that represent the three choices. 
    [Tooltip("The text options within the sub-menu.")] public Text[] optionsSubmenuText; //The text options in the submenu.

    [Header("Audio")]
    [Tooltip("Audio for when the menu opens; set in inspector.")] public AudioClip OpenSound; //The sound made when the menu opens.
    [Tooltip("Audio for when the menu closes; set in inspector.")] public AudioClip CloseSound; //The sound made when the menu closes.
    [Tooltip("Audio for when the selected option changes; set in inspector.")] public AudioClip menuMoveSound; //The sound made when you move around the menu.
    [Tooltip("Audio for when the player tries to select an option that they cannot; set in inspector")] public AudioClip errorSound; //The sound made when an option can't be selected.
    [Tooltip("Audio source required to play above sounds.")] public AudioSource soundMaker; //The audiosource actually needed to /play/ the above sounds.

    [HideInInspector] public Scene currentScene; //The current scene.
    [HideInInspector] public bool mainMenu; //True when in the mainMenu. The script doubles for the menu object accessible during gameplay by pressing esc and the menu at the start of the game.
    [HideInInspector] public bool mainMenuDone; // Once we leave the main menu, flip this off so that the menu can close fully without issue.
    [HideInInspector] public bool animFlag; //a flag bool turned on by animation events. Used to move along WaitUntil coroutines, should always be turned off immediately after.
    private bool playGame; //This is a submenu bool: Only possible to turn on during the main menu, it changes the text and available options, allowing the player to continue or start a new game. Or it will, eventually.
    private bool options; //This is a submenu bool. Accessible during mainMenu and gameplay, turning it on indicates that the Options submenu is open, which changes the size of the menu and the number of options.
    [HideInInspector] public bool closeNow; //a flag bool that immediately closes the menu. It'll still walk through the current close animations as of now.

    [Header("Vectors")]
    [Tooltip("Position of the menu during gameplay. Don't touch.")] public Vector3 endPosition; //In mainmenu, the menu starts off to the side. However, when options opens, it moves to the center. These three Vector3s are involved with setting and moving the menu.
    [Tooltip("Position of the menu in the main menu. Don't touch.")] public Vector3 startPosition;
    private Vector3 currentPosition;
    private float Tick; //An int used to lerp during a coroutine, so we can avoid using update.
    private bool meatLockersVisited = false; // Determines if the player has visited a meat locker, and thus can respawn. Option is greyed out otherwise.

    private bool saveDataExists; // Checks if a save file exists for the game.

    private SaveLoad saveLoad; // The saving and loading script attached to Base. It's used for....well... :/

    void Start () //grabs the basic scripts. Since the script is present during the first scene, it shouldn't need a pseudostart.
    {
        player = GameObject.FindWithTag("Player");
        player = player.transform.Find("OverworldPlayer").gameObject;
        ovm = player.GetComponent<OverworldMovement>();
        saveLoad = GameObject.FindGameObjectWithTag("Base").GetComponent<SaveLoad>();
        boxUI2 = menuBox2.GetComponent<Animator>();
        //menuBox = GetComponent<Image>();

        // Checks the path for the save file, and if it exists, then we know we have save data and can use Continue.
        if (System.IO.File.Exists(System.IO.Path.Combine(Application.persistentDataPath, "porygon.txt")))
        {
            saveDataExists = true;
        }
    }

    void OnEnable() //On enable, this checks which scene has been loaded.
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) //This sets the default menu based on the scene: currently, it's only different if mainmenu is the current scene. Also calls all the initial coroutines.
    {
        currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "MainMenu")
        {
            mainMenu = true;
            menuBox.transform.localPosition = new Vector3(125, 0, 0);
            optionText[0].text = "Play";
        }
        else
        {
            mainMenu = false;
            menuBox.transform.localPosition = new Vector3(220, 0, 0);
            optionText[0].text = "Respawn";
        }
        StopAllCoroutines();
        StartCoroutine(openMenu());
        StartCoroutine(selectOption());
        StartCoroutine(optionChoice());
    }

    void TurnOnText (bool on) //This function serves two purposes: when called as true, it turns on the text component all optiontext. When called as false, it turns those off instead. 
    {
        if (on == true)
        {
            for (int i = 0; i < optionText.Length; i++)
            {
                optionText[i].enabled = true;
            }
        }
        else
        {
            for (int i = 0; i < optionText.Length; i++)
            {
                optionText[i].enabled = false;
            }
        }
    }

    void SwitchTextSix (bool on) //this function does the same thing as TUrnOnText, except for the options submenu text. The two functions should probably be called close to each other, making sure that neither is on at the same time.
    {
        if (on == true)
        {
            for (int i = 0; i < optionsSubmenuText.Length; i++)
            {
                optionsSubmenuText[i].enabled = true;
            }
        }
        else
        {
            for (int i = 0; i < optionsSubmenuText.Length; i++)
            {
                optionsSubmenuText[i].enabled = false;
            }
        }
    }

    void ColorText () //This changes the 'selected' text to red, and everything else to white. However, in the case of respawn text, if it is not unlocked, turns it grey instead.
    {
        if (options == false)
        {
            for (int i = 0; i < optionText.Length; i++)
            {
                optionText[i].color = Color.white;

                if ((optionText[i].text == "Respawn" && !meatLockersVisited) || (optionText[i].text == "Continue" && !saveDataExists))
                {
                    optionText[i].color = new Color(0.3f, 0.3f, 0.3f);
                }
            }
            optionText[optionSelected].color = Color.red;
            if ((optionText[optionSelected].text == "Respawn" && !meatLockersVisited) || (optionText[optionSelected].text == "Continue" && !saveDataExists))
            {
                optionText[optionSelected].color = Color.red * new Color(0.3f, 0.3f, 0.3f);
            }

            
        } else
        {
            for (int i = 0; i < optionsSubmenuText.Length; i++)
            {
                optionsSubmenuText[i].color = Color.white;
            }
            if (optionRow == 0)
            {
                optionsSubmenuText[optionSelected].color = Color.red;
            } else
            {
                optionsSubmenuText[optionSelected + 3].color = Color.red;
            }
        }
    }

	IEnumerator openMenu () //This is a complex function. TLDR it controls how the menu opens and closes. 
    {
        yield return new WaitUntil(() => (Input.GetKeyDown(KeyCode.Escape) && ovm.canMove && !mainMenu) || (mainMenu && animFlag && !mainMenuDone)); //in-game, the function is called on esc press. In the mainmenu, it's called after the logo animation ends.
        // Checks first to see if we've visited a meat locker at some point so we can gray out the option if need be.
        Debug.Log(saveLoad.meatLockerList.Count);
        if (saveLoad.meatLockerList == null || saveLoad.meatLockerList.Count <= 0)
        {
            meatLockersVisited = false;
        }
        else
            meatLockersVisited = true;


        soundMaker.clip = OpenSound; //this sets the clip to the appropriate sound...
        if (mainMenu == false)
        {
            menu2Open(); //enables second menu
            soundMaker.Play(); //...and plays it, if you aren't in the mainMenu scene.
        }

        menuBox.enabled = true; //Enables the image component.
        optionSelected = 0; //Sets the optionselect to default, which should be at the top of the menu.
        ColorText(); //Colors the appropriate text
        boxUI.SetInteger("OptionSelected", -1); //Sets the integers and triggers needed for the animator - usually the optionSelected integer is the same, but setting it to -1 and 3 allow it to open and close without immediately transitioning.
        boxUI.SetTrigger("Escape");
        ovm.canMove = false; //If there's a player in the scene, they can't move while the menu is open. 
        yield return new WaitForSeconds(0.5f); //Waits until the menus opening animation is done.
        TurnOnText(true); //Activates the text components for the default menu.
        MenuOpen = true; //The menu is open, so this is true.
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape) && mainMenu == false && options == false || closeNow == true); //in-game, the function proceeds on escape press OR when closeNow is called. In mainmenu, this portion of the function shouldn't ever be called.
        if (options == false && mainMenu == false)
        {
            menu2Shut();
        }
        closeNow = false; //Turns off the flag, if it's on.
        soundMaker.clip = CloseSound; //Sets the clip to the appropriate sound and plays it.
        soundMaker.Play();
        boxUI.SetTrigger("Escape"); //Sets the variables needed by the animator.
        boxUI.SetInteger("OptionSelected", 3);
        TurnOnText(false); //Turns off the text immediately.
        yield return new WaitForSeconds (4 / 12f); //Waits until the close animation is finished, then switches off the appropriate variables.
        menuBox.enabled = false;
        MenuOpen = false;
        if (!mainMenu || mainMenuDone)
            ovm.canMove = true;
        StartCoroutine(openMenu()); //Calls the coroutine again at the end, so it'll check for escape press again.
    }

    IEnumerator selectOption() //This is a moderately complex function. TLDR it sets optionSelect.
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.DownArrow) && MenuOpen == true || Input.GetKeyDown(KeyCode.UpArrow) && MenuOpen == true); //Waits until the menu is open, and the up and down arrow keys are pressed.
        if (Input.GetKeyDown(KeyCode.DownArrow)) //If the input is down, and optionSelect is less than two, it increases. The appropriate sound is played, and the animation is set.
        {
            if (optionSelected < 2)
            {
                soundMaker.clip = menuMoveSound;
                soundMaker.Play();
                optionSelected++;
                boxUI.SetInteger("OptionSelected", optionSelected);
            }
            ColorText();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) //If the input is up, and optionSelect is greater than zero, it decreases. The appropriate sound is played, and the animation is set.
        {
            if (optionSelected > 0)
            {
                soundMaker.clip = menuMoveSound;
                soundMaker.Play();
                optionSelected--;
                boxUI.SetInteger("OptionSelected", optionSelected);
            }
            ColorText();
        }
        yield return new WaitForSeconds(0.1f); //The function waits a tenth of a second to give the input time to reset, and then calls itself again.
        StartCoroutine(selectOption());
    }

    IEnumerator optionChoice() //This is a complex function. TLDR it controls how the player selects options, opening submenus or executing other functions depending on optionSelect and optionRow.
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) && MenuOpen == true); //When the menu is open and space is pressed, the function activates.
        soundMaker.Play(); //the appropriate sound is played, and the function checks which submenu the menu is currently in. Once it's done this, it moves to a switch statement checking optionSelect. For the submenus with multiple rows, there's a nested switch statement checking the row first.
        if (mainMenu == true && playGame == false && options == false) //This is the default menu for the mainmenu.
        {
            switch (optionSelected)
            {
                case 0: //This changes the submenu to the 'playgame' submenu, setting the text as appropriate and turning on the relevant bool.
                    optionText[0].text = "Continue";
                    optionText[1].text = "New Game";
                    optionText[2].text = "Back";
                    playGame = true;
                    break;

                case 1: //This changes the submenu to the 'options' submenu, which is a complex set of operations contained within its own function.
                    Debug.Log("Options");
                    StartCoroutine(Options());
                    break;

                case 2: //This quits the game from the main menu. No action needed.
                    Debug.Log("Quit");
                    QuitGame();
                    break;
            }
        }
        else if (mainMenu == true && playGame == true) //This is the playgame submenu, accessible only through the mainmenu.
        {
            switch (optionSelected)
            {
                case 0:
                    Debug.Log("Continue"); //this continues the current game. Eventually, it will pull the relevant save data.

                    if (!saveDataExists)
                    {
                        soundMaker.clip = errorSound;
                        break;
                    }

                    soundMaker.clip = CloseSound;
                    closeNow = true;
                    mainMenuDone = true;
                    yield return new WaitUntil(() => !MenuOpen);

                    // Instead of starting the load game coroutine, we call a function that starts the coroutine on its parent script instead, else yield returns don't work properly.
                    saveLoad.LoadGameFunc(false);

                    // Once the screen is dark, set the player active.
                    yield return new WaitUntil(() => saveLoad.blackScreen.color.a >= 1);
                    ovm.gameObject.SetActive(true);
                    
                    
                    break;

                case 1:
                    Debug.Log("New Game; Wipe everything"); //This should probably trigger a choice popup: if you select yes, it wipes everything, resetting from zero and starting a new game. For now, it closes the menu and moves us to the freezer to start the game.
                    closeNow = true;
                    mainMenuDone = true;

                    StartCoroutine(saveLoad.FadeImageToFullAlpha(2, saveLoad.blackScreen)); // Fade into black.

                    yield return new WaitUntil(() => saveLoad.blackScreen.color.a >= 1);
                    saveLoad.gameObject.transform.position = Vector2.zero;
                    
                    SceneManager.LoadScene("OriginFreezer");
                    break;

                case 2: //This backs out of the submenu, 'resetting' back to the default main menu screen. 
                    optionText[0].text = "Play Game";
                    optionText[1].text = "Options";
                    optionText[2].text = "Quit";
                    playGame = false;
                    break;
            }
        }
        else if (options == true) //The options submenu is accessible either through mainmenu or in-game.
        {
            switch (optionRow) 
            {
                case 0:
                    switch (optionSelected) //This undoes most of the parameters set during the options function. 
                    {       
                        case 0:
                            StartCoroutine(resetToDefault());
                            break;

                        case 1:
                            Debug.Log("Sound"); //At some point, this will affect the sound settings... possibly by opening another submenu, god help us all.
                        break;

                        case 2:
                            Debug.Log("Image"); //At some point, this will affect the image settings... possibly by opening another submenu, god help us all.
                            break;
                    }
                    break;

                case 1: //Right now, this does nothing. In the future, it might do something. 
                    switch (optionSelected)
                    {
                        case 0:
                            Debug.Log("Other");
                        break;

                        case 1:
                           Debug.Log("Bullshit");
                        break;

                        case 2:
                           Debug.Log("I guess");
                        break;
                    }
                    break;
        }
        }
        else
        {
            switch (optionSelected) //This is the default menu during gameplay.
            {
                case 0: // Originally Inventory. Now sends the player back to the last Meat Locker they visited.
                    
                    if (meatLockersVisited)
                    {
                        Debug.Log("Respawn");
                        soundMaker.clip = OpenSound;
                        StartCoroutine(saveLoad.LoadGame(true));
                        closeNow = true;
                    }
                    else
                    {
                        soundMaker.clip = errorSound;
                    }
                    break;

                case 1://opens the options menu, as above.
                    Debug.Log("Options"); 
                    StartCoroutine(Options());
                    break;

                case 2://quits the game
                    Debug.Log("Quit");
                    StartCoroutine(saveLoad.SaveGame());
                    yield return new WaitForSeconds(2);
                    Debug.Log("Quitting now!");
                    QuitGame();
                    break;
            }
        }
        ColorText(); //Colors the appropriate text. We run it here as well so that once we select an option, the next page's colors update automatically. Mostly for the sake of if an option can't be selected.
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(optionChoice());
    }

    //these functions can be used to smoothly fade text and images in and out

    public IEnumerator FadeImageToZeroAlpha(float t, Image i)
    {
        yield return new WaitForSeconds(.1f);
        //yield return new WaitUntil(() => readyForFade == true);
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }

    }

    public IEnumerator FadeImageToFullAlpha(float t, Image i)
    {
        yield return new WaitForSeconds(.1f);
        //yield return new WaitUntil(() => readyForFade == true);
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    public IEnumerator FadeTextToZeroAlpha(float t, Text i)
    {
        yield return new WaitForSeconds(.1f);
        //yield return new WaitUntil(() => readyForFade == true);
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }

    }

    public IEnumerator FadeTextToFullAlpha(float t, Text i)
    {
        yield return new WaitForSeconds(.1f);
        //yield return new WaitUntil(() => readyForFade == true);
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    public IEnumerator FadeOutlineToZeroAlpha(float t, Outline i)
    {
        yield return new WaitForSeconds(1f);
        //yield return new WaitUntil(() => readyForFade == true);
        i.effectColor = new Color(i.effectColor.r, i.effectColor.g, i.effectColor.b, 1);
        while (i.effectColor.a > 0.0f)
        {
            i.effectColor = new Color(i.effectColor.r, i.effectColor.g, i.effectColor.b, i.effectColor.a - (Time.deltaTime / t));
            yield return null;
        }

    }

    IEnumerator Options () //A complex function that sets up the options submenu, changing the size and position of the menu object while also adding the concept of 'rows' to optionselect.
    {
        options = true; //Sets the options submenu bool to true.
        transform.localPosition = new Vector3(0, 0, 0); //Originally tried this with lerping, but it looked weird
        menuBox.transform.localScale = new Vector3 (8, 4, 0); //Changes the size of the menu.
        boxUI.SetBool("OptionsMenu", true); //Set the animation bools.
        boxUI.SetInteger("OptionSelected", -1);
        if (mainMenu == false)
        {
            menu2Shut();
        }
        TurnOnText(false); //turns off the default text.
        yield return new WaitForSeconds(0.5f); //waits until the opening animation is finished.
        SwitchTextSix(true); //Turns on the option menu text.
        boxUI.SetInteger("OptionSelected", 0); //Sets the optionselected to the default of zero.
        optionSelected = 0;
        ColorText(); //Colors the appropriate text.
        StartCoroutine(rowSelect()); //Beings the rowselect coroutine, which functions similar to selectOptions, except it adds a second 'dimension' to the available options.
        yield return new WaitUntil(() => (Input.GetKeyDown(KeyCode.Escape) && mainMenu == false) || options == false); //the coroutine ends when options is no longer true OR when escape is pressed in-game.
        if (Input.GetKeyDown(KeyCode.Escape) && mainMenu == false) //when escape is pressed in-game...
        {
            StartCoroutine(resetToDefault());
        }

    }

    IEnumerator resetToDefault ()
    {
        if (mainMenu == true) //This begins to move the menu back to its original position if mainmenu is true.
        {
            StartCoroutine(lerpOver(0, 125));
        }
        else
        {
            StartCoroutine(lerpOver(0, 220));
            menu2Open();
        }
        boxUI.SetBool("OptionsMenu", false); //Sets the appropriate bools.
        boxUI.SetInteger("OptionSelected", 4);
        SwitchTextSix(false); //Turns off the option text.
        menuBox.transform.localScale = new Vector3(4, 4, 0); //Sets the appropriate size of the menu.
        yield return new WaitForSeconds(0.25f); //Waits for the close animation to finish.
        TurnOnText(true); //Turns on the menu text.
        boxUI.SetInteger("OptionSelected", 0); //Resets the optionselect value to default.
        optionSelected = 0;
        options = false; //Sets the submenu bool to false.
        ColorText(); //Colors the appropriate text.
    }

    public void menu2Open ()
    {
        menuBox2.enabled = true;
        boxUI2.SetTrigger("Menu2Close"); //turns on the secondary menu
    }

    public void fadeInMenu2Text () //called from animation, at the end of menu2open. fades in text and images
    {
        Debug.Log("fade in called");
        for (int i = 0; i < ingredientImg.Length; i++) //fades out text and images
        {
            if (ingredientImg[i] != null)
            {
                StartCoroutine(FadeImageToFullAlpha(0.5f, ingredientImg[i]));
                StartCoroutine(FadeTextToFullAlpha(0.5f, ingredientText[i]));
            }
            else
            {
                i = 20;
            }
        }
    }

    public void menu2Shut()
    {
        boxUI2.SetTrigger("Menu2Close"); //closes the secondary menu
            for (int i =0; i < ingredientImg.Length; i++) //fades out text and images
        {
            if (ingredientImg[i] != null)
            {
                ingredientImg[i].color = new Color(ingredientImg[i].color.r, ingredientImg[i].color.g, ingredientImg[i].color.b, 0);
                ingredientText[i].color = new Color(ingredientText[i].color.r, ingredientText[i].color.g, ingredientText[i].color.b, 0);
            } else
            {
                i = 20;
            }
        }
    }

    IEnumerator lerpOver(int startX, int endX) //This moves the object over to the correct position during mainmenu.
    {
        startPosition = new Vector3(startX, 0, 0);
        endPosition = new Vector3(endX, 0, 0);
        transform.localPosition = Vector3.Lerp(startPosition, endPosition, (Tick / 15f));
        currentPosition = this.transform.localPosition;
        yield return new WaitForSeconds(0.01f);
        if (currentPosition != endPosition)
        {
            Tick++;
            StartCoroutine(lerpOver(startX, endX));
        }
        else
        {
            Tick = 0;
        }
    }

    IEnumerator rowSelect () //This functions very similarly to optionselect, except it controls which 'row' (left or right) the player has selected.
    {
        yield return new WaitUntil(() => (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) && options == true && MenuOpen == true);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (optionRow != 0)
            {
                soundMaker.clip = menuMoveSound;
                soundMaker.Play();
                optionRow = 0;
                boxUI.SetInteger("OptionRow", 1);
            }
            ColorText();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (optionRow != 1)
            {
                soundMaker.clip = menuMoveSound;
                soundMaker.Play();
                optionRow = 1;
                boxUI.SetInteger("OptionRow", 2);
            }
            ColorText();
        }
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(rowSelect());
    }

    private void QuitGame() // Quits the game.
    {
        Application.Quit();
    }

}
