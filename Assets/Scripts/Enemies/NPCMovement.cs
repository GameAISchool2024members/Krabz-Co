using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    // For movespeed and constraints
    public float baseSpeed = 2.0f; // Base speed of the NPC
    protected float speed = 2.0f; // Movement speed of the NPC
    public float moveSpeedRange = 1f; // Range of speed the NPC moves
    public float constrainerStart = -10f;
    public float constrainerEnd = 10f;

    // For change direction
    protected bool movingRight = true;
    protected bool changedDirection = false;
    protected float lastDirectionChange;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private protected void Move()
    {
        // If nearby a contrainst change direction
        if ((transform.position.x + 0.3 >= constrainerEnd || transform.position.x - 0.3 <= constrainerStart) && !changedDirection)
        {
            ChangeDirection();
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

    protected void ChangeMovementSpeed()
    {
        speed = Random.Range(baseSpeed - moveSpeedRange, baseSpeed + moveSpeedRange);
    }

    public virtual void ChangeDirection()
    {
        movingRight = !movingRight;
        ChangeMovementSpeed();
        lastDirectionChange = Time.time;
        changedDirection = true;
    }

    // Get score multiplier based on current speed?
    public float GetScoreMultiplier()
    {
        return speed / baseSpeed;
    }

}
