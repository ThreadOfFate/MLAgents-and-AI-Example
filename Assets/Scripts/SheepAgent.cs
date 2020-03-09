using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

/// <summary>
/// Machine Learning Agent for a sheep
/// </summary>
public class SheepAgent : Agent
{
    public float moveSpeed = 5f;
    public float turnSpeed = 180f;

    new private Rigidbody rigidbody;
    public TrainingManager trainingManager;

    

    /// <summary>
    /// Initial setup
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
    /// Resets Envirnment and Agent
    /// </summary>
    public override void AgentReset()
    {
        trainingManager.ResetArea();
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Grass")
        {
            AddReward(1.0f);
            if (collision.transform.GetComponentInParent<GrassManager>().DestroyGrass(collision.gameObject))
            {
                //The extra reward will help destingish any mutation good enough to eat all the grass before the fox catchs it
                AddReward(5.0f);
                Done();
            }
        }
    }
}
