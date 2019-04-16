using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class couchPlayerLifeAndDeath : MonoBehaviour
{
    [HideInInspector]public Vector3 startingPosition;
    [HideInInspector] public Quaternion startingRotation;
    [HideInInspector] public bool alive;
    public float checkTime = 1.0f;
	public GameObject gibs;
	private Vector3 gibPosition;

    GameObject GameManager;
    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        GameManager = GameObject.Find("GameManager");
        alive = true;
    }

    // Update is called once per frame
    /* void Update()
    {
        if (transform.position.y < startingPosition.y - 15)
        {
            GameManager.GetComponent<PlayerManager>().playerDeath(gameObject); // Kills the player
        }
    }
    */
    
    public void KillThisPlayer()
    {
		gibPosition = gameObject.transform.position;
		Instantiate(gibs, gibPosition, Quaternion.identity);
		GameManager.GetComponent<PlayerManager>().playerDeath(gameObject); // Kills the player
    }
}
