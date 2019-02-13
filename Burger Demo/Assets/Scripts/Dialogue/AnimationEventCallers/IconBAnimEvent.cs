using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconBAnimEvent : MonoBehaviour {

    public Animator IB;

    void Flash ()
    {
        IB.SetTrigger("Bounce");
    }

}
