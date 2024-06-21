using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagulNPCMovement : NPCMovement
{
    public List<float> zAxisRows;
    private bool movingRight = true;
    private bool movingUp = true;
    private bool changedDirection = false;
    private float lastDirectionChange;

    public int currentZIndex = 0;
    private float targetZ;

    public float zMovementSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (zAxisRows.Count > 0)
        {

            // Randomly set the direction of the crab
            movingRight = Random.Range(0, 2) == 0;
            // If moving right, flip the sprite
            if (movingRight)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }

            //currentZIndex = Random.Range(0, zAxisRows.Count);
            targetZ = zAxisRows[currentZIndex];
            transform.position = new Vector3(transform.position.x, transform.position.y, zAxisRows[currentZIndex]);
            ChangeMovementSpeed();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        // Handle x-axis movement
        if ((transform.position.x + 0.3f >= constrainerEnd || transform.position.x - 0.3f <= constrainerStart) && !changedDirection)
        {
            movingRight = !movingRight;
            ChangeMovementSpeed();
            lastDirectionChange = Time.time;
            changedDirection = true;

            // Flip side
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }

        if (!movingRight)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }

        // Reset direction change flag after 1 second
        if (Time.time - lastDirectionChange >= 1)
        {
            changedDirection = false;
        }

        // Handle z-axis movement
        transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.MoveTowards(transform.position.z, targetZ, zMovementSpeed * Time.deltaTime));

        if (Mathf.Abs(transform.position.z - targetZ) < 0.01f)
        {
            // Choose new z direction
            if (currentZIndex == 0)
            {
                movingUp = true;
            }
            else if (currentZIndex == zAxisRows.Count - 1)
            {
                movingUp = false;
            }
            else
            {
                movingUp = Random.value > 0.5f;
            }

            if (movingUp)
            {
                currentZIndex = Mathf.Min(currentZIndex + 1, zAxisRows.Count - 1);
            }
            else
            {
                currentZIndex = Mathf.Max(currentZIndex - 1, 0);
            }

            targetZ = zAxisRows[currentZIndex];
        }
    }

    void ChangeMovementSpeed()
    {
        // Set random speed for movement within a reasonable range
        speed = Random.Range(1.0f, 3.0f);
    }
}
