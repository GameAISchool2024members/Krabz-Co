using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabNPCMovement : NPCMovement
{

    // Start is called before the first frame update
    void Start()
    {
        ChangeMovementSpeed();
        // Randomly set the direction of the crab
        movingRight = Random.Range(0, 2) == 0;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public override void ChangeDirection()
    {
        base.ChangeDirection();
    }

}

