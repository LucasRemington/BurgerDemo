using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    public OverworldMovement owm;
    private Vector3 offset;
    public int yTrack;
    public int[] yPositions;
    public int ySetter;
    public Vector3 cameraTarget;
    public Vector3 xTarget;
    public int zTrack;

    public void PseudoStart ()
    {
        player = GameObject.FindWithTag("Player");
        player = player.transform.Find("OverworldPlayer").gameObject;
        owm = player.GetComponent<OverworldMovement>();
        offset.x = transform.position.x - player.transform.position.x;
    }

    public void Start () 
    {
        snaptoYPosition();
        StartCoroutine(upTrack());
        StartCoroutine(downTrack());
    }

    void LateUpdate()
    {
        xTarget = new Vector3(player.transform.position.x + offset.x, ySetter, zTrack);
        cameraTarget = new Vector3(player.transform.position.x, ySetter, zTrack);
        transform.position = Vector3.Lerp(transform.position, cameraTarget, Time.deltaTime * 3);
    }

    IEnumerator upTrack () {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.UpArrow) && owm.canMove == true);
        if (yTrack < yPositions.Length - 1)
        {
            yTrack++;
        }
        snaptoYPosition();
        yield return new WaitForSeconds(0.05f);
        StartCoroutine(upTrack());
    }

    IEnumerator downTrack()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.DownArrow) && owm.canMove == true);
        if (yTrack >= 1)
        {
            yTrack--;
        }
        snaptoYPosition();
        yield return new WaitForSeconds(0.05f);
        StartCoroutine(downTrack());
    }

    void snaptoYPosition ()
    {
        switch (yTrack)
        {
            case 0:
                ySetter = yPositions[0];
                break;
            case 1:
                ySetter = yPositions[0];
                break;
            case 2:
                ySetter = yPositions[1];
                break;
            case 3:
                ySetter = yPositions[2];
                break;
            case 4:
                ySetter = yPositions[3];
                break;
        }
    }
}