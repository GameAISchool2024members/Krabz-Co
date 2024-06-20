using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public float baseSpeed = 2.0f; // Base speed of the NPC
    protected float speed = 2.0f; // Movement speed of the NPC
    public float moveSpeedRange = 1f; // Range of speed the NPC moves

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void ChangeMovementSpeed()
    {
        speed = Random.Range(baseSpeed - moveSpeedRange, baseSpeed + moveSpeedRange);
    }

}
