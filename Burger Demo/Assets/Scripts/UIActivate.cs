using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActivate : MonoBehaviour {

    public GameObject burgerPlate;
    public GameObject bottomUI;
    public GameObject icons;
    public static int entryOrder;
    public Text[] textToFade;

    void Awake () {
        entryOrder = 0;
	}
	
	void ExtendUI ()
    {
        switch (entryOrder)
        {
            case 0:
                icons.SetActive(true);
                bottomUI.SetActive(true);
                entryOrder++;
                break;
            case 1:
                for (int i = 0; i < textToFade.Length ; i++)
                {
                    StartCoroutine(FadeTextToFullAlpha(1f, textToFade[i]));
                }
                break;
        } 
	}

    public IEnumerator FadeTextToFullAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
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
    }

}
