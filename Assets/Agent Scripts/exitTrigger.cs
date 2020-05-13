using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exitTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    private ManagerScript manager;
    void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<ManagerScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var agent = other.gameObject;

        if (agent.CompareTag("Infected"))
        {
            try
            {
                print("removing " + agent);

                manager.activeDoctors.Remove(agent.GetComponent<agentProperties>().assignedDoctor);
                manager.activeNurses.Remove(agent.GetComponent<agentProperties>().assignedNurse);
            }
            catch { }


        }
        else if (other.gameObject.CompareTag("Normal"))
        {
            try
            {
                print("removing " + agent);

                manager.activeDoctors.Remove(agent.GetComponent<agentProperties>().assignedDoctor);
                manager.activeNurses.Remove(agent.GetComponent<agentProperties>().assignedNurse);
            }
            catch { }
        }
    }
}
