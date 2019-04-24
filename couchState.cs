using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class couchState : MonoBehaviour
{
    [HideInInspector] public couchPlayer Owner;
    [HideInInspector] public couchMovement move;
    [HideInInspector] public couchActionBox actionBox;
    [HideInInspector] public couchActionBank action;
    [HideInInspector] public couchGroundCollider foot;
    protected couchPlayer.state previousState;

    protected void changeState(couchPlayer.state newState)
    {
        previousState = Owner.currentState;
        Owner.changeState(newState);
        this.enabled = false;
    }
}
