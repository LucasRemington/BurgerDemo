using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour {

    public NarrativeManager nm;
    public AudioSource audSo;
    public AudioClip[] backgroundMusicByRoom;

	void Start () {
        audSo = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        audSo.clip = backgroundMusicByRoom[nm.room];
        audSo.Play();
    }


    

}
