using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagulNPCMovement : NPCMovement
{

    // Start is called before the first frame update
    void Start()
    {
        // Randomly set the direction of the seagull
        movingRight = Random.Range(0, 2) == 0;

        // If moving left, flip the sprite
        if (!movingRight)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
        ChangeMovementSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public override void ChangeDirection()
    {
        base.ChangeDirection();
        // Flip side
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
}
