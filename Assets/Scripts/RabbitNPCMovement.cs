using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitNPCMovement : NPCMovement
{

    private bool isJumping = false; // If the rabbit is currently jumping
    private float currentLine = 0; // In which y line the rabbit is currently in

    public float jumpSpeed = 5.0f; // Force of the jump
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
