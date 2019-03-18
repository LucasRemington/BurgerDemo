using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEventChecker : MonoBehaviour {

    public enum Action { Delete, Create, Enable, Disable };

    public Action action;
    
    [Tooltip("This is the list of objects you are modifying. It will do whatever action is selected to all of them")] public GameObject[] Objects;

    [Tooltip("This is where things will be Created. Obviously only matters if you are using Create")] public Vector3[] SpawnLocations;

	// Use this for initialization
	void Start () {
        switch (action) {
            case Action.Delete:
                foreach (GameObject thing in Objects) {
                    Destroy(thing);
                }
                break;
            case Action.Create:
                foreach (GameObject thing in Objects)
                {
                    
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
	
	// Update is called once per frame
	void Update () {
		
	}
}
