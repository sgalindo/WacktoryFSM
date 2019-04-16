using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class couchActionBox : MonoBehaviour
{
    public GameObject currentObject;
    bool thisFrame;
    private IEnumerator AfterTrigger;
    private void Awake()
    {
        AfterTrigger = _WaitForFixedUpdate();
        StartCoroutine(AfterTrigger);
    }

    private void FixedUpdate()
    {
        thisFrame = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "grabbable" || other.gameObject.tag == "Interactable")
        {
            if (currentObject != null)
            {
                if (currentObject.tag != "grabbable")
                    currentObject = other.gameObject;
            }
            else
                currentObject = other.gameObject;
            thisFrame = true;
        }
    }

    private IEnumerator _WaitForFixedUpdate()
    { 
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (thisFrame == false)
                currentObject = null;
        }
    }
}
