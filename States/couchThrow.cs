using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class couchThrow : couchState
{
    /*
        Zackvador is a fucking God.
     */

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        move.TurnPlayer();
    }
}
