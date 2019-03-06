using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class SaveData
{
    // Add to this as we need more things that get saved.
    public int currentHealth;
    public int healthMax;
    public Vector2 currentPos;
    public bool[] convoDone;
    public string currentScene;
    public int[] ingredients;
    public bool ingRowTwoUnlocked, ingRowThreeUnlocked, gameStarted;

    public List<string> meatLockerList;
    public int meatLockerIndex;
    public Vector2 meatLockerPos;
}
