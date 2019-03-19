using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class SaveData
{
    // Add to this as we need more things that get saved.

    public int currentHealth; // The player's current health at time of saving. Equal to max after using a meat locker. 
    public int healthMax; // The player's current maximum health at time of saving.
    public Vector2 currentPos; // The location the player saved at.
    public bool[] convoDone; // Conversations that the player has seen. From dialogue holder.
    public string currentScene; // The room the player saved in.
    public int[] ingredients; // Current ingredient count.
    public bool ingRowTwoUnlocked, ingRowThreeUnlocked, gameStarted; // Ingredient tiers unlocked. // If you've watched the opening cutscene or not.

    public List<string> meatLockerList; // All currently-visited meat lockers. Serialized by the name of the scene they inhabit. 
    public int meatLockerIndex; // The index of the meat locker you last visited inside the above list.
    public Vector2 meatLockerPos; // The position of the meat locker saved at.

    public int narrManEventNo; // The current event number being kept track of in narrative manager.
    public int narrManRoomNo; // The current room number being kept track of in narrative manager. I don't like this.
}
