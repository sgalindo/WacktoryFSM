using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class couchGroundCollider : MonoBehaviour
{
    public bool isGrounded = false;
    private Rigidbody rb;
    private bool thisFrame;
    [SerializeField] private float wakeUpInterval = 1f; // Rate at which WakeUpRigidbody is called.

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(WakeUpRigidbody(wakeUpInterval));
        StartCoroutine(_WaitForFixedUpdate());
    }

    private void FixedUpdate() { thisFrame = false; }

    private void OnCollisionStay(Collision collision)
    {
        Vector3 normalCol = collision.contacts[0].normal; //gets the normal of the newest contacted surface
        Vector3 vecFlat = new Vector3(normalCol.x, 0, normalCol.z); //the adjacent vertex
        //check the degree of the angle of the collided normal using "Cosine=Adjacent/Horizontal" to calculate the angle
        if (vecFlat.magnitude / normalCol.magnitude * Mathf.Rad2Deg <= 45) { thisFrame = true; } //if we are walking on a slope <=45 degrees, we are grounded
    }

    private IEnumerator _WaitForFixedUpdate()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            isGrounded = thisFrame;
            Debug.Log("Grounded? - " + isGrounded);
        }
    }

    private IEnumerator WakeUpRigidbody(float waitTime)
    {
        while (true)
        {
            if (rb.IsSleeping())
                rb.WakeUp();
            yield return new WaitForSeconds(waitTime);
        }
    }

}
