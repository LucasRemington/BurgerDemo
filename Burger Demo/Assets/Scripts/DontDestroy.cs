using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour {

    
	void Start () {
        string tempName = gameObject.name;
        gameObject.name += " temp";
        if (!GameObject.Find(tempName))
        {
            DontDestroyOnLoad(this.gameObject);
            gameObject.name = tempName;
        }
        else
            Destroy(gameObject);
            
    }
}
