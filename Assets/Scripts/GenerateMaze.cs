using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public struct MazeCoordinates
{
    public int x;
    public int y;
}


public class GenerateMaze : MonoBehaviour
{
    private List<GameObject> mazeWalls;
    public GameObject mazeWall;
    public GameObject goal;

    public FoxAgentCamera agent;
    
    private List<MazeCoordinates> emptyCoordinates;
    private GameObject newGoal;

    private int range = 5;

    // Start is called before the first frame update
    void Start()
    {
        mazeWalls = new List<GameObject>();
        emptyCoordinates = new List<MazeCoordinates>();
        BuildMaze();
    }

    public void SetRange(int newRange)
    {
        range = newRange;
    }

    public void BuildMaze()
    {
        StartCoroutine(Build());

    }

    IEnumerator Build()
    {
        for (int i = 0; i < mazeWalls.Count; i++)
        {
            Destroy(mazeWalls[i]);
            
        }
        
        mazeWalls.Clear();
        emptyCoordinates.Clear();
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                if (1 == Random.Range(0, 8) ||
                    (i == 0) || (j == 0) || i == 99 || j == 99)
                {
                    GameObject newWall = Instantiate(mazeWall);
                    newWall.transform.position = transform.position + new Vector3(i - 50f, 0.5f, j - 50f);
                    newWall.transform.SetParent(transform);
                    mazeWalls.Add(newWall);
                }
                else
                {
                    MazeCoordinates mazeCoordinates;
                    mazeCoordinates.x = i;
                    mazeCoordinates.y = j;
                    emptyCoordinates.Add(mazeCoordinates);

                }
                
            }
        }
        

        MazeCoordinates startMazeCoor = emptyCoordinates[Random.Range(0, emptyCoordinates.Count)];

        int xDifference = Random.Range(-range, range + 1);
        int yDifference = Random.Range(-range, range + 1);
        MazeCoordinates goalMazeCoor;
        if (startMazeCoor.x + xDifference > 99)
        {
            goalMazeCoor.x = 99;
        }
        else if (startMazeCoor.x + xDifference < 1)
        {
            goalMazeCoor.x = 1;
        }
        else
        {
            goalMazeCoor.x = startMazeCoor.x + xDifference;
        }

        if (startMazeCoor.y + yDifference > 99)
        {
            goalMazeCoor.y = 99;
        }
        else if (startMazeCoor.y + yDifference < 1)
        {
            goalMazeCoor.y = 1;
        }
        else
        {
            goalMazeCoor.y = startMazeCoor.y + yDifference;
        }


        while (!emptyCoordinates.Contains(goalMazeCoor))
        {
            for (int i = goalMazeCoor.x; i < 100; i++)
            {
                for (int j = goalMazeCoor.y; j < 100; j++)
                {
                    MazeCoordinates temp;
                    temp.x = i;
                    temp.y = j;
                    if (emptyCoordinates.Contains(temp))
                    {
                        goalMazeCoor = temp;
                    }
                    
                }
            }
        }
        


        Destroy(newGoal);
        //MazeCoordinates goalMazeCoor = emptyCoordinates[Random.Range(0, emptyCoordinates.Count)];
        newGoal = Instantiate(goal);
        newGoal.transform.position = transform.position + new Vector3(goalMazeCoor.x - 50f, 0.5f, goalMazeCoor.y - 50f);
        newGoal.transform.SetParent(transform);


        agent.transform.position = transform.position + new Vector3(startMazeCoor.x - 50f, 0.5f, startMazeCoor.y - 50f);
        

    }
}
