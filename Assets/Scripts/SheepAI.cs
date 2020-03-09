using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SheepAI : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public GrassManager grassManager;
    public List<Transform> obstacles;
    float eatRange = 1f;

    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        StartCoroutine(PathfindingLoop());
    }

    public void LookForFood()
    {
        if(target == null || target.tag == "Grass")
        {
            target = grassManager.GetNearestGrass(transform.position);
        }
        if(target != null  && (target.position-transform.position).magnitude< eatRange)
        {
            grassManager.DestroyGrass(target.gameObject);
            target = null;
        }
    }

    /// <summary>
    /// Adjusts the pathfinding so that avoiding chasing NPCs takes more proirty the close they get
    /// It does this by running more directly away from the chaser the close it gets
    /// 
    /// Also will check if continuting along the pathfinding direction is possible and if not will repath to the halfway direction between intended direction if the path was valid and the direction vector perpendicular to the direction vector between this AI and the chasing AI
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public NavMeshPath DynamicObstacles(Vector3 target, List<Transform> obstacles)
    {
        NavMeshPath navMeshPath = new NavMeshPath();
        if (obstacles != null || obstacles.Count <1)
        {
            navMeshAgent.CalculatePath(target, navMeshPath);
            return navMeshPath;
        }
        Vector3 obstacleDirection = transform.position - obstacles[0].position;
        for(int i = 1;i < obstacles.Count; i++)
        {
            if((transform.position - obstacles[i].position).magnitude < obstacleDirection.magnitude)
            {
                obstacleDirection = transform.position - obstacles[i].position;
            }
        }


        //Vector3 chasersDirection = transform.position - obstacles[0];
        Vector3 AIDirection = target - transform.position;

        float ignoreDistance = 10f;
        float fullEvasionDistance = 3.5f;

        Vector3 newAIDirection = transform.position + Vector3.Slerp(obstacleDirection.normalized * AIDirection.magnitude, AIDirection, (Mathf.Clamp(obstacleDirection.magnitude, fullEvasionDistance, ignoreDistance) - fullEvasionDistance) / (ignoreDistance - fullEvasionDistance));



        if (!navMeshAgent.CalculatePath(transform.position + (newAIDirection - transform.position).normalized * navMeshAgent.speed, navMeshPath))
        {
            
            //Debug.Log("Invalid Path 1");
            //If the regular path is invalid will turn the target directional vector Pi/2 radians in the rotational directoon furtherest from the chasers directional vector with this agent
            if (!navMeshAgent.CalculatePath(transform.position + Vector3.Lerp((newAIDirection - transform.position).normalized, Vector3.Cross((newAIDirection - transform.position).normalized, transform.up).normalized, 0.5f) * navMeshAgent.speed, navMeshPath))
            {
                //Debug.Log("Invalid Path 2");
                
                //Will attempt to move perpendicualr to the directional vector
                if (!navMeshAgent.CalculatePath(transform.position + Vector3.Cross((newAIDirection - transform.position).normalized, transform.up).normalized * navMeshAgent.speed, navMeshPath))
                {

                    //Trys the reverse direction
                    if (!navMeshAgent.CalculatePath(transform.position - Vector3.Cross((newAIDirection - transform.position).normalized, transform.up).normalized * navMeshAgent.speed, navMeshPath))
                    {
                        navMeshAgent.CalculatePath(target, navMeshPath);
                    }
                }
            }
        }
        return navMeshPath;
    }

    

    /// <summary>
    /// Sets up the Pathfinding Loop in a Coroutine 
    /// </summary>
    IEnumerator PathfindingLoop()
    {
        while(true)
        {
            LookForFood();
            if(target!=null)
            {
                navMeshAgent.path = DynamicObstacles(target.position, obstacles);
            }
            
            yield return new WaitForEndOfFrame();
        }
    }
}
