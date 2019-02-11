using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneSwitcher : MonoBehaviour {

    public GameObject gameController;
    public int RoomComingFrom;
    public Scene RoomGoingTo;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ToTheNextScene() {
        SceneManager.LoadSceneAsync(RoomGoingTo.ToString());
        //gameController.GetComponent<BattleTransitions>().entrypoint = RoomComingTo;                       // Fix this after, its to decide where the player spawns in the next scene
    }
}
