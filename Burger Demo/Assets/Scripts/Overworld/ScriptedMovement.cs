using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptedMovement : MonoBehaviour {

    public bool finished = true;
    public Vector3 destination;
    public float overTime;
    public bool random = false;
    public bool stop;

    private float currentTime;


	// Use this for initialization
	void Start () {
        if (random)
        {
            destination = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5));
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.M) && finished) {
            StartCoroutine(MoveTo(this.gameObject, destination, overTime));
        }
        
	}

    void OnDestroy()
    {
        stop = true;
    }

    public IEnumerator MoveTo(GameObject Object, Vector3 movement, float time) {
        finished = false;
        while (currentTime <= time && stop == false) 
            {
            currentTime += Time.deltaTime;
            transform.Translate(new Vector3(Mathf.Lerp(0, movement.x/(60 * time), 1 / time), (Mathf.Lerp(0, movement.y/ (60 * time), 1/time)), 0));
            yield return new WaitForEndOfFrame();
        }
        currentTime = 0;
        finished = true;
        if (random)
        {
            destination = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5));
        }
    }
}
