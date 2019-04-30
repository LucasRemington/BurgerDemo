using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoftReset : MonoBehaviour
{

    public bool hasReset; // A toggle so we don't reset more than one time at once.
    public GameObject baseObj; // The object "Base"

    private void Start()
    {
        baseObj = GameObject.FindGameObjectWithTag("Base"); // Grab base initially.
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.T) && !hasReset && SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0)) // Start our reset function if we're holding the PAT keys and we're not in the main menu.
        {
            Debug.Log("Reset game!");
            StartCoroutine(ResetGame());
        }

        if (!Input.GetKey(KeyCode.P) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.T) && hasReset) // If we release the keys, toggle back our bool.
        {
            Debug.Log("Reset released.");
            hasReset = false;
        }

        if (baseObj == null) // And if our base is null (ie, after we reset), grab it again
        {
            baseObj = GameObject.FindGameObjectWithTag("Base");
        }
    }


    public IEnumerator ResetGame() // Go back to main menu! First we make sure Base isn't Don'tDestroyOnLoad by removing the relevant script and changing its parent object to a temp empty object, then we load the scene.
    {
        hasReset = true;

        Destroy(baseObj.GetComponent<DontDestroy>());

        GameObject tempObj = new GameObject();
        baseObj.transform.parent = tempObj.transform;

        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene(0);
    }
}
