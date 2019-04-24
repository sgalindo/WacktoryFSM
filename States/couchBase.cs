using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class couchBase : couchState
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
        if (Input.GetButtonDown(Owner.jumpButtonName) && foot.isGrounded) { move.Jump(); }

        if (Input.GetAxisRaw(Owner.interactButtonName) > 0 && actionBox.currentObject != null)
        {
            if (action.PickUp(actionBox.currentObject)) { changeState(couchPlayer.state.Carry); return; }
            if (action.Interact(actionBox.currentObject)) { Debug.Log("Gottem?"); }
        }
    }

    private void FixedUpdate()
    {
        move.MovePlayer();
        move.TurnPlayer();
    }
}
