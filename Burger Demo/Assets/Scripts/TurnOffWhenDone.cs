using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffWhenDone : MonoBehaviour {

	void TurnOff () {
        this.gameObject.SetActive(false);
	}
}
