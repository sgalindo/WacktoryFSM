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
        prepareForDeath();
		GameManager.GetComponent<PlayerManager>().playerDeath(gameObject); // Kills the player
    }




























    private void prepareForDeath()
    {
        Debug.Log("Did you ever hear the tragedy of Darth Plagueis The Wise? I thought not. \nIt's not a story the Jedi would tell you. It's a Sith legend. Darth Plagueis \nwas a Dark Lord of the Sith, so powerful and so wise he could use the Force \nto influence the midichlorians to create life… He had such a knowledge of \nthe dark side that he could even keep the ones he cared about from dying. \nThe dark side of the Force is a pathway to many abilities some consider to \nbe unnatural. He became so powerful… the only thing he was afraid of was \nlosing his power, which eventually, of course, he did. Unfortunately, he \ntaught his apprentice everything he knew, then his apprentice killed him \nin his sleep. Ironic. He could save others from death, but not himself.");
    }
}
