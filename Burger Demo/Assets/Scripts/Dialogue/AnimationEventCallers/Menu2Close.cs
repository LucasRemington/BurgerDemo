using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu2Close : MonoBehaviour {

    public Image menu2;
    public MenuManager mm;

	void Close () {
        menu2.enabled = false;
	}

    void Open ()
    {
        mm.fadeInMenu2Text();
    }
	
}
