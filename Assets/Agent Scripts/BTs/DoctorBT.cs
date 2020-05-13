using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;
using RootMotion.FinalIK;


public class DoctorBT : MonoBehaviour
{
    public GameObject DoctorPrefab;
    public int numDoctors;

    public List<GameObject> doctors;

    private BehaviorAgent behaviorAgent;

    private ManagerScript manager;


    // Use this for initialization
    void Start()
    {
        doctors = new List<GameObject>();
        numDoctors = 2;
        spawnAgent();
        manager = GameObject.Find("Manager").GetComponent<ManagerScript>();
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
    }
    public List<GameObject> GetDoctors()
    {
        return doctors;
    }
    #region Spawn Agents
    public void spawnAgent()
    {
        var parent = GameObject.Find("Agents");

        float MinX = 10f;
        float MaxX = 25f;

        float MinZ = 14.5f;
        float MaxZ = 27;

        for (var i = 0; i < numDoctors; i++)
        {
            float x = Random.Range(MinX, MaxX);
            float z = Random.Range(MinZ, MaxZ);

            print("doc" + new Vector3(x, .5f, z));
            var agent = Instantiate(DoctorPrefab, new Vector3(x, .5f, z), Quaternion.identity);

            agent.transform.parent = parent.transform;

            doctors.Add(agent);
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
        return new Sequence(doctors[0].GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(1000));
    }
    protected virtual RunStatus InfiniteLoop()
    {
        // for each doctor in manager.assigned doctor -> send them to patient. it's a dictionary.
        return RunStatus.Success;
    }
    protected Node BuildTreeRoot()
    {
        //success condition for decorator ^^^^ = restart from beginning
        Node roaming = new DecoratorLoop(
                         new LeafInvoke(() => InfiniteLoop())
                         );
        //goToInfectedBed(),

        /* new DecoratorLoop(
             new Sequence(this.PickUp(participant), this.PutDown(participant)))
         );*/
        return roaming;
    }

    // Walks to infected patients bed one by one
    private Node goToInfectedBed()
    {

        var seq = new Sequence();

        for (int i = 0; i < 14; i++)
        {
            seq.Children.Add(this.ST_ApproachAndWait(doctors[0], GameObject.Find("Beds/bed " + "(" + i + ")").transform));
        }

        return seq;
    }
}
