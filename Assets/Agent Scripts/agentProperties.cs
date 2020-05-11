using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class agentProperties : MonoBehaviour
{
    public bool mask; // has mask
    public float maskChance; //chance to get mask, .3 = 30%

    public bool infected; // is infected
    public float infectionChance; // chance of infecting something, .1 = 10%

    public float reach; // distance for agent to open door 

    // Start is called before the first frame update
    void Start()
    {
        mask = false;
        maskChance = .3f;
        infected = false;
        infectionChance = .1f;

        reach = 5f;

        GetMask();
        CheckInfected();
    }

    #region mask stuff
    private bool DeservesMask()
    {
        if (UnityEngine.Random.value > (1-maskChance))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void GetMask()
    {
        var tag = this.tag;
        if (CompareTag("Normal") || CompareTag("Infected"))
        {
            mask = DeservesMask();
        }
        else
        {
            mask = true;
        }

    }
    #endregion
    #region infected stuff
    public void CheckInfected()
    {
        if (CompareTag("Infected"))
        {
            infected = true;
        }
        else
        {
            infected = false;
        }
    }
    public void infectDoor(GameObject door)
    {
        var rand = UnityEngine.Random.Range(0f, 1f);
        if (rand > (1-infectionChance))
        {
            print((1 - infectionChance) + " " + rand);
            door.GetComponent<doorProperties>().infected = true;
            door.GetComponent<Renderer>().material.color = Color.red;
        }
    }
    #endregion


    #region Door Movement

    public void DetectDoor()
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, fwd, out hit, reach) && (hit.transform.CompareTag("Door")))
        {
            GameObject door = hit.transform.gameObject;

            if (infected && !door.GetComponent<doorProperties>().infected)
            {
                infectDoor(door);
            }

            door.GetComponent<doorProperties>().MoveDoor();

        }
    }
    #endregion
    // Update is called once per frame
    void FixedUpdate()
    {
        DetectDoor();
    }
}
