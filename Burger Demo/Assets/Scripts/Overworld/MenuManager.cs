using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public GameObject player;
    public OverworldMovement ovm;
    public bool MenuOpen;
    public Image menuBox;
    public Animator boxUI;
    public int optionSelected;
    public Text[] optionText;
    public AudioClip OpenSound;
    public AudioClip CloseSound;
    public AudioClip menuMoveSound;
    public AudioSource soundMaker;

	void Start () {
        player = GameObject.FindWithTag("Player");
        player = player.transform.Find("OverworldPlayer").gameObject;
        ovm = player.GetComponent<OverworldMovement>();
        StartCoroutine(openMenu());
        menuBox.enabled = false;
        TurnOnText(false);
        StartCoroutine(selectOption());
        StartCoroutine(optionChoice());
    }
	
    void TurnOnText (bool on)
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

    void ColorText ()
    {
        for (int i = 0; i < optionText.Length; i++)
        {
            optionText[i].color = Color.white;
        }
        optionText[optionSelected].color = Color.red;
    }

	IEnumerator openMenu () {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));
        soundMaker.clip = OpenSound;
        soundMaker.Play();
        menuBox.enabled = true;
        optionSelected = 0;
        ColorText();
        boxUI.SetInteger("OptionSelected", -1);
        boxUI.SetTrigger("Escape");
        ovm.canMove = false;
        yield return new WaitForSeconds(0.5f);
        TurnOnText(true);
        MenuOpen = true;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));
        soundMaker.clip = CloseSound;
        soundMaker.Play();
        boxUI.SetTrigger("Escape");
        boxUI.SetInteger("OptionSelected", 3);
        TurnOnText(false);
        yield return new WaitForSeconds (4 / 12f);
        menuBox.enabled = false;
        MenuOpen = false;
        ovm.canMove = true;
        StartCoroutine(openMenu());
    }

    IEnumerator selectOption()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.DownArrow) && MenuOpen == true || Input.GetKeyDown(KeyCode.UpArrow) && MenuOpen == true);
        if (Input.GetKeyDown(KeyCode.DownArrow))
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
        if (Input.GetKeyDown(KeyCode.UpArrow))
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
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(selectOption());
    }

    IEnumerator optionChoice()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) && MenuOpen == true);
        switch (optionSelected)
        {
            case 0:
                Debug.Log("Inventory");
                break;

            case 1:
                Debug.Log("Options");
                break;

            case 2:
                Debug.Log("Quit");
                break;
        }
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(optionChoice());
    }
}
