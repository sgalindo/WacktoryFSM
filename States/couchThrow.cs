using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class couchThrow : couchState
{
     /*
        Zackvador is a fucking God.
     */

    // Update is called once per frame
    void Update()
    {
        if (action.heldItem == null) { changeState(couchPlayer.state.Base); return; }
        if (Input.GetButtonDown(Owner.jumpButtonName))
        {
            Debug.Log("Please");
            action.CancelThrow();
            changeState(couchPlayer.state.Carry);
            return;
        }
        if(Input.GetAxisRaw(Owner.interactButtonName) == 0)
        {
            action.Throw();
            changeState(couchPlayer.state.Base);
            return;
        }
        action.DrawThrow();
    }

    private void FixedUpdate()
    {
        move.TurnPlayer();
        move.MovePlayer(0.3f);
        action.ChargeThrow();
    }
}
