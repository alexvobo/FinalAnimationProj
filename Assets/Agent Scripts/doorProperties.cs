using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorProperties : MonoBehaviour
{
    public bool infected = false;
    public float stayOpenTime = 1.5f;

    public void MoveDoor()
    {
        StartCoroutine(moveDoor());
    }
    IEnumerator moveDoor()
    {
        DoorRotationLite dooropening = this.GetComponent<DoorRotationLite>();

        // Open/close the door by running the 'Open' function found in the 'Door' script
        if (dooropening.RotationPending == false)
        {
            StartCoroutine(dooropening.Move());
            yield return new WaitForSeconds(stayOpenTime); // Close door after 1.5 secs
            StartCoroutine(dooropening.Move());
        }

    }
}
