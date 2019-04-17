using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StringArray

{
    public string[] row; // Used in a [column][row] format, the parent array is the column (x) and the child array is the row (y).

    // Allow us to apply indexing. 
    public string this[int i]
    {
        get
        {
            return row[i];
        }
        set
        {
            row[i] = value;
        }
    }
}
