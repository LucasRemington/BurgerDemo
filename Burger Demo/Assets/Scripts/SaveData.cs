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
    public bool[] ingUnlocked; // Ingredients the player has obtained. 0 = Bun; 1 = Tomato; 2 = Lettuce; 3 = Onion; 4 = Bacon; 5 = Sauce; 6 = Pickles; 7 = Ketchup; 8 = Mustard; 9 = Cheese; 10 = Patty 
    public bool gameStarted; // Ingredient tiers unlocked. // If you've watched the opening cutscene or not.

    public bool[] combosUnlocked;
    public bool[] itemsUnlocked;
    public int[] itemCount;

    public List<string> meatLockerList; // All currently-visited meat lockers. Serialized by the name of the scene they inhabit. 
    public int meatLockerIndex; // The index of the meat locker you last visited inside the above list.
    public Vector2 meatLockerPos; // The position of the meat locker saved at.

    public int narrManEventNo; // The current event number being kept track of in narrative manager.
    public int narrManRoomNo; // The current room number being kept track of in narrative manager. I don't like this.

    //public List<string> louNoteSceneList; // A list of every scene in which Lou notes appear in. Upon transitioning to a new room, check: does this room have a Lou note somewhere within it? If yes: is this room already in the list? If no: Add it.
    public List<bool> louNotesSeen; // So basically, the first list[a] acts similarly to the above list of strings; their indeces correspond to each other. The lists[b] inside of it are the same size as the Lou Note Holders within those scenes, and determine if they've been read or not and can be destroyed if so.
    public bool louDone; // Whether or not to bother checking and updating Lou notes in the first place.
}
