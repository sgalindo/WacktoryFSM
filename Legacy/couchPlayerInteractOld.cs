////Reed Scriven - SolSearcher
////CouchPlayerInteract.cs
////
////Functionality: 
////  Allows the player to control and use the joystick to control an outcome in the game
////
////How to Attach:
////  tbd
////
////Public Variables:
////  tbd

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class couchPlayerMine : MonoBehaviour
//{
//    /* -----------  Throw Variables  ------------- */
//    [SerializeField] private float minThrowForce = 3.0f; // Minimum force with which an item would be thrown
//    [SerializeField] private float maxThrowForce = 10.0f; // Maximum force with which an item would be thrown
//    [SerializeField] private float chargeRate = 0.1f; // Rate at which a throw is charged
//    [SerializeField] private float throwHeight = 0.5f; // Height of the angle at which an item would be thrown
//    private float throwForce; // How hard the player would throw if the throw button was released
//    private bool charging = false; // If a throw is being charged or not
//    public bool Charging { get { return charging; } }
//    private bool release;

//    /* -----------  Pickup Variables  ------------- */

//    ControlScheme control;
//    private float timestamp;
//    bool held;


//    [SerializeField] private float pickupCooldown = 1.0f; // Cooldown for picking up items (to avoid weird collision glitches)


//    private Transform holdPosition; // Position at which the item will be held
//    private float maxHoldDistance = 2.0f; // Maximum distance a held item can go before dropping it 
//    private bool pickup = false; // If a valid pickup input was made
//    [HideInInspector] public FixedJoint joint; // Whether or not a joint between the hold position and item is made
//    private bool pickupNextFrame = false; // Boolean for if we need to pick up item on next frame.

//    public PhysicMaterial heldItemPhysicMaterial; // Physics material used to disable held item's friction and bounciness to reduce collision anomalies


//    private GameObject heldItem;
//    private bool ragdolling = false;

//    private GameObject interactObject; // the object we interact with
//    public bool interactingWithObject = false; // currently interacting
//    public bool interact = false; // interact available
//    private bool interPressed = false; // multi press prevention
//    //Input name for interacting 
//    private string interactButtonName;
//    public float interactButtonInput;

//    // Jump button used to swing item (only while holding an item).
//    [SerializeField] private float swingForce = 2000f;
//    private string swingButtonName;
//    private float swingButtonInput;
//    private bool swingHalfPressed = false;

//    private string throwButtonName;
//    private float throwButtonInput;
//    private couchPlayer movementScript;
//    private int playerNumber;

//    /* -- Controller Variables -- */
//    private float reMapStart;
//    private float reMapTick;
//    GameObject vrPlayer;
//    GameObject[] players;


//    // Start is called before the first frame update
//    void Start()
//    {
//        holdPosition = gameObject.transform.parent.Find("CouchPlayerHoldPosition");

//        //Find movement script instance and find player numver
//        movementScript = transform.parent.GetComponent<couchPlayer>();
//        playerNumber = movementScript.playerNumber;

//        //Queue up automtic control mapping
//        reMapStart = movementScript.reMapStart;
//        reMapTick = movementScript.reMapTick;
//        reMap();
//        InvokeRepeating("reMap", reMapStart, reMapTick);

//        throwForce = minThrowForce;

//        vrPlayer = GameObject.Find("playerVR");
//        players = GameObject.FindGameObjectsWithTag("CouchPlayers");
//    }

//    //reMaps to correct controller
//    private void reMap()
//    {
//        control = movementScript.control;
//        playerNumber = movementScript.playerNumber;

//        interactButtonName = control.Interact + playerNumber;
//        throwButtonName = control.Throw + playerNumber;
//        swingButtonName = control.Jump + playerNumber;
//    }

//    int counter = 0;
//    // Update is called once per frame
//    void FixedUpdate()
//    {
//        interactButtonInput = Input.GetAxis(interactButtonName);
//        throwButtonInput = Input.GetAxis(throwButtonName);
//        swingButtonInput = Input.GetAxis(swingButtonName);

//        ragdolling = movementScript.ragdolling;

//        /* --- Not interacting and not holding item --- */
//        if (!interactingWithObject && heldItem == null)
//        {
//            if (interactButtonInput == 0)  //Checks if there's no input and resets interpressed. keeps out multi input
//            {
//                interPressed = false; //prevents multipress per frame
//                interact = false; // interact no longer available
//                pickup = false;

//            }

