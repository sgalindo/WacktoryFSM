//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class couchPlayerOld : MonoBehaviour
//{

//    [SerializeField] private Text DebugText;


//    public int playerNum;
//    [HideInInspector] public int playerNumber = -1;

//    private float moveSpeed = 0; //current speed
//    [SerializeField] private float maxMoveSpeed = 50.0f;
//    [SerializeField] private float acceleration = 0.15f;
//    [SerializeField] private float turnSpeed = 0.1f;
//    [SerializeField] private float friction = 1200.0f;
//    [SerializeField] private float gravity = 3.0f;
//    [SerializeField] private float jumpForce = 350.0f;
//    [SerializeField] private float timeToGetUp = 3.0f;
//    public static float deadzone = 0.4f;

//    public ControlScheme control;

//    private Collider coll;

//    [HideInInspector] public bool explosion; // When the player is being affected by an explosion. This is accessed by the explosion.cs script

//    [HideInInspector] public bool ragdolling; // When the player is ragdolling.

//    //Raw Controller Inputs
//    [HideInInspector] public float leftHorizontalAxis;
//    [HideInInspector] public float leftVerticalAxis;
//    [HideInInspector] public float rightHorizotalAxis;
//    [HideInInspector] public float rightVerticalAxis;
//    [HideInInspector] public float leftTrigger;
//    [HideInInspector] public float rightTrigger;
//    [HideInInspector] public float rightBumper;

//    //Triggers
//    private string leftTriggerName;
//    private string rightTriggerName;

//    // Left stick input
//    private string leftVerticalAxisName;
//    private string leftHorizontalAxisName;
//    public Vector3 movementInput; // Vector3 that holds the left stick input for this player
//    private float magnitude; //taking into accound deadzones

//    // Right bumper
//    private string rightBumperName;

//    // Right stick input
//    private string rightVerticalAxisName;
//    private string rightHorizotalAxisName;
//    public Vector3 turnInput; // Vector3 that holds the right stick input for this player

//    private string jumpButtonName;
//    private bool jumpHalfPressed;
//    private bool pingHold;
//    private Rigidbody rb;

//    [HideInInspector] public bool isGrounded;  // Checks if the player is on some level surface to jump from
//    [HideInInspector] public bool jumped;      // If player already jumped (is in midair)

//    public bool isInteracting; // When the player is interacting with a machine and can't move

//    private IEnumerator ragdoll; // IEnumerator reference that we can use to check if the ragdoll coroutine is null

//    private Quaternion turnAngle = new Quaternion(); // Angle to turn to automatically

//    public PhysicMaterial physMat;

//    GameObject ControllerManager;

//    private couchPlayerInteract interactScript;

//    private Canvas playerCanvas; //Canvas hosting playerCircle
//    private Image playerCircle; // Player's circle image
//    private Image pinga; //Player's ping wheel image

//    [SerializeField] private Color playerColor;

//    private void Awake()
//    {
//        rb = GetComponent<Rigidbody>();
//        isGrounded = false;
//        jumped = false;
//        isInteracting = false;
//        explosion = false;
//        ragdolling = false;
//        jumpHalfPressed = false;
//        pingHold = false;

//        ControllerManager = GameObject.Find("ControllerManager");
//        reMapStart = ControllerManager.GetComponent<ControllerManager>().reMapStart;
//        reMapTick = ControllerManager.GetComponent<ControllerManager>().reMapTick;

//        interactScript = transform.Find("CouchPlayerInteractCollider").GetComponent<couchPlayerInteract>();

//        // Find the canvas attached to player
//        playerCanvas = transform.Find("Canvas").GetComponent<Canvas>();
//        // Find the player's circle image
//        playerCircle = playerCanvas.transform.Find("CouchPlayerCircle").GetComponent<Image>();
//        // Set its color to player's color
//        playerCircle.color = playerColor;
//        // Find player's pingle image and set its image to default hidden state
//        pinga = playerCanvas.transform.Find("Pinger").GetComponent<Image>();
//        pinga.gameObject.SetActive(pingHold);
//    }

//    public float reMapStart;
//    public float reMapTick;



//    private void Start()
//    {
//        reMap();
//        //Queue up automtic control mapping
//        InvokeRepeating("reMap", reMapStart, reMapTick);

//        //Other code
//        rb.constraints = RigidbodyConstraints.FreezeRotation;
//        coll = GetComponent<Collider>();
//        physMat = coll.material;
//    }

//    private void reMap()
//    {
//        //set playerNumber to current controllerNumber
//        playerNumber = ControllerManager.GetComponent<ControllerManager>().SpecifyJoyStick(playerNum);

//        //Get correct controlScheme
//        control = ControllerManager.GetComponent<ControllerManager>().specifyType(playerNum);

//        // Add the player's number to get the right input from the Input Manager
//        leftVerticalAxisName = control.VerticalMovement + playerNumber;
//        leftHorizontalAxisName = control.HorizontalMovement + playerNumber;

