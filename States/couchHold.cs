using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class couchHold : couchState
{
    private int counter;
    private bool stillDown;
    private void OnEnable() { counter = 0; stillDown = true; }

    private void OnDisable()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (action.heldItem == null) { changeState(couchPlayer.state.Base);  return; }

        if (stillDown)
        {
            if (Input.GetAxisRaw(Owner.interactButtonName) == 0) { stillDown = false; }
            return;
        }

        if (Input.GetAxisRaw(Owner.interactButtonName) != 0) counter++;
        else counter = 0;

        if (Input.GetButtonUp(Owner.interactButtonName))
        {
            action.Release();
            Debug.Log("Test");
            changeState(couchPlayer.state.Base);
            return;
        }

        if(counter > 20) { changeState(couchPlayer.state.Throw); return; }
    }

    private void FixedUpdate()
    {
        move.MovePlayer();
        move.TurnPlayer();
    }
}