//            /* --- Interact/Pickup Input --- */
//            if (interactButtonInput > 0.0f && !pickup && timestamp <= Time.time && !ragdolling && !interact && !interPressed) // button pressed && interact available && not pressed before
//            {
//                pickup = true;
//                interact = true; // active interaction, locks down button inputs
//                interPressed = true; // can't get a new input until the button is released
//            }

//        }
//        /* --- Interacting, not holding item --- */
//        else if (interactingWithObject)   // if currently interacting with object, press button again to stop
//        {
//            pickup = false;
//            if (interactButtonInput == 0) // multipres checks
//            {
//                interPressed = false;
//            }
//            if (interactButtonInput > 0.0f && !interPressed)
//            {
//                interPressed = true; // can't get a new input until the button is released
//                EndInteraction(); // stops interacting with the gameobject and deletes references
//            }
//        }
//        /* --- Not interacting, holding item --- */
//        else if (heldItem != null)
//        { // Holding item
//            interact = false;
//            if (interactButtonInput == 0f)
//            {
//                held = false;
//                interPressed = false;
//                if (charging == true)
//                {
//                    Throw();
//                    release = false;
//                    counter = 0;
//                }
//                else if (release)
//                {
//                    Release(true);
//                    release = false;
//                    counter = 0;
//                }
//                else
//                {
//                    /* --- Swing Item --- */
//                    if (swingButtonInput > 0.0f && !swingHalfPressed)
//                    {
//                        SwingItem(-heldItem.transform.up * swingForce);
//                        swingHalfPressed = true;
//                    }
//                    if (swingButtonInput == 0f)
//                        swingHalfPressed = false;

//                    /* --- Item Release Conditions --- */
//                    if (Vector3.Distance(heldItem.transform.position, holdPosition.position) >= maxHoldDistance || heldItem == null)
//                    {
//                        Debug.Log("Released from distance: " + (Vector3.Distance(heldItem.transform.position, holdPosition.position) >= maxHoldDistance));
//                        Release(true);
//                    }
//                }
//            }
//            else if (!interPressed || ragdolling || held)
//            {
//                held = true;
//                interPressed = true;
//                /* --- Charge/Throw Item --- */
//                // If we press the throw button, start charging the throw. 
//                // Else, if we are not pressing the throw button but charging is still true (AKA we let go of the throw button), throw item.
//                release = true;
//                counter++;
//                if (counter > 10)
//                {
//                    charging = true;
//                    movementScript.isInteracting = true;
//                    ChargeThrow();
//                }
//            }
//        }
//    }

//    //Check if the interactable object is in front of us
//    private void OnTriggerStay(Collider other)
//    {
//        // If we're pressing the hold button, we're not already holding an item, and this item is grabbable, pick it up.
//        if (pickup && heldItem == null && other.gameObject.tag == "grabbable")
//        {
//            PickUp(other.gameObject);
//        }
//        else if (other.gameObject.tag == "Interactable" && interact && !interactingWithObject) // if object is interactable, interact available, not interacting
//        {
//            //check if the object is already being interacted with
//            if (!other.GetComponent<interactable>().isAlreadyInteracting)
//            {
//                //Want to call interactable parent object specifically the function interact() which all children override 
//                interactObject = other.gameObject; // reference the interacting game object
//                interactingWithObject = true; // interacting with object
//                if (other.GetComponent<interactable>().interact(movementScript) == true) // want to pop out of interacting and not lock down. Toggle switch
//                {
//                    interact = false; // interact not available
//                    interactingWithObject = false; // a tap input so you don't need to give it active inputs
//                }
//                else
//                { // lockdown script
//                    print("This is a lock down interactions. Hands up!");
//                }

//            }

//        }
//    }

//    private void EndInteraction() // ends interaction and deletes references
//    {
//        if (interactObject != null) // makes sure we're looking at an object
//        {
//            interactObject.GetComponent<interactable>().interactEnd(); // calls parent function that's overwritten by child
//            interact = false; // interact not available
//            interactingWithObject = false; // no longer interacting
//        }
//    }


//    private void PickUp(GameObject item)
//    {
//        Rigidbody rbItem = item.GetComponent<Rigidbody>();

//        // This stupid boolean is here because Unity only destroys objects after its current frame. Otherwise the joint isn't completely destroyed, making
//        // the GetComponent<FixedJoint>() != null conditional useless.
//        // So, pickupNextFrame is required to check if we need to pick the item up on the next frame (aka next time this function is called).
//        // If pickupNextFrame is false, this means we are on the first frame of pickup, so we check if the item has a joint and we destroy it,
//        // then we set pickupNextFrame = true for the next time we call it (which would be on the very next frame.
//        // TL;DR - Unity only destroys objects after this frame, so we use a boolean as a 1-frame buffer for Pickup().
//        if (pickupNextFrame)
//        {
//            heldItem = item;
//            // Get the max size of the item's collider (either x-axis  or z-axis) for safer spacing to avoid model clipping through player
//            float itemSize = 0f;
//            foreach (Collider coll in heldItem.GetComponents<Collider>())
//            {
//                itemSize += (coll.bounds.size.z >= coll.bounds.size.x) ? coll.bounds.size.z : coll.bounds.size.x;
//            }

