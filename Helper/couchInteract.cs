using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class couchInteract : MonoBehaviour
{
    [HideInInspector] public GameObject heldItem;
    [SerializeField] private Transform holdPosition; // Position at which the item will be held
    [SerializeField] private float pickupCooldown = 0.5f;

    private float timeStamp;

    public GameObject vrPlayer;
    public GameObject[] players;

    private void Awake()
    {
        vrPlayer = GameObject.Find("playerVR");
        players = GameObject.FindGameObjectsWithTag("CouchPlayers");
    }

    public bool PickUp(GameObject item)
    {
        if (Time.time > timeStamp)
        {
            if (item.GetComponent<FixedJoint>() != null)
                item.GetComponent<FixedJoint>().connectedBody.transform.parent.GetComponent<couchInteract>().Release();
            heldItem = item;

            StartCoroutine(WaitForFrame());
            return true;
        }
        return false;
    }

    private IEnumerator WaitForFrame()
    {
        yield return new WaitForEndOfFrame();
        Rigidbody rbItem = heldItem.GetComponent<Rigidbody>();
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

        // Revert holdPosition back to where it was
        holdPosition.transform.position = transform.position + transform.forward * (transform.GetComponent<Collider>().bounds.size.z / 2);

        heldItem = null; // Set heldItem back to null

        timeStamp = Time.time + pickupCooldown; // Take a timestamp of when the item was released in order to check the pickup cooldown
        //if (reset) ResetThrow();
    }

    // For use in Hold, Throw, and Base states
    protected void SetIgnorePlayerCollisions(bool ignore)
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

}