//        rightVerticalAxisName = control.VerticalLook + playerNumber;
//        rightHorizotalAxisName = control.HorizontalLook + playerNumber;

//        leftTriggerName = control.LeftTrigger + playerNumber;
//        rightTriggerName = control.RightTrigger + playerNumber;

//        rightBumperName = control.RightBumper + playerNumber;

//        jumpButtonName = control.Jump + playerNumber;
//        ControllerManager.GetComponent<ControllerManager>().testSet(playerNum, jumpButtonName);
//    }

//    private void Update()
//    {
//        //DebugText.text = leftVerticalAxisName + "\n" + leftVerticalAxis;

//        //updating the controller inputs that can be grabbed by other scripts
//        leftHorizontalAxis = Input.GetAxisRaw(leftHorizontalAxisName);
//        leftVerticalAxis = Input.GetAxisRaw(leftVerticalAxisName);
//        rightHorizotalAxis = Input.GetAxisRaw(rightHorizotalAxisName);
//        rightVerticalAxis = Input.GetAxisRaw(rightVerticalAxisName);
//        leftTrigger = Input.GetAxisRaw(leftTriggerName);
//        rightTrigger = Input.GetAxisRaw(rightTriggerName);
//        rightBumper = Input.GetAxisRaw(rightBumperName);


//        // Store input to movementInput vector3 every frame.
//        movementInput = new Vector3(leftHorizontalAxis, 0f, leftVerticalAxis);
//        turnInput = new Vector3(rightHorizotalAxis, 0f, rightVerticalAxis);

//        //Debug.Log("Magnitude" + movementInput.magnitude);

//        // If the jump button is pressed, jump.
//        // Could not use GetButtonDown properly since for some reason it would always allow player to double jump.
//        if (Input.GetAxis(jumpButtonName) > 0.0f && !isInteracting && interactScript.joint == null && !jumpHalfPressed)
//        {
//            jumpHalfPressed = true;
//            Jump();
//        }

//        // If the jump button was released, reset jump input
//        if (Input.GetAxis(jumpButtonName) == 0.0f && jumpHalfPressed)
//        {
//            jumpHalfPressed = false;
//        }

//        if (Input.GetAxis(rightBumperName) > 0.0f && !pingHold)
//        {
//            pingHold = true;
//            Ping();
//        }

//        if (Input.GetAxis(rightBumperName) == 0.0f && pingHold)
//        {
//            pingHold = false;
//            Ping();
//        }


//        // If there was an explosion, start ragdoll coroutine (ragdoll for some amount of time)
//        if (explosion)
//        {
//            Explode();
//        }

//        placeCanvas();
//    }

//    private void FixedUpdate()
//    {
//        // Handles movement and automatic turning.
//        if (!ragdolling)
//        {
//            if (!isInteracting)
//            {
//                MovePlayer();
//                TurnPlayer();
//            }
//            else if (interactScript.Charging)
//            {
//                TurnPlayer();
//            }

//            if (isGrounded)
//            {
//                // Only apply friction if the player is grounded
//                ApplyFriction();
//            }
//            else
//            {
//                // Only apply gravity if the player is not grounded
//                ApplyGravity();
//            }
//        }
//    }

//    //int[] mask = new int[] { 11 };

//    public LayerMask mask;

//    private void placeCanvas()
//    {
//        Ray downRay = new Ray(transform.position, Vector3.down);
//        RaycastHit hit;
//        if (Physics.Raycast(downRay, out hit, Mathf.Infinity, layerMask: ~mask))
//        {
//            Vector3 temp = playerCanvas.transform.localPosition;
//            temp.y = -1 * hit.distance + 0.015f;
//            playerCanvas.transform.localPosition = temp;
//            Debug.Log(hit.collider.gameObject);
//        }
//    }

//    // Function that finds the turn angle for the player based on their inputs 
//    // From user "YoungDeveloper" in:
//    // https://answers.unity.com/questions/1032673/how-to-get-0-360-degree-from-two-points.html
//    // ----------------------------------------------------------------------------------------
//    float calculateAngle(float x, float y)
//    {
//        float angle = (Mathf.Atan2(x, y) / Mathf.PI) * 180f;
//        if (angle < 0)
//        {
//            angle += 360;
//        }
//        return angle;
//    }
//    // ---------------------------------------------------------------------------------------

//    // Handle player movement
//    private void MovePlayer()
//    {
//        // New vector3 with 0 y value since we dont care about that direction. 
//        Vector3 rbVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
//        magnitude = Mathf.Clamp(movementInput.magnitude, deadzone, 1);
//        float temp = Mathf.Pow((magnitude - deadzone) / (1 - deadzone), 2);

//        if (temp <= 0.5f)
//            moveSpeed = Mathf.Max(temp * maxMoveSpeed, 0.4f * maxMoveSpeed);
//        else if (moveSpeed < maxMoveSpeed)
//        {
//            if (temp == 1)
//                moveSpeed += temp * acceleration;
//            if (moveSpeed >= maxMoveSpeed)
//                moveSpeed = maxMoveSpeed;
//        }


