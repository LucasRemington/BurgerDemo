using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBAnimEvents : MonoBehaviour {

    public GameObject MainCamera;
    public NarrativeManager nm;
    public GameObject IB;

    void Awake () {
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

    void enableBackgrounds ()
    {
        IB.SetActive(true);
    }

    void Choice()
    {
        nm.dbChoiceSS = true;
    }

}