//            // Item ignores collision with players so that there aren't any crazy game-breaking rigidbody bugs like being launched at the speed of light
//            SetIgnorePlayerCollisions(true);

//            // Change the holdPosition's transform to fit the item's size, plus a little bit of padding
//            holdPosition.position = new Vector3(transform.parent.position.x, transform.parent.position.y - 0.2f, transform.parent.position.z)
//                + transform.parent.forward
//                * ((transform.parent.GetComponent<Collider>().bounds.size.z / 2)
//                + (itemSize / 2) + 0.05f);
//            heldItem.transform.rotation = holdPosition.rotation; // Set item's rotation to holdPosition's
//            heldItem.transform.position = holdPosition.position; // Place item in the holdPosition
//            heldItem.GetComponent<Collider>().material = heldItemPhysicMaterial; // Set item's physics material to one without friction and bounciness

//            // To actually hold the object and use still use rigidbody physics, the item is held in place (holdPosition) with a FixedJoint. 
//            // This tells Unity to keep calculating heldItem's rigidbody physics and to not force it through other objects like walls 
//            // (something that would happen if we set it as a child of holdPosition instead)
//            heldItem.AddComponent<FixedJoint>();
//            joint = heldItem.GetComponent<FixedJoint>();
//            joint.GetComponent<FixedJoint>().connectedBody = holdPosition.GetComponent<Rigidbody>();
//            //Debug.Log("New joint: " + joint.connectedBody.transform.parent.name);
//            //Debug.Log(transform.parent.name + " picked up: " + heldItem.name);
//            pickupNextFrame = false;
//        }
//        else if (rbItem != null && joint == null)
//        {
//            if (item.GetComponent<FixedJoint>() != null)
//            {
//                item.GetComponent<FixedJoint>().connectedBody.transform.parent.Find("CouchPlayerInteractCollider").GetComponent<couchPlayerInteract>().Release(true);
//            }
//            pickupNextFrame = true;
//        }
//    }

//    public void Release(bool resetThrow)
//    {
//        // Destroy the FixedJoint that held the item in place
//        pickup = false;
//        Destroy(heldItem.GetComponent<FixedJoint>());
//        joint = null;
//        //Debug.Log(transform.parent.name + " released: " + heldItem.name);

//        // Reset the item's physics material to null (resets its friction and bounciness)
//        heldItem.GetComponent<Collider>().material = null;

//        // Re-enable collision with player and VR player
//        SetIgnorePlayerCollisions(false);

//        // Revert holdPosition back to where it was
//        holdPosition.transform.position = transform.parent.position + transform.parent.forward * (transform.parent.GetComponent<Collider>().bounds.size.z / 2);

//        heldItem = null; // Set heldItem back to null

//        timestamp = Time.time + pickupCooldown; // Take a timestamp of when the item was released in order to check the pickup cooldown
//        if (resetThrow) ResetThrow();
//    }

//    private void Throw()
//    {
//        Rigidbody rbItem = heldItem.GetComponent<Rigidbody>();
//        Release(false); // Release the item right before applying a force to it
//        if (rbItem != null)
//        {
//            // Throw direction is the player's forward direction plus throwHeight to increase the arc
//            Vector3 throwDirection = new Vector3(transform.forward.x, transform.forward.y + throwHeight, transform.forward.z);

//            // Throw the item at throwForce strength
//            rbItem.AddForce(throwDirection * throwForce, ForceMode.Impulse);
//        }
//        ResetThrow();
//    }

//    // If throwForce is >= maxThrowForce, cap throwForce at maxThrowForce
//    // Else, increase throwForce by chargeRate
//    private int chargeSign = 1;
//    private void ChargeThrow()
//    {
//        if (throwForce > maxThrowForce || throwForce < minThrowForce)
//        {
//            chargeSign *= -1;
//        }
//        throwForce += (chargeRate * chargeSign);
//        DrawThrowLine();
//    }

//    // Resets all the values for throwing.
//    private void ResetThrow()
//    {
//        // Reset throwForce to minimum throw force value
//        throwForce = minThrowForce;
//        chargeSign = 1;
//        charging = false;
//        movementScript.isInteracting = false;

