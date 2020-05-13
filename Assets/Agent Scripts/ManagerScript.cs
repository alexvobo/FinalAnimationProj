using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerScript : MonoBehaviour
{
    public Dictionary<GameObject, GameObject> activeNurses = new Dictionary<GameObject, GameObject>(); // # Nurse -> assigned to what patient
    public Dictionary<GameObject, GameObject> activeDoctors = new Dictionary<GameObject, GameObject>(); // # doctor -> assigned to what patient

    private Queue<GameObject> docs, nurses;
    private Queue<GameObject> infected, normal;
    private bool nursesDocsSet;
    // Start is called before the first frame update
    void Start()
    {
        docs = new Queue<GameObject>();
        nurses = new Queue<GameObject>();
        nursesDocsSet = false;
        setNursesDocs();
        infected = new Queue<GameObject>();
        normal = new Queue<GameObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!nursesDocsSet)
            setNursesDocs();

        assignStaff();
    }
    #region getters
    public Queue<GameObject> getDocs()
    {
        return docs;
    }
    public Queue<GameObject> getNurses()
    {
        return nurses;
    }

    public Queue<GameObject> getInfected()
    {
        return infected;
    }
    public Queue<GameObject> getNormal()
    {
        return normal;

    }
    #endregion
    #region setters
    public void setNursesDocs()
    {
        var doctorbt = GameObject.Find("Updater").GetComponent<DoctorBT>().doctors;
        var nursebt = GameObject.Find("Updater").GetComponent<NurseBT>().nurses;
        var counter = 0;
        print(GameObject.Find("Updater").GetComponent<DoctorBT>().doctors);
        if (doctorbt.Count > 0)
        {
            foreach (var d in doctorbt)
            {
                docs.Enqueue(d);
            }
            counter++;
        }

        if (nursebt.Count > 0)
        {
            foreach (var n in nursebt)
            {
                nurses.Enqueue(n);
            }
            counter++;
        }

        if (counter == 2)
        {
            nursesDocsSet = true;
        }
    }
    public void addInfected(GameObject p)
    {
        infected.Enqueue(p);
    }
    public void addNormal(GameObject p)
    {
        normal.Enqueue(p);
    }
    #endregion
    #region Dictionary functions
    public void assignDoctor(GameObject doctor, GameObject patient)
    {
        if (docs.Contains(doctor) && (infected.Contains(patient) || normal.Contains(patient)))
        {
            activeDoctors[doctor] = patient;
        }
    }
    public void assignNurse(GameObject nurse, GameObject patient)
    {
        if (docs.Contains(nurse) && (infected.Contains(patient) || normal.Contains(patient)))
        {
            activeNurses[nurse] = patient;
        }
    }
    public void removeNurse(GameObject nurse)
    {
        if (activeNurses.ContainsKey(nurse))
        {
            activeNurses.Remove(nurse);
            nurses.Enqueue(nurse);
        }
    }
    public void removeDoctor(GameObject doctor)
    {
        if (activeDoctors.ContainsKey(doctor))
        {
            activeDoctors.Remove(doctor);
            docs.Enqueue(doctor);
        }
    }

    #endregion

    #region main functions we want to call
    public void assignNurseToPatient()
    {
        if (nurses.Count > 0)
        {
            if (infected.Count > 0)
            {
                var patient = infected.Dequeue();
                var nurse = nurses.Dequeue();
                patient.GetComponent<agentProperties>().assignedNurse = nurse;
                assignNurse(nurse, patient);
                Debug.Log("Assigned " + nurse + " to " + patient);
                return;
            }
            else if (normal.Count > 0)
            {
                var patient = normal.Dequeue();
                var nurse = nurses.Dequeue();
                patient.GetComponent<agentProperties>().assignedNurse = nurse;
                assignNurse(nurse, patient);
                Debug.Log("Assigned " + nurse + " to " + patient);
                return;
            }
        }
    }
    public void assigndoctorToPatient()
    {
        if (docs.Count > 0)
        {
            if (infected.Count > 0)
            {
                var patient = infected.Dequeue();
                var doc = docs.Dequeue();
                patient.GetComponent<agentProperties>().assignedDoctor = doc;
                assignDoctor(doc, patient);
                Debug.Log("Assigned " + doc + " to " + patient);
                return;
            }
            else if (normal.Count > 0)
            {
                var patient = normal.Dequeue();
                var doc = docs.Dequeue();
                patient.GetComponent<agentProperties>().assignedDoctor = doc;
                assignDoctor(doc, patient);
                Debug.Log("Assigned " + doc + " to " + patient);

                return;
            }
        }
    }

    IEnumerator assignNurseDoc()
    {
        assignNurseToPatient();
        yield return new WaitForSeconds(60);
        assigndoctorToPatient();

    }
    public void assignStaff()
    {
        StartCoroutine(assignNurseDoc());
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Infected"))
        {
            print("Detected " + other.gameObject);
            addInfected(other.gameObject);


        }
        else if (other.gameObject.CompareTag("Normal"))
        {
            print("Detected " + other.gameObject);
            addNormal(other.gameObject);

        }
    }
    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("Infected"))
        {
            try
            {
                print("removing " + other.gameObject);

                activeDoctors.Remove(other.gameObject.GetComponent<agentProperties>().assignedDoctor);
                activeNurses.Remove(other.gameObject.GetComponent<agentProperties>().assignedNurse);
            }
            catch { }


        }
        else if (other.gameObject.CompareTag("Normal"))
        {
            try
            {
                print("removing " + other.gameObject);

                activeDoctors.Remove(other.gameObject.GetComponent<agentProperties>().assignedDoctor);
                activeNurses.Remove(other.gameObject.GetComponent<agentProperties>().assignedNurse);
            }
            catch { }
        }
    }

    #endregion
}
