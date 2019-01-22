using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurgerComponentInstantiator : MonoBehaviour {

    public float burgerPosition; //add 0.2 per pixel of the object
    float formerBurgerPosition; //burger position before prefab was instantiated

    public bool canSpawn; //called by animations when they go static
    //bool bottomPlaced; //set when bottom bun is placed - defunct
    bool topPlaced; //set when top bun is placed
    public bool spawnReset; //checked by individual component to despawn
    public int componentNumber; //number of components 
    public int bounceTrigger; //used to determine bouncing
    public int componentCap; //caps components to this int

    public GameObject bottomBun; //all prefabs that are instantiated
    public GameObject tomato;
    public GameObject lettuce;
    public GameObject cheese;
    public GameObject patty;
    public GameObject topBun;

    //called on start
    void Start () {
        canSpawn = true;
        StartCoroutine(ComponentSpawn(KeyCode.Space, 3, bottomBun, 0)); //bottom bun must spawn before others
    }

    IEnumerator ComponentSpawn(KeyCode key, float pixels, GameObject prefab, int identity) 
        //key: corresponding keyboard key 
        //pixels: number of pixels to occupy 
        //prefab: desired prefab 
        //identity: 0 = buns 1 = tomato 2 = lettuce 3 = cheese 4 = patty
    {
        yield return new WaitUntil(() => Input.GetKeyDown(key) == true || topPlaced == true);
        if (topPlaced == false)
        {
            yield return new WaitUntil(() => canSpawn == true);
            canSpawn = false;
            componentNumber++;
            switch (identity) //checks identity of placed prefab to determine position relative to BurgerPosition, and also pass the correct arguments to the coroutine. There's probably a more efficient way to do this.
            {
                case 0: //called only for bottom bun. starts the process
                    Instantiate(prefab, new Vector3(0, burgerPosition, 0), Quaternion.identity);
                    setBurger(pixels);
                    StartCoroutine(ComponentSpawn(KeyCode.A, 1, tomato, 1));
                    StartCoroutine(ComponentSpawn(KeyCode.S, 1, lettuce, 2));
                    StartCoroutine(ComponentSpawn(KeyCode.D, 1, cheese, 3));
                    StartCoroutine(ComponentSpawn(KeyCode.W, 2, patty, 4));
                    StartCoroutine(TopBunSpawn());
                    //bottomPlaced = true;
                    break;
                case 1:
                    Instantiate(prefab, new Vector3(0, burgerPosition - 0.2f, 0), Quaternion.identity);
                    setBurger(pixels);
                    StartCoroutine(ComponentSpawn(KeyCode.A, 1, tomato, 1));
                    break;
                case 2:
                    Instantiate(prefab, new Vector3(0, burgerPosition - 0.2f, 0), Quaternion.identity);
                    setBurger(pixels);
                    StartCoroutine(ComponentSpawn(KeyCode.S, 1, lettuce, 2));
                    break;
                case 3:
                    Instantiate(prefab, new Vector3(0, burgerPosition - 0.4f, 0), Quaternion.identity);
                    setBurger(pixels);
                    StartCoroutine(ComponentSpawn(KeyCode.D, 1, cheese, 3));
                    break;
                case 4:
                    Instantiate(prefab, new Vector3(0, burgerPosition, 0), Quaternion.identity);
                    setBurger(pixels);
                    StartCoroutine(ComponentSpawn(KeyCode.W, 2, patty, 4));
                    break;
            }
        } 
    }

    void setBurger (float pixels) //this is a bit of legacy code, tracking the former burger position. Could be useful in the future, so I kept it
    {
        formerBurgerPosition = burgerPosition;
        burgerPosition = burgerPosition + (0.2f * pixels);
    }

    IEnumerator TopBunSpawn() //Triggers when the cap is reached or space is pressed a second time.
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) == true || (canSpawn == true && componentNumber >= componentCap));
        yield return new WaitUntil(() => canSpawn == true);
        componentNumber = 0;
        canSpawn = false;
        topPlaced = true;
        Instantiate(topBun, new Vector3(0, burgerPosition - 0.2f, 0), Quaternion.identity);
        yield return new WaitForSeconds(1f);
        spawnReset = true;
        //bottomPlaced = false;
        topPlaced = false;
        canSpawn = true;
        burgerPosition = 0;
        formerBurgerPosition = 0;
        yield return new WaitForSeconds(0.1f);
        spawnReset = false;
        StartCoroutine(ComponentSpawn(KeyCode.Space, 3, bottomBun, 0));
        bounceTrigger = 0;
    }

}
