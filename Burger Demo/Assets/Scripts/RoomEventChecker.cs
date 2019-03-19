using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEventChecker : MonoBehaviour {

    public enum Action { Delete, Create, Enable, Disable };
    public enum B4OrAfter { Before, During, After };

    [Tooltip("Use this to determine when whatever you're doing happens.")] public B4OrAfter When;

    [Tooltip("This is the which event the thing is happening before, during, or after (see previous variable)")] public int eventNumber;
    
    [Tooltip("This determines what the rest of the script does. Delete destroys things, Create makes things, Enable turns things on, Disable turns things off.")] public Action action;
    
    [Tooltip("This is the list of objects you are modifying. It will do whatever action is selected to all of them.")] public GameObject[] Objects;

    [Tooltip("This is where things will be Created. Obviously only matters if you are using Create.")] public Vector3[] SpawnLocations;

    private NarrativeManager NarMan;

    // Use this for initialization
    void Start()
    {
        NarMan = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<NarrativeManager>();
        if ((NarMan.ev < eventNumber && When == B4OrAfter.Before) || (NarMan.ev == eventNumber && When == B4OrAfter.During) || (NarMan.ev > eventNumber && When == B4OrAfter.After))
        {
            switch (action)
            {
                case Action.Delete:
                    foreach (GameObject thing in Objects)
                    {
                        Destroy(thing);
                    }
                    break;
                case Action.Create:
                    for (int i = 0; i < Objects.Length; i++)
                    {
                        Instantiate(Objects[i], SpawnLocations[i], Quaternion.identity);
                    }
                    break;
                case Action.Enable:
                    foreach (GameObject thing in Objects)
                    {
                        thing.SetActive(true);
                    }
                    break;
                case Action.Disable:
                    foreach (GameObject thing in Objects)
                    {
                        thing.SetActive(false);
                    }
                    break;
            }
        }
    }
}
