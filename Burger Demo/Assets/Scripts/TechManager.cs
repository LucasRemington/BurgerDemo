using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TechManager : MonoBehaviour {

	void Start () {

        Application.targetFrameRate = 60;
        //StartCoroutine(quitDetect());
	}

    public IEnumerator quitDetect ()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));
        Debug.Log("This is TechManager. This coroutine is active, and you have hit escape. In a build, this would quit the game. Something went wrong if you see this!");
        Application.Quit();
    }

}
