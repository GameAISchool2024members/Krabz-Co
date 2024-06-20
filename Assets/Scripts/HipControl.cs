using UnityEngine;
using Mediapipe.Unity; // Add this using directive
using System.Collections.Generic;

public class HipControl : MonoBehaviour
{
    // Reference to the cube you want to move
    public Transform cube;

    // Locked Y and Z positions
    public float lockedY;
    public float lockedZ;

    // Reference to the PoseLandmarkListAnnotation component
    public PoseLandmarkListAnnotation poseLandmarkListAnnotation;

    // Scaling factor to reduce movement range
    public float scalingFactor = 0.1f;
    public float Y_Landmark;
    public float Threshold_Landmark;
    
    // Offsets to adjust the hip position if needed
    public Vector3 positionOffset;

    private const int leftHipIndex = 23;
    private const int rightHipIndex = 24;


    private const int leftShoulderIndex = 11;
    private const int rightShoulderIndex = 12;
    // Baseline y position of the left hip
    public float baselineY;
    private bool baselineSet = false;

    // List of cube prefabs for random instantiation
    public List<GameObject> cubePrefabs;

    // Bounds for random cube position
    public Vector3 cubeSpawnMin;
    public Vector3 cubeSpawnMax;

    void Start()
    {
        // Initialize the locked Y and Z values if not set in the Inspector
        lockedY = cube.position.y;
        lockedZ = cube.position.z;
    }

    void Update()
    {
        // Ensure that pose landmarks are available
        if (poseLandmarkListAnnotation != null)
        {
            // Get the landmarks
            var leftHip = poseLandmarkListAnnotation[leftHipIndex];
            var rightHip = poseLandmarkListAnnotation[rightHipIndex];

            var leftShoulder = poseLandmarkListAnnotation[leftShoulderIndex];
            var rightShoulder = poseLandmarkListAnnotation[rightShoulderIndex];
            if (leftHip != null && rightHip != null)
            {
                // Use left hip for positioning (can be averaged with right hip if needed)
                Vector3 hipPosition = new Vector3(leftShoulder.transform.position.x, leftShoulder.transform.position.y, leftShoulder.transform.position.z);
                Y_Landmark = leftShoulder.transform.position.y;

                // Scale down the movement
                hipPosition *= scalingFactor;

                // Apply offset if needed
                hipPosition += positionOffset;

                // Update the X position to match the hip's X position
                Vector3 currentPosition = cube.position;
                currentPosition.x = hipPosition.x;

                // Keep the Y and Z positions locked
                currentPosition.y = lockedY;
                currentPosition.z = lockedZ;

                // Clamp the position within screen bounds
                currentPosition = ClampToScreenBounds(currentPosition);

                // Apply the new position to the cube
                cube.position = currentPosition;

                // Check for baseline setting
                if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.JoystickButton0))
                {
                    baselineY = leftShoulder.transform.position.y;
                    baselineSet = true;
                    Debug.Log("Baseline set to: " + baselineY);
                    if(baselineY>0)
                        Threshold_Landmark = leftShoulder.transform.position.y - baselineY * 0.8f * 10;
                    else
                        Threshold_Landmark = leftShoulder.transform.position.y + baselineY * 0.8f * 10;
                }

                // Check for crouch condition
                if (baselineSet && leftShoulder.transform.position.y < Threshold_Landmark)
                {
                    SpawnRandomCube();
                }
            }
        }
    }

    // Method to clamp the position within screen bounds
    private Vector3 ClampToScreenBounds(Vector3 position)
    {
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            Vector3 screenPoint = mainCamera.WorldToViewportPoint(position);
            screenPoint.x = Mathf.Clamp(screenPoint.x, 0.0f, 1.0f);
            screenPoint.y = Mathf.Clamp(screenPoint.y, 0.0f, 1.0f);

            position = mainCamera.ViewportToWorldPoint(screenPoint);
        }

        return position;
    }

    // Method to spawn a random cube at a random position within specified bounds
    private void SpawnRandomCube()
    {
        Debug.Log("Threshold");
        if (cubePrefabs.Count > 0)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(cubeSpawnMin.x, cubeSpawnMax.x),
                Random.Range(cubeSpawnMin.y, cubeSpawnMax.y),
                Random.Range(cubeSpawnMin.z, cubeSpawnMax.z)
            );

            int randomIndex = Random.Range(0, cubePrefabs.Count);
            Instantiate(cubePrefabs[randomIndex], randomPosition, Quaternion.identity);
        }
    }
}