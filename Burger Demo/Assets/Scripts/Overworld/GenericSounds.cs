using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSounds : MonoBehaviour {

    public GameObject GameController;
    public AudioClip[] walk;
    public int step;
    public SoundManager sm;

    void Start()
    {
        GameController = GameObject.FindGameObjectWithTag("GameController");
        sm = GameController.GetComponent<SoundManager>();
    }

    public void Step()
    {
        if (step < 1)
        {
            step++;
        }
        else
        {
            step = 0;
        }
        sm.audSo.clip = walk[step];
        //Debug.Log(step);
        sm.audSo.Play();
    }

}
