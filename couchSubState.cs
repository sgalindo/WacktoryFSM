using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class couchSubState : MonoBehaviour
{
    [HideInInspector] public couchPlayer Owner;
    [HideInInspector] public couchMovement move;
    [HideInInspector] public couchGroundCollider foot;
    protected couchPlayer.subState previousSub;

    protected void changeSub(couchPlayer.subState newSub)
    {
        previousSub = Owner.currentSub;
        Owner.changeSub(newSub);
        this.enabled = false;
    }
}
