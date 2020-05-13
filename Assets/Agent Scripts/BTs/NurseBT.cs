using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;
using RootMotion.FinalIK;

public class NurseBT : MonoBehaviour
{
    public GameObject NursePrefab;
    public int numNurses;

    public List<GameObject> nurses = new List<GameObject>();


    private BehaviorAgent behaviorAgent;

    private ManagerScript manager;

    // Use this for initialization
    void Start()
    {
        numNurses = 2;
        spawnAgent();
        manager = GameObject.Find("Manager").GetComponent<ManagerScript>();
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
    }
    #region Spawn Agents
    public void spawnAgent()
    {
        var parent = GameObject.Find("Agents");

        float MinX = 10f;
        float MaxX = 25f;

        float MinZ = 14.5f;
        float MaxZ = 27;

        for (var i = 0; i < numNurses; i++)
        {
            float x = Random.Range(MinX, MaxX);
            float z = Random.Range(MinZ, MaxZ);

            //print("nurs" + new Vector3(x, .5f, z));

            var agent = Instantiate(NursePrefab, new Vector3(x, 0f, z), Quaternion.identity);

            agent.transform.parent = parent.transform;

            nurses.Add(agent);
        }


    }
    #endregion
    #region IK related function
    /* protected Node PickUp(GameObject p)
     {
         return new Sequence(this.Node_BallStop(),
                             p.GetComponent<BehaviorMecanim>().Node_StartInteraction(hand, ikBall),
                             new LeafWait(1000),
                             p.GetComponent<BehaviorMecanim>().Node_StopInteraction(hand));
     }

     public Node Node_BallStop()
     {
         return new LeafInvoke(() => this.BallStop());
     }
     public virtual RunStatus BallStop()
     {
         Rigidbody rb = ball.GetComponent<Rigidbody>();
         rb.velocity = Vector3.zero;
         rb.isKinematic = true;

         return RunStatus.Success;
     }

     protected Node PutDown(GameObject p)
     {
         return new Sequence(p.GetComponent<BehaviorMecanim>().Node_StartInteraction(hand, ikBall),
                             new LeafWait(300),
                             this.Node_BallMotion(),
                             new LeafWait(500), p.GetComponent<BehaviorMecanim>().Node_StopInteraction(hand));
     }

     public Node Node_BallMotion()
     {
         return new LeafInvoke(() => this.BallMotion());
     }

     public virtual RunStatus BallMotion()
     {
         Rigidbody rb = ball.GetComponent<Rigidbody>();
         rb.velocity = Vector3.zero;
         rb.isKinematic = false;
         ball.transform.parent = null;
         return RunStatus.Success;
     }*/
    #endregion

    protected Node ST_ApproachAndWait(GameObject patient, Transform target)
    {
        Val<Vector3> position = Val.V(() => target.position);
        return new Sequence(nurses[0].GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(1000));
    }
    protected virtual RunStatus PatientAvailable()
    {
        /*  if (manager.activeNurses.Count > 0)
          {
              return RunStatus.Failure;
          }
          else
          {
              return RunStatus.Success;
          }*/
        return RunStatus.Failure;

    }
    protected Node SendNursesToPatients()
    {
        Sequence seq = new Sequence();
        foreach (var n in manager.activeNurses.Keys)
        {
            //SEND ALL NURSES TO BOUND PATIENTS.
            seq.Children.Add(ST_ApproachAndWait(n, manager.activeNurses[n].transform));
        }
        return seq;
    }

    protected Node BuildTreeRoot()
    {
        ////NURSES AND DOCTORS ONLY MOVE WHEN ASSIGNED. SEE MANAGER SCRIPT (activenurses,activedoctors)
        Node roaming = new Sequence(new DecoratorLoop(
                         new LeafInvoke(() => PatientAvailable())
                         ),SendNursesToPatients());
        //goToInfectedBed(),

        /* new DecoratorLoop(
             new Sequence(this.PickUp(participant), this.PutDown(participant)))
         );*/
        return roaming;
    }


    private Node goToInfectedPatientBed()
    {
        var seq = new Sequence(
        this.ST_ApproachAndWait(nurses[0], GameObject.Find("Beds/bed").transform),
        new LeafWait(2)
        );

        return seq;
    }

    private Node goToNormalPatientBed()
    {
        var seq = new Sequence(
        this.ST_ApproachAndWait(nurses[0], GameObject.Find("Big Room/bed").transform),
        new LeafWait(2)
        );

        return seq;
    }

    private Node goToLounge()
    {
        var seq = new Sequence(
        this.ST_ApproachAndWait(nurses[0], GameObject.Find("Employee Lounge").transform),
        new LeafWait(2)
        );
        //remove nurse from manager

        return seq;
    }

    private Node exitHospital()
    {
        var seq = new Sequence(
        this.ST_ApproachAndWait(nurses[0], GameObject.Find("ExitHospital").transform),
        new LeafWait(2)
        );

        return seq;
    }

    // Check infection
    // If Patient is infected
    //    Nurse takes patient to COVID area
    // If Patient is NOT infected
    //    Nurse takes patient to regular area
    private Node checkInfection()
    {

        var seq = new Sequence();

        // need to synchronize patients
        // if(patients[0].GetComponent<agentProperties>().CompareTag("Infected")) {
        //     return new Sequence(
        //     goToInfectedPatientBed(),
        //     new LeafWait(2)
        //     );
        // } else {
        //   return new Sequence(
        //    goToNormalPatientBed(),
        //    new LeafWait(2)
        //   );
        // }

        return seq;

    }
}
