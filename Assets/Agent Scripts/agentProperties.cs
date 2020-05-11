using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class agentProperties : MonoBehaviour
{
    //https://www.reuters.com/article/uk-factcheck-coronavirus-mask-efficacy/partly-false-claim-wear-a-face-mask-covid-19-risk-reduced-by-up-to-98-5-idUSKCN2252T6
    public bool mask; // has mask
    public float maskChance; //chance to get mask, .3 = 30%

    public bool infected; // is infected
    public float infectionChance; // chance of infecting something, .1 = 10%

    public float reach; // distance for agent to open door 

    public float infectionRadius; //radius of infectivity

    public Dictionary<GameObject, bool> perceivedNeighbors = new Dictionary<GameObject, bool>();

    public GameObject orb;
    // Start is called before the first frame update
    void Start()
    {
        mask = false;
        maskChance = .3f;
        infected = false;
        CheckInfected();
        infectionChance = .1f;
        infectionRadius = 6f;
        reach = 10f;

        orb.SetActive(false);

        GetComponent<CapsuleCollider>().radius = infectionRadius / 2;
        GetMask();
    }

    #region mask stuff
    private bool DeservesMask()
    {
        if (UnityEngine.Random.value > (1 - maskChance))
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
        if (rand >= (1 - infectionChance))
        {
            print((1 - infectionChance) + " " + rand);
            door.GetComponent<doorProperties>().infected = true;
            door.GetComponent<Renderer>().material.color = Color.red;
        }
    }
    #endregion
    public bool infect(float chance)
    {
        if (UnityEngine.Random.Range(0f, 1f) >= (1 - chance))
        {
            return true;
        }

        return false;
    }
    public void infectOthers()
    {
        List<GameObject> taggedNeighbors = new List<GameObject>();
        // 70% of getting covid if other person not wearing mask
        // 5% if covid person has it but the other person doesnt have one
        // 1.5% chance if both have masks
        if (infected)
        {

            foreach (var n in perceivedNeighbors.Keys)
            {
                var neighborProps = n.GetComponent<agentProperties>();
                // if infection has not been evaluated already
                if (perceivedNeighbors[n] == false)
                {
                    //if we have a mask on
                    if (mask)
                    {
                        //if neighbor has mask
                        if (neighborProps.mask)
                        {

                            if (infect(.015f))
                                neighborProps.infected = true;
                            else
                            {
                                //if neighbor doesnt have mask
                                if (infect(.05f))
                                {
                                    neighborProps.infected = true;
                                }

                            }
                        }
                    }
                    else
                    {
                        // if we dont have a mask on 

                        if (neighborProps.mask)
                        {
                            //if neighbor has mask
                            if (infect(.1f))
                            {
                                neighborProps.infected = true;
                            }
                            else
                            {
                                //if neighbor doesnt have mask
                                if (infect(.7f))
                                {
                                    neighborProps.infected = true;
                                }
                            }
                        }

                    }
                    taggedNeighbors.Add(n); //cant modify dict at runtime so add tagged neighbor to sep. list
                }
            }
            //deal with neighbors tagged during that loop
            foreach (var neighbor in taggedNeighbors)
            {
                if (perceivedNeighbors.ContainsKey(neighbor))
                    perceivedNeighbors[neighbor] = true;
            }

        }
    }

    #region Door Movement

    public void DetectDoor()
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, fwd, out hit, reach) && (hit.transform.CompareTag("Door")))
        {
            GameObject door = hit.transform.gameObject;
            door.GetComponent<doorProperties>().MoveDoor();

            if (infected && !door.GetComponent<doorProperties>().infected)
            {
                infectDoor(door);
            }

    

        }
    }
    #endregion
    // Update is called once per frame
    void FixedUpdate()
    {
        DetectDoor();
        infectOthers();

        if (infected)
        {
            orb.SetActive(true);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Infected") || other.gameObject.CompareTag("Normal") || other.gameObject.CompareTag("Doctor") || other.gameObject.CompareTag("Nurse"))
        {
            perceivedNeighbors[other.gameObject] = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Infected") || other.gameObject.CompareTag("Normal") || other.gameObject.CompareTag("Doctor") || other.gameObject.CompareTag("Nurse"))
        {
            perceivedNeighbors.Remove(other.gameObject);
        }
    }
}