//        // Clear the charging line renderer
//        ClearThrowLine();
//    }

//    /*----DRAWING CHARGE LINE FOR THROWING----*/

//    public Gradient lineGradient;
//    public float segmentTime = 0.05f;
//    public int maxSegments = 100;

//    // Draw line that indicates how much force a throw will have
//    private void DrawThrowLine()
//    {
//        // Check if there is already a LineRenderer component in the holdPosition object
//        // If not, create a new one and set its attributes.
//        LineRenderer line = holdPosition.gameObject.GetComponent<LineRenderer>();
//        if (!line)
//        {
//            holdPosition.gameObject.AddComponent<LineRenderer>();
//            line = holdPosition.gameObject.GetComponent<LineRenderer>();
//            line.startWidth = line.endWidth = 0.1f;
//            line.receiveShadows = false;
//            line.material = (Material)Resources.Load("Line", typeof(Material));
//            line.colorGradient = lineGradient;
//            //Debug.Log("Got em");
//        }
//        Vector3 throwDirection = new Vector3(transform.forward.x, transform.forward.y + throwHeight, transform.forward.z);
//        float mass = heldItem.GetComponent<Rigidbody>().mass;
//        updateLine(line, segmentTime, maxSegments, heldItem.GetComponent<Rigidbody>(), holdPosition.gameObject.transform.position, throwForce * throwDirection * (1 / mass), Physics.gravity);

//    }
//    //gfoot's methodology used as a basis https://forum.unity.com/threads/projectile-prediction-line.143636/
//    public LayerMask mask;
//    public Canvas laserPoint;
//    void updateLine(LineRenderer line, float segmentTime, int maxSegments, Rigidbody thrownItem, Vector3 initialPosition, Vector3 initialVelocity, Vector3 gravity)
//    {
//        float timeDelta = segmentTime / initialVelocity.magnitude; // for example

//        Vector3 lastPositon;
//        Vector3 position = initialPosition;
//        Vector3 velocity = initialVelocity;
//        RaycastHit hit;
//        float distance;
//        float drag = thrownItem.drag;
//        Vector3 direction;
//        for (int i = 0; i < maxSegments; ++i)
//        {
//            line.positionCount = i + 1;
//            line.SetPosition(i, position);

//            lastPositon = position;
//            position += velocity * timeDelta + 0.5f * gravity * timeDelta * timeDelta;
//            distance = Vector3.Distance(lastPositon, position);
//            direction = (lastPositon - position).normalized;
//            if (Physics.Raycast(lastPositon, direction, out hit, distance, layerMask: ~mask))
//            {
//                if (hit.collider.gameObject != gameObject && hit.collider.gameObject != transform.parent.gameObject)
//                {
//                    //laserPoint.transform.position = hit.point + new Vector3(0, .015f, 0);
//                    //laserPoint.transform.rotation = Quaternion.LookRotation(hit.normal);
//                    //Debug.Log(hit.normal);
//                    //laserPoint.GetComponent<CanvasGroup>().alpha = 1;
//                    //Debug.Log(hit.normal);
//                    break;
//                }
//            }
//            velocity += gravity * timeDelta;
//            velocity *= (1 - (drag * timeDelta));
//            //laserPoint.GetComponent<CanvasGroup>().alpha = 0;
//        }
//    }

//    private void ClearThrowLine()
//    {
//        LineRenderer line = holdPosition.gameObject.GetComponent<LineRenderer>();
//        if (line != null)
//        {
//            Destroy(line);
//        }
//        laserPoint.GetComponent<CanvasGroup>().alpha = 0;
//    }

//    // Enable or disable heldItem collisions with couch players and VR player.
//    private void SetIgnorePlayerCollisions(bool ignore)
//    {
//        Collider[] colliders = heldItem.GetComponents<Collider>();
//        foreach (Collider coll in colliders)
//        {
//            for (int i = 0; i < players.Length; i++)
//            {
//                Physics.IgnoreCollision(players[i].GetComponent<Collider>(), coll, ignore);
//            }
//            if (vrPlayer && heldItem)
//            {
//                Physics.IgnoreCollision(vrPlayer.GetComponent<Collider>(), coll, ignore);
//            }
//        }
//    }

//    private void SwingItem(Vector3 swingVelocity)
//    {
//        if (heldItem != null)
//        {
//            Rigidbody itemRb = heldItem.GetComponent<Rigidbody>();
//            itemRb.AddForce(swingVelocity, ForceMode.Acceleration);

//            Debug.Log("Item swung.");
//        }
//    }
//}

