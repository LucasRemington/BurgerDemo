using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour {

    public GameObject playerNoises;
    public AudioClip[] walk;
    public AudioClip climb;
    public int step;
    public SoundManager sm;

    void Start()
    {
        playerNoises = GameObject.FindGameObjectWithTag("PlayerNoises");
        sm = playerNoises.GetComponent<SoundManager>();
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

    public void Climb()
    {
        sm.audSo.clip = climb;
        sm.audSo.Play();
    }
}
