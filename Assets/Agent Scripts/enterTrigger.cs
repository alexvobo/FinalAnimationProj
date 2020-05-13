using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enterTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    private ManagerScript manager;
    void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<ManagerScript>();
    }

    private void OnTriggerExit(Collider other)
    {
        var agent = other.gameObject;
        if (agent.CompareTag("Infected"))
        {
            print("Detected " + agent.tag);
            if (!manager.getInfected().Contains(agent))
            {
                manager.addInfected(agent);
                return;
            }

        }
        else if (agent.CompareTag("Normal"))
        {
            print("Detected " + agent.tag);
            if (!manager.getNormal().Contains(agent))
            {
                manager.addNormal(agent);
                return;
            }


        }
    }
}
