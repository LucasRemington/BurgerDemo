using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSounds : MonoBehaviour {

    public GameObject gameController;
    public AudioClip[] walk;
    public int step;
    public SoundManager sm;

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        sm = gameController.GetComponent<SoundManager>();
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
        Debug.Log(step);
        sm.audSo.Play();
    }
}
