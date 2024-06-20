using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{

    // Total number of crabs and rabbits to spawn
    public int numCrabsSpawn;
    public int numRabbitsSpawn;

    public int xFloorSplits = 4;

    public List<float> zAxisRows;

    // Game object prefabs
    public GameObject crabPrefab;
    public GameObject rabbitPrefab;

    // Scene floor
    public GameObject floor;

    // Floor boundaries
    public float floorStartX = -3.3f;
    public float floorEndX = 3.5f;

    // Current crab and rabbits
    private List<GameObject> crabs;
    private List<GameObject> rabbits;


    // Start is called before the first frame update
    void Start()
    {
        crabs = new List<GameObject>();
        GenerateNPCs();
    }

    private void GenerateNPCs()
    {

        // Get floor boundaries

        float floorWidth = floorEndX - floorStartX;

        // Calculate the width of each segment
        float segmentWidth = floorWidth / 4.0f;

        for (int i = 0; i < xFloorSplits; i++)
        {
            // Calculate the start and end positions of the current segment
            float segmentStartX = floorStartX + (segmentWidth * i);
            float segmentEndX = segmentStartX + segmentWidth;

            // Loop rows
            for (int j = 0; j < zAxisRows.Count; j++)
            {
                // Generate crabs
                GenerateCrabs(zAxisRows[j], segmentStartX, segmentEndX);
            }
        }
    }

    private void GenerateCrabs(float z, float xstart, float xend)
    {
        // Spawn the NPC at the random position
        for (int j = 0; j < numCrabsSpawn / xFloorSplits / zAxisRows.Count; j++)
        {
            // Generate a random x position within the current segment
            float randomX = Random.Range(xstart, xend);

            Vector3 spawnPosition = new Vector3(randomX, 0.1f, z);
            GameObject newCrab = (GameObject)Instantiate(crabPrefab, spawnPosition, Quaternion.identity);

            //Debug.Log(newCrab.position);

            //// Set position y to 0.1
            //newCrab.transform.position = new Vector3(newCrab.transform.position.x, 0, newCrab.transform.position.z);

            // Set rotation of x to 90
            newCrab.transform.rotation = Quaternion.Euler(90, 0, 0);

            // Get CrabNPCMovement component
            CrabNPCMovement crabMovement = newCrab.GetComponent<CrabNPCMovement>();


            crabs.Add(newCrab);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
