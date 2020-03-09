using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class FoxAgent : Agent
{
    [Tooltip("How fast the agent moves forward")]
    public float moveSpeed = 5f;

    [Tooltip("How fast the agent turns")]
    public float turnSpeed = 180f;

    new private Rigidbody rigidbody;

    public TrainingManager trainingManager;

    

    /// <summary>
    /// Initial setup, called when the agent is enabled
    /// </summary>
    public override void InitializeAgent()
    {
        base.InitializeAgent();
        rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Perform actions based on a vector of numbers
    /// </summary>
    /// <param name="vectorAction">The list of actions to take</param>
    public override void AgentAction(float[] vectorAction)
    {
        // Convert the first action to forward movement
        float forwardAmount = vectorAction[0];

        // Convert the second action to turning left or right
        float turnAmount = 0f;
        if (vectorAction[1] == 1f)
        {
            turnAmount = -1f;
        }
        else if (vectorAction[1] == 2f)
        {
            turnAmount = 1f;
        }

        // Apply movement
        rigidbody.MovePosition(transform.position + transform.forward * forwardAmount * moveSpeed * Time.fixedDeltaTime);
        transform.Rotate(transform.up * turnAmount * turnSpeed * Time.fixedDeltaTime);

        // Apply a tiny negative reward every step to encourage action
        if (maxStep > 0) AddReward(-1f / maxStep);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override float[] Heuristic()
    {
        float forwardAction = 0f;
        float turnAction = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            // move forward
            forwardAction = 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            // turn left
            turnAction = 1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // turn right
            turnAction = 2f;
        }
        // Put the actions into an array and return
        return new float[] { forwardAction, turnAction };
    }

    public override void CollectObservations()
    {

        // Direction wolf is facing (1 Vector3 = 3 values)
        AddVectorObs(transform.forward);

        // 3 total values
    }

    private void FixedUpdate()
    {
        // Request a decision every 5 steps. RequestDecision() automatically calls RequestAction(),
        // but for the steps in between, we need to call it explicitly to take action using the results
        // of the previous decision
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
    /// 
    /// </summary>
    public override void AgentReset()
    {
        trainingManager.ResetArea();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Sheep")
        {
            AddReward(5.0f);
            
            Done();
            
        }
    }

}
