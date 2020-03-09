using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

/// <summary>
/// Behaviour
/// Contains the different behaviors this nueral network has
/// Its important its an int enum so it can be cast into the Agent's Observations
/// </summary>
public enum Behaviour
{
    sheep = 0,
    fox = 1,
}

/// <summary>
/// MLAgents does not support training multiple neural networks at the same time, so if you want agents with seperate behaviours to interact while training you have to train them with the same neural network
/// 
/// This is done by making one of the observations the behaviour setting, the neural network should learn that it is rewarded differently based on the behaviour observation
/// This works best with reinforcement learning and unfortunatly often make curriculum less effective
/// </summary>
public class GenericAgent : Agent
{
    public float moveSpeed = 5f;
    public float turnSpeed = 180f;
    new private Rigidbody rigidbody;
    public Behaviour behaviour;
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
        //Casts the behaviour as an int (1 valuse)
        AddVectorObs((int)behaviour);

        // Direction agent is facing (1 Vector3 = 3 values)
        AddVectorObs(transform.forward);

        // 4 total values
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
    /// Rewards are give on collision with the agents target, but its target depends on the currently set behaviour
    /// 
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        //If the behaviour is set to fox the fox will be rewarded for catching a sheep and the sheep will be punished
        if(behaviour == Behaviour.fox)
        {
            if (collision.transform.tag == "Sheep")
            {
                AddReward(2.0f);
                collision.transform.GetComponent<Agent>().AddReward(-2.0f);
                Done();
            }
        }
        //If the behaviour is set to sheep the sheep will be rewarded for eating grass, and will get an extra reward for collecting all the grass
        if (behaviour == Behaviour.sheep)
        {
            if (collision.transform.tag == "Grass")
            {
                AddReward(1.0f);
                if(collision.transform.GetComponentInParent<GrassManager>().DestroyGrass(collision.gameObject))
                {
                    //The extra reward will help destingish any mutation good enough to eat all the grass before the fox catchs it
                    AddReward(5.0f);
                    Done();
                }
            }
        }
    }

    /// <summary>
    /// Resets Envirnment and Agent
    /// </summary>
    public override void AgentReset()
    {
        trainingManager.ResetArea();
    }
    
}
