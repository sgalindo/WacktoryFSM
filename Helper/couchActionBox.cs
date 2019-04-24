using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class couchActionBox : MonoBehaviour
{
    private GameObject slatedObject;
    public GameObject currentObject;
    bool thisFrame;

    private void Awake() { StartCoroutine(_WaitForFixedUpdate()); }

    private void FixedUpdate()
    {
        thisFrame = false;
        slatedObject = null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (currentObject == other.gameObject) { thisFrame = true; return; }

        if (other.gameObject.tag == "grabbable" || other.gameObject.tag == "Interactable")
        {
            if (slatedObject != null)
            {
                if (slatedObject.tag != "grabbable")
                    slatedObject = other.gameObject;
            }
            else
                slatedObject = other.gameObject;
        }
    }

    private IEnumerator _WaitForFixedUpdate()
    { 
        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (thisFrame == false) { currentObject = slatedObject; continue; }

            if (slatedObject && currentObject)
                if (currentObject.tag == "Interactable" && slatedObject.tag == "grabbable") currentObject = slatedObject;

        }
    }
}
