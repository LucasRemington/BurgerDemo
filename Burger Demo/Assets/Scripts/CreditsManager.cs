using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour {

    public Image DWBImage; //The DWB logo image.
    public Text DWBText; //Large text, across the whole screen.
    public Image logoImage; //Smaller image, below name text.
    public Text[] nameText; //The name of the person/company being credited. Is an array in case we ever want to display a multitude of these names at once.
    public Text[] creditsText; //What the person is being credited for. Allows multiple roles.
    [HideInInspector] public Image personalImg; //A small image above name text.
    public Sprite[] spr; //These are set in the inspector, and are pulled by the various images to display.
    public float SmallWait; //A small wait time.
    public float LongWait; //A long wait time.
    public float timeToFade; //The base time an image takes to fade out.

    void Start () //Clears all text, sets all images to zero alpha, clears the sprites of logoimage and personalimage. Starts the credits coroutine.
    {
        instantClearText();
        personalImg.sprite = null;
        personalImg.color = new Color(personalImg.color.r, personalImg.color.g, personalImg.color.b, 0);
        logoImage.sprite = null;
        logoImage.color = new Color(logoImage.color.r, logoImage.color.g, logoImage.color.b, 0);
        DWBImage.color = new Color(DWBImage.color.r, DWBImage.color.g, DWBImage.color.b, 0);
        StartCoroutine(credits());
	}

    IEnumerator credits() //This function is long but it's actually pretty simple. Each chunk follows a very specific template, except for the first two.
    {
        yield return new WaitForSeconds(LongWait + 1f); //After a wait, the logo image slowly fades in, waits, and then slowly fades out.
        logoImage.sprite = spr[0];
        StartCoroutine(FadeImageToFullAlpha(timeToFade * 3, DWBImage));
        yield return new WaitForSeconds(LongWait + 2f);
        StartCoroutine(FadeImageToZeroAlpha(timeToFade * 2, DWBImage));

        yield return new WaitForSeconds(LongWait + 1f); // Then, the 3MF logo and text fade in, stay, and fade out.
        logoImage.sprite = spr[12];
        DWBText.fontSize = 40;
        StartCoroutine(FadeTextToFullAlpha(timeToFade * 2f, DWBText, "A Game By Three Minute Flip", 255, 255, 255));
        yield return new WaitForSeconds(timeToFade * 2f);
        StartCoroutine(FadeImageToFullAlpha(timeToFade, logoImage));
        yield return new WaitForSeconds(LongWait + 1f);
        StartCoroutine(FadeTextToZeroAlpha(timeToFade, DWBText));
        StartCoroutine(FadeImageToZeroAlpha(timeToFade * 1.5f, logoImage));

        yield return new WaitForSeconds(LongWait); //This chunk is the 'template' for most other chunks
        personalImg.sprite = spr[1]; //it sets the personalImage to the corresponindg sprite...
        StartCoroutine(FadeTextToFullAlpha(timeToFade, nameText[0], "Garrett Bryant", 81, 172, 146)); //Calls FTTFA on nametext, giving the appropriate name and RGB color...
        yield return new WaitForSeconds(timeToFade * 2f); //...fades in...
        StartCoroutine(FadeImageToFullAlpha(timeToFade, personalImg)); //...fades in the image...
        yield return new WaitForSeconds(timeToFade * 3f); //...waits for a bit...
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[0], "Art Director", 255, 255, 255)); //...and displays all the credits, after waiting for smallwait betwen each.
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[1], "Creative Director", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[2], "2D Artist", 255, 255, 255));
        yield return new WaitForSeconds(LongWait); //At the end, after a longwait, the text is cleared, and the next person is credited.
        clearText();

        yield return new WaitForSeconds(LongWait);
        personalImg.sprite = spr[2];
        StartCoroutine(FadeTextToFullAlpha(timeToFade, nameText[0], "Kristie Morrison", 255, 157, 211));
        yield return new WaitForSeconds(timeToFade * 2f);
        StartCoroutine(FadeImageToFullAlpha(timeToFade, personalImg));
        yield return new WaitForSeconds(timeToFade * 3f);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[0], "2D Artist", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[1], "2D Animator", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[2], "Spreadsheet Manager", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();

        yield return new WaitForSeconds(LongWait);
        personalImg.sprite = spr[3];
        StartCoroutine(FadeTextToFullAlpha(timeToFade, nameText[0], "Quintin Connell", 106, 168, 79));
        yield return new WaitForSeconds(timeToFade * 2f);
        StartCoroutine(FadeImageToFullAlpha(timeToFade, personalImg));
        yield return new WaitForSeconds(timeToFade * 3f);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[0], "Lead Animator", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();

        yield return new WaitForSeconds(LongWait);
        personalImg.sprite = spr[4];
        StartCoroutine(FadeTextToFullAlpha(timeToFade, nameText[0], "Matt Spillane", 74, 134, 232));
        yield return new WaitForSeconds(timeToFade * 2f);
        StartCoroutine(FadeImageToFullAlpha(timeToFade, personalImg));
        yield return new WaitForSeconds(timeToFade * 3f);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[0], "Lead Engineer", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();

        yield return new WaitForSeconds(LongWait);
        personalImg.sprite = spr[5];
        StartCoroutine(FadeTextToFullAlpha(timeToFade, nameText[0], "James Vasquez", 244, 198, 255));
        yield return new WaitForSeconds(timeToFade * 2f);
        StartCoroutine(FadeImageToFullAlpha(timeToFade, personalImg));
        yield return new WaitForSeconds(timeToFade * 3f);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[0], "Engineer", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();

        yield return new WaitForSeconds(LongWait);
        personalImg.sprite = spr[6];
        StartCoroutine(FadeTextToFullAlpha(timeToFade, nameText[0], "Lucas Acevedo", 180, 167, 214));
        yield return new WaitForSeconds(timeToFade * 2f);
        StartCoroutine(FadeImageToFullAlpha(timeToFade, personalImg));
        yield return new WaitForSeconds(timeToFade * 3f);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[0], "Lead Level Designer", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[1], "Producer", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();

        yield return new WaitForSeconds(LongWait);
        personalImg.sprite = spr[7];
        StartCoroutine(FadeTextToFullAlpha(timeToFade, nameText[0], "Sam Gomes", 234, 153, 153));
        yield return new WaitForSeconds(timeToFade * 2f);
        StartCoroutine(FadeImageToFullAlpha(timeToFade, personalImg));
        yield return new WaitForSeconds(timeToFade * 3f);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[0], "Level Designer", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();

        yield return new WaitForSeconds(LongWait);
        personalImg.sprite = spr[8];
        StartCoroutine(FadeTextToFullAlpha(timeToFade, nameText[0], "Torrey Wang", 221, 126, 107));
        yield return new WaitForSeconds(timeToFade * 2f);
        StartCoroutine(FadeImageToFullAlpha(timeToFade, personalImg));
        yield return new WaitForSeconds(timeToFade * 3f);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[0], "Level Designer", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();

        yield return new WaitForSeconds(LongWait);
        personalImg.sprite = spr[9];
        StartCoroutine(FadeTextToFullAlpha(timeToFade, nameText[0], "TJ Mitchell", 182, 215, 168));
        yield return new WaitForSeconds(timeToFade * 2f);
        StartCoroutine(FadeImageToFullAlpha(timeToFade, personalImg));
        yield return new WaitForSeconds(timeToFade * 3f);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[0], "Narrative Design", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[1], "Audio Design", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();

        yield return new WaitForSeconds(LongWait);
        personalImg.sprite = spr[10];
        StartCoroutine(FadeTextToFullAlpha(timeToFade, nameText[0], "Lucas Remington", 255, 106, 0));
        yield return new WaitForSeconds(timeToFade * 2f);
        StartCoroutine(FadeImageToFullAlpha(timeToFade, personalImg));
        yield return new WaitForSeconds(timeToFade * 3f);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[0], "Creative Lead", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[1], "UX Designer", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[2], "Narrative Lead", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[3], "Mechanics Designer", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();

        yield return new WaitForSeconds(LongWait); //Other than being a special thanks, not individual credits, this functions basically the same as the other chunks.
        personalImg.sprite = spr[11];
        StartCoroutine(FadeTextToFullAlpha(timeToFade, nameText[0], "Special Thanks", 255, 255, 255));
        yield return new WaitForSeconds(timeToFade * 2f);
        StartCoroutine(FadeImageToFullAlpha(timeToFade, personalImg));
        yield return new WaitForSeconds(timeToFade * 3f);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[0], "Jeffrey Warmouth", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[1], "Jon Amakowa", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[2], "Samuel Tobin", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[3], "Les Nelkin", 255, 255, 255));
        yield return new WaitForSeconds(SmallWait);
        StartCoroutine(FadeTextToFullAlpha(timeToFade, creditsText[4], "Britt Snyder", 255, 255, 255));
        yield return new WaitForSeconds(LongWait);
        clearText();
    }

    void instantClearText() //Instantly clears all text, without fading.
    {
        for (int i = 0; i < nameText.Length; i++)
        {
            nameText[i].text = "";
            DWBText.text = "";
        }
        for (int i = 0; i < creditsText.Length; i++)
        {
            creditsText[i].text = "";
        }
    }

    void clearText () //Fades out the personal images, and calls clearTextTimed.
    {
        StartCoroutine(clearTextTimed());
        StartCoroutine(FadeImageToZeroAlpha(timeToFade * 2, personalImg));
    }

    IEnumerator clearTextTimed () //fades out nameText and creditsText.
    {
        for (int i = 0; i < nameText.Length; i++)
        {
            StartCoroutine(FadeTextToZeroAlpha(timeToFade, nameText[i]));
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < creditsText.Length; i++)
        {
            StartCoroutine(FadeTextToZeroAlpha(timeToFade, creditsText[i]));
            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator FadeTextToFullAlpha(float t, Text i, string nText, float r, float g, float b) //Fades in text from zero to full alpha.
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
    } //Fades out text from zero to full alpha.

    public IEnumerator FadeImageToFullAlpha(float t, Image i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0f);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    } //Fades in images from zero to full alpha.

    public IEnumerator FadeImageToZeroAlpha(float t, Image i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    } //fades out images from zero to full alpha.
    
}
