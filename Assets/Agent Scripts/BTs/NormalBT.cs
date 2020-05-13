using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;
using RootMotion.FinalIK;
using System.Linq;
using System.Xml;

public class NormalBT : MonoBehaviour
{
    public GameObject normalPrefab, infectedPrefab;

    private List<GameObject> patients;
    public int numPatients;
    public long spawnRate;
    public int maxPatients;
    private BehaviorAgent behaviorAgent;
    public int index;

    private ManagerScript manager;

    // Use this for initialization
    void Start()
    {
        index = 0;
        numPatients = maxPatients = 3;
        spawnRate = 5;
        patients = new List<GameObject>();
        manager = GameObject.Find("Manager").GetComponent<ManagerScript>();
        SpawnAgents();
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
    }

    #region helper functions
    public List<GameObject> GetNormal()
    {
        return patients.ToList();
    }
    #endregion
    #region Spawn Agents
    public void SpawnAgents()
    {
        for (var i = 0; i < numPatients; i++)
        {
            print("spawning");
            GameObject parent = GameObject.Find("Agents");
            GameObject spawn = GameObject.Find("spawnNormal");


            GameObject agent;

            var v = Random.value;

            //50% chance of spawning infected, 50% chance of spawning non infected
            if (v >= .5f)
            {
                agent = Instantiate(normalPrefab, spawn.transform.position, Quaternion.identity);

            }
            else
            {
                agent = Instantiate(infectedPrefab, spawn.transform.position, Quaternion.identity);

            }
            agent.SetActive(false);
            agent.transform.parent = parent.transform;
            patients.Add(agent);
        }
    }

    /*    public GameObject popAgent()
        {
            var agent = patients[index];
            index++;
            agent.SetActive(true);
            return agent;
        }*/
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


    /*  public Node Node_AgentIsBlocking()
      {
          // Check if an agent is blocking the spawn
          return new LeafInvoke(() => this.isSpawned());

      }
      public virtual RunStatus isBlocking()
      {
          foreach (var p in patients)
          {
              if (p.transform == lastSpawnLoc)
              {
                  return RunStatus.Running;
              }
          }
          return RunStatus.Failure;
      }
  */
    #region movement
    protected Node ST_ApproachAndWait(GameObject patient, Transform target)
    {
        Val<Vector3> position = Val.V(() => target.position);
        Sequence seq = new Sequence();
        seq.Children.Add(patient.GetComponent<BehaviorMecanim>().Node_GoTo(position));
        return seq;
    }
    protected Node SendAll(Transform target)
    {
        Sequence seq = new Sequence();
        foreach (var p in patients)
        {
            p.SetActive(true);
            seq.Children.Add(ST_ApproachAndWait(p, target));
            if (p.GetComponent<agentProperties>().infected)
            {
                seq.Children.Add(GoToInfectedSection(p));
            }
            else
            {
                seq.Children.Add(GoToNormalSection(p));
            }

            seq.Children.Add(new LeafWait(Val<long>.V(() => spawnRate * 1000)));

        }
        return new SequenceShuffle(seq);
    }
    protected Node GoToInfectedSection(GameObject patient)
    {
        var seq = new Sequence(
            ST_ApproachAndWait(patient, GameObject.Find("InfectedArea/Beds/bed").transform), //watch for naming in sidebar
            new LeafInvoke(() => patient.GetComponent<BodyMecanim>().SitDown()), //check these scripts for more animations, need to encapsulate in a node
            new LeafWait(2000), //1000 = 1 second
             new LeafInvoke(() => patient.GetComponent<BodyMecanim>().StandUp()),
            exitHospital(patient)
            );
        return seq;
    }
    protected Node GoToNormalSection(GameObject patient)
    {
        var seq = new Sequence(
            ST_ApproachAndWait(patient, GameObject.Find("Big Room/bed").transform), //watch for naming in sidebar
            new LeafInvoke(() => patient.GetComponent<BodyMecanim>().SitDown()), //check these scripts for more animations, need to encapsulate in a node
            new LeafWait(2000), //1000 = 1 second
             new LeafInvoke(() => patient.GetComponent<BodyMecanim>().StandUp()),
            exitHospital(patient)
            );
        return seq;
    }
    protected Node exitHospital(GameObject patient)
    {
        var seq = new Sequence(
            ST_ApproachAndWait(patient, GameObject.Find("spawnNormal").transform),
            new LeafWait(2000)
        );

        return seq;
    }

    #endregion
    protected Node BuildTreeRoot()
    {

        Node roaming = new DecoratorLoop(
                      SendAll(GameObject.Find("LobbyWP").transform));
        /* this.ST_ApproachAndWait(patients[0], GameObject.Find("Lobby").transform),*/

        /* new DecoratorLoop(
             new Sequence(this.PickUp(participant), this.PutDown(participant)))
         );*/
        return roaming;
    }

    /*    private Node goToInfectedSection()
        {
            var seq = new Sequence(
            this.ST_ApproachAndWait(patients[0], GameObject.Find("Big Room/bed (0)").transform),
            //new LeafWait(2),
            exitHospital()
            );

            return seq;
        }*/


}
