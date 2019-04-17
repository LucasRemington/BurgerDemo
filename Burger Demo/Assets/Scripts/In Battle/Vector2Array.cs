using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Vector2Array
{
    public Vector2[] location; // An array of Vector2's. A wrapper class for an array of V2 arrays.

    // Allow us to apply indexing. 
    public Vector2 this[int i]
    {
        get
        {
            return location[i];
        }
        set
        {
            location[i] = value;
        }
    }
}
