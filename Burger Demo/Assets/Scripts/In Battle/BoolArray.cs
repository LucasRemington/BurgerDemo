using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BoolArray
{
    public bool[] bools; // An array of bools. Wrapper class.

    // Allow us to apply indexing. 
    public bool this[int i]
    {
        get
        {
            return bools[i];
        }
        set
        {
            bools[i] = value;
        }
    }
}
