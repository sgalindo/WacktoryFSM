using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class couchHold : couchState
{

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (interact.heldItem == null)
            changeState(couchPlayer.state.Base);
        if (Input.GetButtonDown(Owner.interactButtonName))
        {
            interact.Release();
            changeState(couchPlayer.state.Base);
        }
    }

    private void FixedUpdate()
    {
        move.MovePlayer();
        move.TurnPlayer();
    }
}
