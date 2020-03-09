using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

/// <summary>
/// Machine Learning Agent for a fox using a camera to identify the goal
/// </summary>
public class FoxAgentCamera : Agent
{
    public float moveSpeed = 5f;
    public float turnSpeed = 180f;
    new private Rigidbody rigidbody;
    public GenerateMaze maze;

    /// <summary>
    /// Initial setup
    /// </summary>
    public override void InitializeAgent()
    {
        base.InitializeAgent();
        rigidbody = GetComponent<Rigidbody>();
        maze = GetComponentInParent<GenerateMaze>();
        maze.SetRange((int)Academy.Instance.FloatProperties.GetPropertyWithDefault("Range", 4f));
    }

    /// <summary>
    /// Perform actions based on a vector of numbers
    /// </summary>
    /// <param name="vectorAction">The list of actions to take</param>
    public override void AgentAction(float[] vectorAction)
    {
        
        float forwardAmount = vectorAction[0];

        float turnAmount = 0f;
        if (vectorAction[1] == 1f)
        {
            turnAmount = -1f;
        }
        else if (vectorAction[1] == 2f)
        {
            turnAmount = 1f;
        }

        
        rigidbody.MovePosition(transform.position + transform.forward * forwardAmount * moveSpeed * Time.fixedDeltaTime);
        transform.Rotate(transform.up * turnAmount * turnSpeed * Time.fixedDeltaTime);

        if (maxStep > 0)
        {
            AddReward(-1f / maxStep);
        }
    }

    /// <summary>
    /// Allows the User to control the Agent
    /// </summary>
    /// <returns></returns>
    public override float[] Heuristic()
    {
        float forwardAction = 0f;
        float turnAction = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            forwardAction = 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            turnAction = 1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            turnAction = 2f;
        }
        return new float[] { forwardAction, turnAction };
    }

    /// <summary>
    /// Observations, these are the variables the ML Agent "sees" and determines what to do on
    /// </summary>
    public override void CollectObservations()
    {
        // Direction wolf is facing (1 Vector3 = 3 values)
        AddVectorObs(transform.forward);
        // 3 total values
    }

    /// <summary>
    /// Asks for a decision every 5 updates
    /// Asks for a action every other frame
    /// </summary>
    private void FixedUpdate()
    {
        if (GetStepCount() % 5 == 0)
        {
            RequestDecision();
        }
        else
        {
            RequestAction();
        }

    }

    /// <summary>
    /// Resets Envirnment and Agent
    /// </summary>
    public override void AgentReset()
    {
        maze.SetRange((int)Academy.Instance.FloatProperties.GetPropertyWithDefault("Range", 4f));
        maze.BuildMaze();
    }

    /// <summary>
    /// Will reward the agent when it reachs the goal
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Goal")
        {
            Destroy(collision.gameObject);
            AddReward(10.0f);
            
            Done();

        }
    }
    

}
