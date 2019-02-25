using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour {

    public Text[] nameText;
    public Text[] creditsText;
    public Image personalImg;
    public Sprite[] spr;
    public float SmallWait;
    public float LongWait;

    void Start ()
    {
        instantClearText();
        personalImg.sprite = null;
        personalImg.color = new Color(personalImg.color.r, personalImg.color.g, personalImg.color.b, 0);
        StartCoroutine(credits());
	}
	
	IEnumerator credits ()
    {
        yield return new WaitForSeconds(LongWait + 1);
        personalImg.sprite = spr[0];
        StartCoroutine(FadeTextToFullAlpha(0.5f,nameText[0],"Dark World Burger", 255, 255, 255));
        yield return new WaitForSeconds(0.45f);
        StartCoroutine(FadeImageToFullAlpha(0.5f, personalImg));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[0], "A Game By Three Minute Flip", 255, 255, 255));
        yield return new WaitForSeconds(LongWait + 1);
        clearText();

        yield return new WaitForSeconds(LongWait);
        personalImg.sprite = spr[1];
        StartCoroutine(FadeTextToFullAlpha(0.5f, nameText[0], "Garrett Bryant", 81, 172, 146));
        yield return new WaitForSeconds(0.45f);
        StartCoroutine(FadeImageToFullAlpha(0.5f, personalImg));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[0], "Art Director", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[1], "Creative Director", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[2], "2D Artist", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();

        yield return new WaitForSeconds(LongWait);
        personalImg.sprite = spr[2];
        StartCoroutine(FadeTextToFullAlpha(0.5f, nameText[0], "Kristie Morrison", 255, 157, 211));
        yield return new WaitForSeconds(0.45f);
        StartCoroutine(FadeImageToFullAlpha(0.5f, personalImg));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[0], "2D Artist", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[1], "2D Animator", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[2], "Spreadsheet Manager", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();

        yield return new WaitForSeconds(LongWait);
        personalImg.sprite = spr[3];
        StartCoroutine(FadeTextToFullAlpha(0.5f, nameText[0], "Quintin Connell", 106, 168, 79));
        yield return new WaitForSeconds(0.45f);
        StartCoroutine(FadeImageToFullAlpha(0.5f, personalImg));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[0], "Lead Animator", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();

        yield return new WaitForSeconds(LongWait);
        personalImg.sprite = spr[4];
        StartCoroutine(FadeTextToFullAlpha(0.5f, nameText[0], "Matt Spillane", 74, 134, 232));
        yield return new WaitForSeconds(0.45f);
        StartCoroutine(FadeImageToFullAlpha(0.5f, personalImg));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[0], "Lead Engineer", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();

        yield return new WaitForSeconds(LongWait);
        personalImg.sprite = spr[5];
        StartCoroutine(FadeTextToFullAlpha(0.5f, nameText[0], "James Vasquez", 153, 0, 255));
        yield return new WaitForSeconds(0.45f);
        StartCoroutine(FadeImageToFullAlpha(0.5f, personalImg));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[0], "Engineer", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();

        yield return new WaitForSeconds(LongWait);
        personalImg.sprite = spr[6];
        StartCoroutine(FadeTextToFullAlpha(0.5f, nameText[0], "Lucas Acevedo", 180, 167, 214));
        yield return new WaitForSeconds(0.45f);
        StartCoroutine(FadeImageToFullAlpha(0.5f, personalImg));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[0], "Lead Level Designer", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[1], "Producer", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();

        yield return new WaitForSeconds(LongWait);
        personalImg.sprite = spr[7];
        StartCoroutine(FadeTextToFullAlpha(0.5f, nameText[0], "Sam Gomes", 234, 153, 153));
        yield return new WaitForSeconds(0.45f);
        StartCoroutine(FadeImageToFullAlpha(0.5f, personalImg));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[0], "Level Designer", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();

        yield return new WaitForSeconds(LongWait);
        personalImg.sprite = spr[8];
        StartCoroutine(FadeTextToFullAlpha(0.5f, nameText[0], "Torrey Wang", 221, 126, 107));
        yield return new WaitForSeconds(0.45f);
        StartCoroutine(FadeImageToFullAlpha(0.5f, personalImg));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[0], "Level Designer", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();

        yield return new WaitForSeconds(LongWait);
        personalImg.sprite = spr[9];
        StartCoroutine(FadeTextToFullAlpha(0.5f, nameText[0], "TJ Mitchell", 182, 215, 168));
        yield return new WaitForSeconds(0.45f);
        StartCoroutine(FadeImageToFullAlpha(0.5f, personalImg));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[0], "Narrative Design", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[1], "Audio Design", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();

        yield return new WaitForSeconds(LongWait);
        personalImg.sprite = spr[10];
        StartCoroutine(FadeTextToFullAlpha(0.5f, nameText[0], "Lucas Remington", 255, 106, 0));
        yield return new WaitForSeconds(0.45f);
        StartCoroutine(FadeImageToFullAlpha(0.5f, personalImg));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[0], "Creative Lead", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[1], "UX Designer", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[2], "Narrative Lead", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[3], "Mechanics Designer", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();

        yield return new WaitForSeconds(LongWait);
        personalImg.sprite = spr[11];
        StartCoroutine(FadeTextToFullAlpha(0.5f, nameText[0], "Special Thanks", 255, 255, 255));
        yield return new WaitForSeconds(0.45f);
        StartCoroutine(FadeImageToFullAlpha(0.5f, personalImg));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[0], "Jeffrey Warmouth", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[1], "Jon Amakowa", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[2], "Samuel Tobin", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[3], "Les Nelkin", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(0.5f, creditsText[4], "Britt Snyder", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();
    }

    void instantClearText()
    {
        for (int i = 0; i < nameText.Length; i++)
        {
            nameText[i].text = "";
        }
        for (int i = 0; i < creditsText.Length; i++)
        {
            creditsText[i].text = "";
        }
    }

    void clearText ()
    {
        StartCoroutine(clearTextTimed());
        StartCoroutine(FadeImageToZeroAlpha(1f, personalImg));
    }

    IEnumerator clearTextTimed ()
    {
        for (int i = 0; i < nameText.Length; i++)
        {
            StartCoroutine(FadeTextToZeroAlpha(0.5f, nameText[i]));
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < creditsText.Length; i++)
        {
            StartCoroutine(FadeTextToZeroAlpha(0.5f, creditsText[i]));
            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator FadeTextToFullAlpha(float t, Text i, string nText, float r, float g, float b)
    {
        i.text = nText;
        i.color = new Color(r / 255f, g / 255f, b / 255f, 0f);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    public IEnumerator FadeTextToZeroAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
        i.text = "";
    }

    public IEnumerator FadeImageToFullAlpha(float t, Image i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0f);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    public IEnumerator FadeImageToZeroAlpha(float t, Image i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }

}
