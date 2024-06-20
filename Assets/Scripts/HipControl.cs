using UnityEngine;
using Mediapipe.Unity; // Add this using directive
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

    // Offsets to adjust the hip position if needed
    public Vector3 positionOffset;

    private const int leftHipIndex = 23;
    private const int rightHipIndex = 24;

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

            if (leftHip != null && rightHip != null)
            {
                // Use left hip for positioning (can be averaged with right hip if needed)
                Vector3 hipPosition = new Vector3(leftHip.transform.position.x, leftHip.transform.position.y, leftHip.transform.position.z);

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
}