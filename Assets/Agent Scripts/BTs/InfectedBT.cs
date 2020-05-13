using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;
using RootMotion.FinalIK;

public class InfectedBT : MonoBehaviour
{
    public GameObject infectedPrefab;

    private List<GameObject> patients = new List<GameObject>();
    public int numPatients = 1;
    public int spawnRate = 1;

    private BehaviorAgent behaviorAgent;

    // Use this for initialization
    void Start()
    {
        spawnAgent();

        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
    }
    #region Spawn Agents
    public void spawnAgent()
    {
        var parent = GameObject.Find("Agents");

        var spawn = GameObject.Find("spawnInfected");

        var agent = Instantiate(infectedPrefab, spawn.transform.position, Quaternion.identity);

        agent.transform.parent = parent.transform;

        patients.Add(agent);

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
        return new Sequence(patients[0].GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(1000));
    }
    protected Node BuildTreeRoot()
    {
        Node roaming = new Sequence(
                        this.ST_ApproachAndWait(patients[0], GameObject.Find("Lobby").transform),
                        goToInfectedSection()
                        );
        /* new DecoratorLoop(
             new Sequence(this.PickUp(participant), this.PutDown(participant)))
         );*/
        return roaming;
    }

    private Node goToInfectedSection() {
      var seq = new Sequence(
      this.ST_ApproachAndWait(patients[0], GameObject.Find("Beds/bed (0)").transform),
      //new LeafWait(2),
      exitHospital()
      );

      return seq;
    }

    private Node exitHospital() {
      var seq = new Sequence(
      this.ST_ApproachAndWait(patients[0], GameObject.Find("ExitHospital").transform),
      new LeafWait(2)
      );

      return seq;
    }
}
