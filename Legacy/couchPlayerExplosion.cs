using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class couchPlayerExplosion : MonoBehaviour
{
	private float startTime;
	private float deletionTime = 4;
	private float radius = 1;
	private float force = 10;
	private float upwardForce = 0;
    // Start is called before the first frame update
    void Start()
    {
		//Debug.Log(gameObject.transform.position);
		//Debug.Log("Explosion");
		Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 5);
		foreach(Collider hit in colliders)
		{
			Rigidbody boy = hit.GetComponent<Rigidbody>();
			if(boy != null)
				boy.AddExplosionForce(force, gameObject.transform.position, radius, upwardForce, ForceMode.Impulse);
		}	
		startTime = Time.time;
	}

    // Update is called once per frame
    void Update()
    {
		if(Time.time - startTime >= deletionTime)
		{
			//Debug.Log("Deleting");
			Destroy(gameObject);
		}
    }
}
