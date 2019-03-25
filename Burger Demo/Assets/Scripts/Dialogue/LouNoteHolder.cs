using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LouNoteHolder : MonoBehaviour
{
    // Keeps a list of Lou notes within the scene. Acts similarly to Transition Tracker.
    public List<GameObject> louNoteList; // A list of each note in the scene.

    [HideInInspector] public SaveLoad saveLoad;
    private string sceneName; 

    private void Start()
    {
        // Immediately grab our SaveLoad script off of Base.
        saveLoad = GameObject.FindGameObjectWithTag("Base").GetComponent<SaveLoad>();

        // For efficiency's sake, grab our scene name once.
        sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // This for loop goes through all of the holder's children and adds them to a list of all lou notes within the scene.
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (!louNoteList.Contains(child))
            {
                louNoteList.Add(child);
            }
        }
    }


    // FUNCTION: Called from LouNoteTrigger from each child objects; upon one being read, this function is called so that the list doesn't need to be refreshed every frame. Go through our list of lou notes, and if it's been read, we mark it as such and then update our saveload script.
    public void ListUpdate(Dialogue dia)
    {
        Debug.Log("ListUpdate in " + this + " called.");

        // This updates the relevant seen list in our save data.
        for (int i = 0; i < saveLoad.louNotes.Count; i++) // A for loop that iterates through each dialogue in saveLoad.
        {
            if (saveLoad.louNotes[i] == dia) // If the dialogue in saveLoad is the same as the dialogue that's been read...
            {
                saveLoad.louNotesSeen[i] = true; // We mark it as having been so, and then break.
                break;
            }
        }
    }
}
