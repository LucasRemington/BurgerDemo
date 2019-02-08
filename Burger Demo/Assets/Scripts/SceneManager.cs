using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour {

    private static SceneManager playerInstance;
    private GameObject UIPlate;
    public AudioSource background;

    void Awake()
    {
        Musical();
        DontDestroyOnLoad(this);
        StartCoroutine(resetScene());
        StartCoroutine(closeScene());
        UIPlate = GameObject.Find("BurgerUIPlate_0");
        if (playerInstance == null)
        {
            playerInstance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Musical()
    {
        background = GetComponent<AudioSource>();
        if (background.isPlaying == false)
        {
            background.Play();
        }
    }

    IEnumerator flickerPlate()
    {
        UIPlate.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        UIPlate.SetActive(true);
    }

    IEnumerator resetScene () {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Equals) == true);
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        UIPlate = GameObject.Find("BurgerUIPlate_0");
        Musical();
        StartCoroutine(resetScene());
    }

    IEnumerator closeScene()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape) == true);
        Application.Quit();
    }
}
