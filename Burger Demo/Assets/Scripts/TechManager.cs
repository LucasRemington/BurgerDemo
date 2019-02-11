using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TechManager : MonoBehaviour {

	void Start () {

        Application.targetFrameRate = 60;
        StartCoroutine(quitDetect());
	}

    public IEnumerator quitDetect ()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));
        Application.Quit();
    }

}
