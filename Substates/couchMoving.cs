using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class couchMoving : couchSubState
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!foot.isGrounded) { changeSub(couchPlayer.subState.Aerial); return; }

        if (move.movementInput.magnitude == 0) { changeSub(couchPlayer.subState.Idle); return; }
    }
}
