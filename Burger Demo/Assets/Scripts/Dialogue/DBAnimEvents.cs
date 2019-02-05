using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBAnimEvents : MonoBehaviour {

    public GameObject MainCamera;
    public NarrativeManager nm;

    void Start () {
        MainCamera = GameObject.FindWithTag("MainCamera");
        nm = MainCamera.GetComponent<NarrativeManager>();
    }
	
	void StartText ()
    {
        nm.dbStartStop = true;
	}

    void StopText ()
    {
        nm.dbStartStop = true;
    }
}
