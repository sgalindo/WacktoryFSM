using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class couchActionBank : MonoBehaviour
{

    [Header("Hold Variables")]
    [HideInInspector] public GameObject heldItem;
    [SerializeField] private Rigidbody heldRB;
    [SerializeField] private Transform holdPosition; // Position at which the item will be held
    [SerializeField] private float maxHoldDistance = 2.0f; // Maximum distance a held item can go before dropping it 
    [SerializeField] private float pickupCooldown = 0.5f;

    [Header("Throw Variables")]
    [SerializeField] private projectionGuide pathProjector;
    [SerializeField] private float minThrowForce = 3.0f; // Minimum force with which an item would be thrown
    [SerializeField] private float maxThrowForce = 10.0f; // Maximum force with which an item would be thrown
    [SerializeField] private float chargeRate = 0.05f; // Rate at which a throw is charged
    [SerializeField] private float throwHeight = 0.4f; // Height of the angle at which an item would be thrown

    [Header("Interact Variables")]
    [HideInInspector] public GameObject interItem;


    private float timeStamp;

    public GameObject vrPlayer;
    public GameObject[] players;

    private couchPlayer owner;

    private void Awake()
    {
        throwForce = minThrowForce;
        vrPlayer = GameObject.Find("playerVR");
        players = GameObject.FindGameObjectsWithTag("CouchPlayers");
    }

    private void FixedUpdate()
    {
        if (heldItem)
            if (Vector3.Distance(heldItem.transform.position, holdPosition.position) >= maxHoldDistance) { Release(); CancelThrow(); }
    }




    /*---------------- INTERACT FUNCTIONS -----------------*/
    public bool Interact(GameObject item)
    {
        if(!item.GetComponent<interactable>().isAlreadyInteracting && item.tag == "Interactable")
        {
            interItem = item;
            item.GetComponent<interactable>().interact(owner);
            return true;
        }
        return false;
    }

    private void EndInteraction() // ends interaction and deletes references
    {
        if (interItem != null) // makes sure we're looking at an object
        {
            interItem.GetComponent<interactable>().interactEnd(); // calls parent function that's overwritten by child
            //interact = false; // interact not available
            //interactingWithObject = false; // no longer interacting
        }
    }

    /*------------------ CARRY FUNCTIONS ------------------*/
    public bool PickUp(GameObject item)
    {
        if (Time.time > timeStamp && item.tag == "grabbable")
        {
            if (item.GetComponent<FixedJoint>() != null)
            {
                couchActionBank temp = item.GetComponent<FixedJoint>().connectedBody.transform.parent.GetComponent<couchActionBank>();
                temp.Release();
                temp.CancelThrow();
            }
            heldItem = item;
            heldRB = heldItem.GetComponent<Rigidbody>();

            StartCoroutine(FinishPickup()); //Will finish at end of frame
            return true;
        }
        return false;
    }

    private IEnumerator FinishPickup()
    {
        yield return new WaitForEndOfFrame();
        float itemSize = 0f;
        foreach (Collider coll in heldItem.GetComponents<Collider>())
            itemSize += (coll.bounds.size.z >= coll.bounds.size.x) ? coll.bounds.size.z : coll.bounds.size.x;

        // heldItem ignores collision with players so that there aren't any crazy game-breaking rigidbody bugs like being launched at the speed of light
        SetIgnorePlayerCollisions(true);
        holdPosition.position = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z)
                + transform.forward
                * ((transform.GetComponent<Collider>().bounds.size.z / 2)
                + (itemSize / 2) + 0.05f);
        heldItem.transform.rotation = holdPosition.rotation; // Set heldItem's rotation to holdPosition's
        heldItem.transform.position = holdPosition.position; // Place heldItem in the holdPosition
        PhysicMaterial mat = heldItem.GetComponent<Collider>().material;
        mat.staticFriction = mat.bounciness = 0; // Set heldItem's physics material to one without friction and bounciness

        heldItem.AddComponent<FixedJoint>();
        heldItem.GetComponent<FixedJoint>().connectedBody = holdPosition.GetComponent<Rigidbody>();
    }


    public void Release()
    {
        // Destroy the FixedJoint that held the item in place
        Destroy(heldItem.GetComponent<FixedJoint>());

        // Reset the item's physics material to null (resets its friction and bounciness)
        heldItem.GetComponent<Collider>().material = null;

        // Re-enable collision with player and VR player
        SetIgnorePlayerCollisions(false);

        heldRB.AddForce(transform.forward * .01f, ForceMode.Impulse);
        // Revert holdPosition back to where it was
        holdPosition.transform.position = transform.position + transform.forward * (transform.GetComponent<Collider>().bounds.size.z / 2);

        heldItem = null; // Set heldItem back to null
        heldRB = null;

        timeStamp = Time.time + pickupCooldown; // Take a timestamp of when the item was released in order to check the pickup cooldown
        //if (reset) ResetThrow();
    }
    

    private void SetIgnorePlayerCollisions(bool ignore)
    {
        Collider[] colliders = heldItem.GetComponents<Collider>();
        foreach (Collider coll in colliders)
        {
            for (int i = 0; i < players.Length; i++)
                Physics.IgnoreCollision(players[i].GetComponent<Collider>(), coll, ignore);
            if (vrPlayer && heldItem)
                Physics.IgnoreCollision(vrPlayer.GetComponent<Collider>(), coll, ignore);
        }
    }



    /*------------------ THROW FUNCTIONS ------------------*/
    private int chargeSign = 1;
    private float throwForce; // How hard the player would throw if the throw button was released
    public void ChargeThrow()
    {
        if (throwForce > maxThrowForce || throwForce < minThrowForce)
            chargeSign *= -1;
        throwForce += (chargeRate * chargeSign);
    }

    public void DrawThrow()
    {
        pathProjector.DrawThrow(heldItem.transform.position, ThrowDirection() * throwForce, heldRB.drag, heldRB.mass);
    }

    public void CancelThrow() { ResetThrow(); }

    public void Throw()
    {
        if (heldItem)
        {
            // Throw the item at throwForce strength
            heldRB.AddForce(ThrowDirection() * throwForce, ForceMode.Impulse);
            Release();
        }
        ResetThrow();
    }

    // Reset throwForce to minimum throw force value
    private void ResetThrow()
    {
        throwForce = minThrowForce;
        chargeSign = 1;
        pathProjector.ToggleLineVisibility(false);
    }

    // Throw direction is the player's forward direction plus throwHeight to increase the arc
    private Vector3 ThrowDirection() { return new Vector3(transform.forward.x, transform.forward.y + throwHeight, transform.forward.z); }
}
