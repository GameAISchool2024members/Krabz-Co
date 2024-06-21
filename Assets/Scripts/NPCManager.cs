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
    public GameObject Explosion;
    public static NPCManager Instance;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        crabs = new List<GameObject>();
        seagulls = new List<GameObject>();
        GenerateNPCs();
    }

    private void GenerateNPCs()
    {

        // Loop through z axis rows
        for (int i = 0; i < zAxisRows.Count; i++)
        {
            // If we are not in the last row, generate crabs
            if (i != zAxisRows.Count - 1)
            {
                // Generate crabs in the current row and segment
                GenerateNPCsInRow(crabPrefab, numCrabsSpawnLine, zAxisRows[i], floorStartX, floorEndX, crabs);
            }
            else
            {
                GenerateNPCsInRow(seagullsPrefab, numSeagullsSpawnLine, zAxisRows[i], floorStartX, floorEndX, seagulls);
            }
        }
    }

    private void GenerateNPCsInRow(GameObject prefab, int numToSpawn, float z, float xstart, float xend, List<GameObject> npcList)
    {
        // Split the x axis into segments
        float segmentWidth = (xend - xstart) / xFloorSplits;

        // Create a list to store segment min and max values
        List<float[]> segmentBounds = new List<float[]>();

        // Loop through the number of splits
        for (int i = 0; i < xFloorSplits; i++)
        {
            // Calculate the start and end of the segment
            float segmentStart = xstart + (i * segmentWidth);
            float segmentEnd = segmentStart + segmentWidth;

            // Add the segment to the list
            segmentBounds.Add(new float[] { segmentStart, segmentEnd });
        }

        // Evenly distribute the NPCs across segments
        int segmentIndex = 0;
        for (int j = 0; j < numToSpawn; j++)
        {
            // Generate a random x position within the current segment
            float randomX = Random.Range(segmentBounds[segmentIndex][0], segmentBounds[segmentIndex][1]);

            // Spawn the NPC at the random position
            Vector3 spawnPosition = new Vector3(randomX, 0.1f, z);
            GameObject newNPC = Instantiate(prefab, spawnPosition, Quaternion.identity);

            // Set rotation of x to 35
            newNPC.transform.rotation = Quaternion.Euler(35, 0, 0);

            // Add the new NPC to the corresponding list
            npcList.Add(newNPC);

            // Move to the next segment in a round-robin fashion
            segmentIndex = (segmentIndex + 1) % xFloorSplits;
        }
    }

    public void ExplosionOfNPC(Transform transform)
    {
        GameObject Explosion_=Instantiate(Explosion, transform.position, Quaternion.identity);
        StartCoroutine(HandleExplosion(Explosion_));
    }

    private IEnumerator HandleExplosion(GameObject gameObject)
    {
       
        // Wait for another 1 second
        yield return new WaitForSeconds(2f);

        // Destroy the explosion
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
