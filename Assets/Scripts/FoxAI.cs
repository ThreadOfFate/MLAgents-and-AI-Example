using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MLAgents;

public class FoxAI : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public GrassManager grassManager;

    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        StartCoroutine(PathfindingLoop());
    }

    

    /// <summary>
    /// Adjusts the pathfinding so that avoiding chasing NPCs takes more proirty the close they get
    /// It does this by running more directly away from the chaser the close it gets
    /// 
    /// Also will check if continuting along the pathfinding direction is possible and if not will repath to the halfway direction between intended direction if the path was valid and the direction vector perpendicular to the direction vector between this AI and the chasing AI
    /// </summary>
    /// <param name="target">Target location</param>
    /// <returns>Path to take</returns>
    public NavMeshPath DynamicObstacles(Vector3 target, List<Transform> obstacles)
    {
        NavMeshPath navMeshPath = new NavMeshPath();
        if (obstacles != null || obstacles.Count < 1)
        {
            navMeshAgent.CalculatePath(target, navMeshPath);
            return navMeshPath;
        }
        Vector3 obstacleDirection = transform.position - obstacles[0].position;
        for (int i = 1; i < obstacles.Count; i++)
        {
            if ((transform.position - obstacles[i].position).magnitude < obstacleDirection.magnitude)
            {
                obstacleDirection = transform.position - obstacles[i].position;
            }
        }


        
        Vector3 AIDirection = target - transform.position;

        float ignoreDistance = 10f;
        float fullEvasionDistance = 2.0f;

        Vector3 newAIDirection = transform.position + Vector3.Slerp(obstacleDirection.normalized * AIDirection.magnitude, AIDirection, (Mathf.Clamp(obstacleDirection.magnitude, fullEvasionDistance, ignoreDistance) - fullEvasionDistance) / (ignoreDistance - fullEvasionDistance));


        if (!navMeshAgent.CalculatePath(transform.position + (newAIDirection - transform.position).normalized * navMeshAgent.speed, navMeshPath))
        {
            if (!navMeshAgent.CalculatePath(transform.position + Vector3.Lerp((newAIDirection - transform.position).normalized, Vector3.Cross((newAIDirection - transform.position).normalized, transform.up).normalized, 0.5f) * navMeshAgent.speed, navMeshPath))
            {
                navMeshAgent.CalculatePath(newAIDirection, navMeshPath);
            }
        }
        return navMeshPath;
    }

    /// <summary>
    /// Sets up the Pathfinding Loop in a Coroutine
    /// 
    /// Will immediately start looking for a new path once the last path has been decided
    /// </summary>
    IEnumerator PathfindingLoop()
    {
        while (true)
        {
            navMeshAgent.path = PredictRunner();
            yield return new WaitForEndOfFrame();
        }
    }


    /// <summary>
    /// Will attempt to predict where the Target will be in 1 second and pathfind to there
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public NavMeshPath PredictRunner()
    {
        //Assumes the agent is facing the direction it is moving
        Vector3 agentDirection = //agent.GetComponent<Rigidbody>().velocity;
        target.transform.forward;

        Vector3 agentPosition = target.transform.position;
        float AIspeed = navMeshAgent.speed;

        //(transform.position - agentPosition).magintude/speed = Time
        float eta = (transform.position - agentPosition).magnitude / AIspeed;
        //(agentPosition + agentDirection * eta) = (transform.position + predictedDirection * eta)
        //predictedDirection = (agentPosition-transfrom.position + agentDirection * eta)/eta
        Vector3 predictedDirection = (agentPosition - transform.position) / eta + agentDirection;

        //
        NavMeshPath navMeshPath = new NavMeshPath();
        
        //Checks if the current path will be valid if traveled for another second
        if (!navMeshAgent.CalculatePath(transform.position + predictedDirection, navMeshPath))
        {
            

            //If the regular path is invalid will turn the target directional vector Pi/2 radians in the rotational directoon furtherest from the chasers directional vector with this agent
            if (!navMeshAgent.CalculatePath(agentPosition, navMeshPath))
            {

                

            }
        }
        return navMeshPath;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Sheep")
        {
            if(collision.transform.GetComponent<Agent>() != null)
            {
                collision.transform.GetComponent<Agent>().AddReward(-5f);
            }
            else
            {
                GetComponentInParent<TrainingManager>().ResetArea();
            }
            
        }
    }
}
