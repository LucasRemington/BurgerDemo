using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumpingHatMan : MonoBehaviour {

    public Animator crumpingSpeed;
    public InteractDia id;
    public bool crumpFlag;

    public void Update ()
    {
        if (id.canInteract == false && crumpFlag == false)
        {
            crumpFlag = true;
        }
        if (crumpFlag == true && id.canInteract == true)
        {
            crumpingSpeed.speed = 3f;
        }
    }

}