//        // If our player has not reached their max speed (maxMoveSpeed), add movement to it.
//        // Minimum magnitude needed from control sticks set to 0.2f specifically so slightly biased controllers don't automatically
//        // move or rotate players and so there is not need for substantion deadzones on axes. Fixes controller feel quite a bit -Zack
//        if (movementInput.magnitude > deadzone && ((rbVelocity + movementInput).magnitude < maxMoveSpeed / 10 || (rbVelocity + movementInput).magnitude < rbVelocity.magnitude))
//        {
//            rb.AddForce(movementInput * moveSpeed);
//        }
//    }

//    // Handle automatic turning
//    bool justHappened = false;
//    int count = 0;
//    float prevAngle = 0;
//    float moveAngle;

//    private void TurnPlayer()
//    {
//        float angleToTurnTo;
//        // If movement is being applied from the input, calculate the angle that we need to turn to.
//        // Minimum magnitude needed from control sticks set to 0.2f specifically so slightly biased controllers don't automatically
//        // move or rotate players and so there is not need for substantion deadzones on axes. Fixes controller feel quite a bit -Zack

//        if (turnInput.magnitude > deadzone)
//        {
//            angleToTurnTo = calculateAngle(turnInput.x, turnInput.z);
//            turnAngle = Quaternion.Euler(transform.rotation.x, angleToTurnTo, transform.rotation.z);
//            justHappened = false;
//            count = 0;
//        }
//        else if (movementInput.magnitude > deadzone)
//        {
//            float moveAngle = calculateAngle(movementInput.x, movementInput.z);
//            float upperBound1 = prevAngle + 90;
//            float upperBound2 = upperBound1;
//            float lowerBound1 = prevAngle - 90;
//            float lowerBound2 = lowerBound1;
//            if (lowerBound1 < 0)
//                lowerBound1 = 360 - lowerBound1;
//            if (upperBound2 > 360)
//                upperBound2 = upperBound2 - 360;
//            if ((moveAngle > lowerBound1 && moveAngle < upperBound1) || ((moveAngle > lowerBound2 && moveAngle < upperBound2)))
//                count++;
//            else
//                count = 0;
//            prevAngle = moveAngle;
//            if (count > 2)
//            {
//                angleToTurnTo = moveAngle;
//                turnAngle = Quaternion.Euler(transform.rotation.x, angleToTurnTo, transform.rotation.z);
//            }

//        }
//        else
//        {
//            justHappened = false;
//            count = 0;
//        }


//        // Lerp to the turnAngle (smooth turning)
//        transform.rotation = Quaternion.Lerp(transform.rotation, turnAngle, turnSpeed);
//    }

//    // Simply applies a Vector that is an inverse of the player's velocity * some predefined friction
//    private void ApplyFriction()
//    {
//        rb.AddForce(-rb.velocity.normalized * friction * Time.deltaTime, ForceMode.Acceleration);
//    }

//    // Applies stronger gravity to make jumping / falling feel better
//    private void ApplyGravity()
//    {
//        rb.AddForce(Vector3.down * 9.81f * gravity);
//    }

//    // Only jump if we are grounded and have not already jumped (not in midair)
//    private void Jump()
//    {
//        if (isGrounded && !jumped)
//        {
//            jumped = true;
//            rb.AddForce(Vector3.up * jumpForce);
//        }
//    }

//    //Ping wheel displays when right bumper is held and turn stick selects pingable
//    private void Ping()
//    {
//        pinga.gameObject.SetActive(pingHold);
//    }

//    // Coroutine that handles ragdolling
//    private IEnumerator Ragdoll()
//    {
//        ragdolling = true;
//        coll.material.dynamicFriction = 0.2f; // Change the value of the player's friction so they don't slide around while ragdolling
//        coll.material.bounciness = 0.5f;      // Add some bounciness for fun

//        yield return new WaitForSeconds(timeToGetUp); // Wait specified amount of time to get up

//        GetUp();
//        coll.material.dynamicFriction = 0.0f; // Change friction and bounciness back to normal
//        coll.material.bounciness = 0.0f;
//        ragdolling = false;
//    }

//    // Handles resetting the player's position / rotation after ragdolling
//    private void GetUp()
//    {
//        Quaternion resetAngle = Quaternion.identity;
//        transform.rotation = Quaternion.Lerp(transform.rotation, resetAngle, turnSpeed);
//        rb.constraints = RigidbodyConstraints.FreezeRotation;
//        transform.Find("CouchPlayerHoldPosition").transform.rotation = transform.rotation;
//    }

//    // Handles player death. Right now, it only disables the player's movement.
//    public void Kill()
//    {
//        rb.constraints = RigidbodyConstraints.None;
//        ragdolling = true;
//        coll.material.dynamicFriction = 0.2f;
//        coll.material.bounciness = 0.5f;
//    }

//    private void Explode()
//    {
//        explosion = false;
//        if (ragdoll != null)
//            StopCoroutine(ragdoll);
//        ragdoll = Ragdoll();
//        StartCoroutine(ragdoll);
//    }

//}