using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    // Total number of crabs and rabbits to spawn
    public int numCrabsSpawnLine;
    public int numSeagullsSpawnLine;

    public int xFloorSplits = 4;

    public List<float> zAxisRows;

    // Game object prefabs
    public GameObject crabPrefab;
    public GameObject seagullsPrefab;

    // Scene floor
    public GameObject floor;

    // Floor boundaries
    public float floorStartX = -3.3f;
    public float floorEndX = 3.5f;

    // Current crabs and rabbits
    private List<GameObject> crabs;
    private List<GameObject> seagulls;

    // Start is called before the first frame update
    void Start()
    {
        crabs = new List<GameObject>();
        seagulls = new List<GameObject>();
        GenerateNPCs();
    }

    private void GenerateNPCs()
    {

        // Loop through z axis rows
        for (int i = 0; i < zAxisRows.Count; i++)
        {
            // Generate crabs in the current row and segment
            GenerateNPCsInRow(crabPrefab, numCrabsSpawnLine, zAxisRows[i], floorStartX, floorEndX, crabs);

            // Generate rabbits in the current row and segment
            GenerateNPCsInRow(seagullsPrefab, numSeagullsSpawnLine, zAxisRows[i], floorStartX, floorEndX, seagulls, true);
        }
    }

    private void GenerateNPCsInRow(GameObject prefab, int numToSpawn, float z, float xstart, float xend, List<GameObject> npcList, bool isSeagull = false)
    {

        // Split the x axis into segments
        float segmentWidth = (xend - xstart) / xFloorSplits;

        // Create a dictionary to store segment min and max values
        Dictionary<int, float[]> segmentBounds = new Dictionary<int, float[]>();

        // Loop through the number of splits
        for (int i = 0; i < xFloorSplits; i++)
        {
            // Calculate the start and end of the segment
            float segmentStart = xstart + (i * segmentWidth);
            float segmentEnd = segmentStart + segmentWidth;

            // Add the segment to the dictionary
            segmentBounds.Add(i, new float[] { segmentStart, segmentEnd });
        }


        for (int j = 0; j < numToSpawn; j++)
        {
            // Get a random segment
            int randomSegment = Random.Range(0, xFloorSplits);

            // Generate a random x position within the current segment
            float randomX = Random.Range(segmentBounds[randomSegment][0], segmentBounds[randomSegment][1]);

            // Spawn the NPC at the random position
            Vector3 spawnPosition = new Vector3(randomX, 0.1f, z);
            GameObject newNPC = Instantiate(prefab, spawnPosition, Quaternion.identity);

            // Set rotation of x to 90
            newNPC.transform.rotation = Quaternion.Euler(35, 0, 0);

            // If it's a seaguel, set the z-axis rows
            if (isSeagull)
            {
                SeagulNPCMovement seagullMovement = newNPC.GetComponent<SeagulNPCMovement>();
                seagullMovement.zAxisRows = zAxisRows;
                seagullMovement.currentZIndex = zAxisRows.IndexOf(z);
            }

            // Add the new NPC to the corresponding list
            npcList.Add(newNPC);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
