using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class couchPlayer : MonoBehaviour
{

    public enum state { Base, Carry, Throw, Dead }
    public enum subState { Idle, Moving, Aerial, None }

    [HideInInspector] public state currentState = state.Base;
    [HideInInspector] public subState currentSubState = subState.Idle;


    [SerializeField] private Text DebugText;

    public int playerNum;
    [HideInInspector] public int playerNumber = -1;

    public ControlScheme control;


    //Triggers
    [HideInInspector] public string leftTriggerName;
    [HideInInspector] public string rightTriggerName;

    // Left stick input
    [HideInInspector] public string leftVerticalAxisName;
    [HideInInspector] public string leftHorizontalAxisName;

    // Right stick input
    [HideInInspector] public string rightVerticalAxisName;
    [HideInInspector] public string rightHorizotalAxisName;

    // Right bumper
    [HideInInspector] public string rightBumperName;

    // Bottom Button
    [HideInInspector] public string jumpButtonName;

    [HideInInspector] public string interactButtonName;

    private bool pingHold;

    GameObject ControllerManager;

    private Canvas playerCanvas; //Canvas hosting playerCircle
    private Image playerCircle; // Player's circle image
    private Image pinga; //Player's ping wheel image

    //private couchPlayerInteract interactScript;

    public bool isInteracting; // When the player is interacting with a machine and can't move

    [SerializeField] private Color playerColor;

    //Raw Controller Inputs

    [HideInInspector] public float leftTrigger;
    [HideInInspector] public float rightTrigger;
    [HideInInspector] public float rightBumper;

    /*------------ FSM STATES ------------*/
    couchHold HoldState;
    couchIdle IdleState;
    couchBase BaseState;
    couchDead DeadState;
    couchMoving MovingState;
    couchThrow ThrowState;
    /*------------------------------------*/
    /*--------- CONSTANT SCRIPTS ---------*/
    private couchMovement MovementScript;
    private couchActionBox ActionBox;
    private couchInteract InteractScript;
    /*------------------------------------*/

    public void changeState(state newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case state.Base:
                BaseState.enabled = true;
                break;
            case state.Carry:
                HoldState.enabled = true;
                break;
            case state.Throw:
                ThrowState.enabled = true;
                break;
        }
    }


    public float reMapStart;
    public float reMapTick;

    private Rigidbody rb;

    private void Awake()
    {
        //Configure States
        BaseState = GetComponent<couchBase>();
        HoldState = GetComponent<couchHold>();
        ThrowState = GetComponent<couchThrow>();
        MovementScript = GetComponent<couchMovement>();
        ActionBox = GetComponentInChildren<couchActionBox>();
        InteractScript = GetComponent<couchInteract>();
        BaseState.Owner = HoldState.Owner = ThrowState.Owner = MovementScript.Owner = this;
        BaseState.move = HoldState.move = ThrowState.move = MovementScript;
        BaseState.actionBox = HoldState.actionBox = ThrowState.actionBox = ActionBox;
        BaseState.interact = HoldState.interact = ThrowState.interact = InteractScript;

        BaseState.enabled = HoldState.enabled = ThrowState.enabled = false;
        changeState(state.Base);

        rb = GetComponent<Rigidbody>();

        isInteracting = false;
        pingHold = false;
        ControllerManager = GameObject.Find("ControllerManager");
        reMapStart = ControllerManager.GetComponent<ControllerManager>().reMapStart;
        reMapTick = ControllerManager.GetComponent<ControllerManager>().reMapTick;

        ActionBox = transform.Find("CouchPlayerInteractCollider").GetComponent<couchActionBox>();

        // Find the canvas attached to player
        playerCanvas = transform.Find("Canvas").GetComponent<Canvas>();
        // Find the player's circle image
        playerCircle = playerCanvas.transform.Find("CouchPlayerCircle").GetComponent<Image>();
        // Set its color to player's color
        playerCircle.color = playerColor;
        // Find player's pingle image and set its image to default hidden state
        //pinga = playerCanvas.transform.Find("Pinger").GetComponent<Image>();
        //pinga.gameObject.SetActive(pingHold);
    }

    // Start is called before the first frame update
    void Start()
    {
        reMap();
        InvokeRepeating("reMap", reMapStart, reMapTick);

    }

    private void reMap()
    {
        //set playerNumber to current controllerNumber
        playerNumber = ControllerManager.GetComponent<ControllerManager>().SpecifyJoyStick(playerNum);

        //Get correct controlScheme
        control = ControllerManager.GetComponent<ControllerManager>().specifyType(playerNum);

        // Add the player's number to get the right input from the Input Manager
        leftVerticalAxisName = control.VerticalMovement + playerNumber;
        leftHorizontalAxisName = control.HorizontalMovement + playerNumber;

        rightVerticalAxisName = control.VerticalLook + playerNumber;
        rightHorizotalAxisName = control.HorizontalLook + playerNumber;

        leftTriggerName = control.LeftTrigger + playerNumber;
        rightTriggerName = control.RightTrigger + playerNumber;

        rightBumperName = control.RightBumper + playerNumber;

        jumpButtonName = control.Jump + playerNumber;

        interactButtonName = control.Interact + playerNumber;
        //ControllerManager.GetComponent<ControllerManager>().testSet(playerNum, jumpButtonName);
    }

    // Update is called once per frame
    void Update()
    {
        placeCanvas();

        if (Input.GetAxis(rightBumperName) > 0.0f && !pingHold)
        {
            pingHold = true;
            Ping();
        }

        if (Input.GetAxis(rightBumperName) == 0.0f && pingHold)
        {
            pingHold = false;
            Ping();
        }
    }

    public LayerMask mask;

    private void placeCanvas()
    {
        Ray downRay = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(downRay, out hit, Mathf.Infinity, layerMask: ~mask))
        {
            Vector3 temp = playerCanvas.transform.localPosition;
            temp.y = -1 * hit.distance + 0.015f;
            playerCanvas.transform.localPosition = temp;
            //Debug.Log(hit.collider.gameObject);
        }
    }

    //Ping wheel displays when right bumper is held and turn stick selects pingable
    private void Ping()
    {
        pinga.gameObject.SetActive(pingHold);
    }
}