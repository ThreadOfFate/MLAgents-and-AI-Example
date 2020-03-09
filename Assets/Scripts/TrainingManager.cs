using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using UnityEngine.AI;

/// <summary>
/// Class to reset the envirnment for MLTraining
/// To work with NavMesh and ML agents
/// </summary>
public class TrainingManager : MonoBehaviour
{
    public List<Agent> mlagents;
    public List<NavMeshAgent> navMeshAgents;
    public GrassManager grassManager;
    
    /// <summary>
    /// Resets the training area
    /// </summary>
    public void ResetArea()
    {
        grassManager.ResetGrass();
        ResetAgents();
    }

    /// <summary>
    /// Resets the ML and/or NavMesh Agents
    /// </summary>
    public void ResetAgents()
    {
        //Debug.Log("s");

        for (int i = 0; i < mlagents.Count; i++)
        {
            mlagents[i].transform.position = transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        }

        for (int i = 0; i < navMeshAgents.Count; i++)
        {
            navMeshAgents[i].transform.position = transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        }

    }

    
}
