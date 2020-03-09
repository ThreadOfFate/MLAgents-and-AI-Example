using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the grass
/// </summary>
public class GrassManager : MonoBehaviour
{
    public GameObject plane;
    public GameObject grass;

    private List<GameObject> grassList;

    public TrainingManager trainingManager;

    // Start is called before the first frame update
    void Start()
    {
        grassList = new List<GameObject>();
        SpawnMoreGrass(40);
    }

    /// <summary>
    /// Destroys all grass and clears the grassList
    /// </summary>
    public void ResetGrass()
    {
        for (int i = 0; i < grassList.Count; i++)
        {
            Destroy(grassList[i]);
        }
        grassList.Clear();
        SpawnMoreGrass(40);
    }

    /// <summary>
    /// Removes the given grass Gameobject from the grassList and destroys it
    /// Will send true back if the GrassList is empty to signal Training Envirnment needs reset
    /// </summary>
    /// <param name="grass">Grass to remove</param>
    /// <returns> If grassList is empty </returns>
    public bool DestroyGrass(GameObject grass)
    {
        grassList.Remove(grass);
        Destroy(grass);
        return (grassList.Count == 0);
    }


    /// <summary>
    /// Spawns more grass on the Land plane
    /// </summary>
    /// <param name="amountToSpawn"></param>
    public void SpawnMoreGrass(int amountToSpawn)
    {
        for(int i =0; i< amountToSpawn; i++)
        {
            GameObject temp = Instantiate(grass,this.transform);
            temp.transform.position = GetPointOnPlane();
            temp.transform.rotation.eulerAngles.Set(0,Random.Range(0f,360f), 0);
            temp.transform.localScale = Random.Range(0.8f, 1.2f) * Vector3.one;
            grassList.Add(temp);
        }
    }

    /// <summary>
    /// Returns a randomly generated point on the Land plane
    /// </summary>
    /// <returns> A randomly generated point on the Land plane </returns>
    public Vector3 GetPointOnPlane()
    {
        Vector3 point = plane.transform.position 
            + Random.Range(-0.9f, 0.9f) * new Vector3(plane.GetComponent<Collider>().bounds.extents.x, 0, 0)
            + Random.Range(-0.9f, 0.9f) * new Vector3(0,plane.GetComponent<Collider>().bounds.extents.y,0)
            + Random.Range(-0.9f, 0.9f) * new Vector3(0, 0, plane.GetComponent<Collider>().bounds.extents.z);

        return point;
    }

    /// <summary>
    /// Returns the nearest grass Transform to the World Vector given
    /// </summary>
    /// <param name="nPosition"> The World Vector you need to find the nearest grass Transform to </param>
    /// <returns> The Closest Grass Transform </returns>
    public Transform GetNearestGrass(Vector3 nPosition)
    {
        if (grassList != null && grassList.Count > 0)
        { 
            Transform closestGrass = grassList[0].transform;
            for (int i = 1; i < grassList.Count; i++)
            {
                if ((nPosition - grassList[i].transform.position).magnitude < (nPosition- closestGrass.position).magnitude)
                {
                        closestGrass = grassList[i].transform;
                }
            }
            return closestGrass;
        }
        else
        {
            return null;
        }
        

    }
}
