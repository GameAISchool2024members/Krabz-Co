using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabNPCMovement : NPCMovement
{
    private bool movingRight = true;

    private bool changedDirection = false;
    private float lastDirectionChange;

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

    void Move()
    {

        // If nearby a contrainst change direction
        if ((transform.position.x + 0.3 >= constrainerEnd || transform.position.x - 0.3 <= constrainerStart) && !changedDirection)
        {
            movingRight = !movingRight;
            ChangeMovementSpeed();
            lastDirectionChange = Time.time;
            changedDirection = true;
        }

        if (movingRight)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }

        // If 1 second has passed since the last direction change
        if (Time.time - lastDirectionChange >= 1)
        {
            changedDirection = false;
        }
    }

}

