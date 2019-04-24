//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class couchPlayerIsGrounded : MonoBehaviour
//{

//    private couchPlayer movementScript;
//    private Rigidbody rb;
//    [SerializeField] private float wakeUpInterval = 1f; // Rate at which WakeUpRigidbody is called.

//    // Use this for initialization
//    void Start()
//    {
//        movementScript = transform.parent.GetComponent<couchPlayer>();
//        rb = GetComponent<Rigidbody>();
//        StartCoroutine(WakeUpRigidbody(wakeUpInterval));
//    }

//    // Checks if player is grounded to let them jump.
//    // Courtesy of Dustin Halsey
//    private void OnCollisionStay(Collision collision)
//    {
//        Vector3 normalCol = collision.contacts[0].normal; //gets the normal of the newest contacted surface
//        Vector3 vecFlat = new Vector3(normalCol.x, 0, normalCol.z); //the adjacent vertex
//        //check the degree of the angle of the collided normal using "Cosine=Adjacent/Horizontal" to calculate the angle
//        if (vecFlat.magnitude / normalCol.magnitude * Mathf.Rad2Deg <= 45)
//        { //if we are walking on a slope <=45 degreesp
//            movementScript.isGrounded = true;
//            movementScript.jumped = false;
//        }
//    }

//    private void OnCollisionExit(Collision collision)
//    {
//        movementScript.isGrounded = false;
//        movementScript.jumped = false;
//    }

//    // If the Rigidboy falls asleep, wake it up. 
//    private IEnumerator WakeUpRigidbody(float waitTime)
//    {
//        while (true)
//        {
//            if (rb.IsSleeping())
//                rb.WakeUp();
//            yield return new WaitForSeconds(waitTime);
//        }
//    }
//}
