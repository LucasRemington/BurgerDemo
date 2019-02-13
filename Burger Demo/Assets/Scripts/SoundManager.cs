using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource audSo;
    public AudioClip testClip;

	// Use this for initialization
	void Start () {
        audSo = GetComponent<AudioSource>();
	}
	
}
