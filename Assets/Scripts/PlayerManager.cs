using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe.Unity;
public class PlayerManager : MonoBehaviour
{
    // Reference to the cube you want to move
    public Transform Player;
    public GameObject Cannon;

    // Locked Y and Z positions
    public float lockedY;
    public float lockedZ;

    // Reference to the PoseLandmarkListAnnotation component
    public PoseLandmarkListAnnotation poseLandmarkListAnnotation;

    [Header("Movement")]
    // Scaling factor to reduce movement range
    public float scalingFactor = 0.1f;
    public float Z_MidPointShoulders;
    public Vector3 positionOffset; // Offsets to adjust the shoulder position if needed

    private const int leftShoulderIndex = 11;
    private const int rightShoulderIndex = 12;
    private const int leftHandIndex = 15;
    private const int rightHandIndex = 16;

    //Baselines
    [Header("Baselines")]
    public float leftHandToShoulderDistance_baseline;
    public float rightHandToShoulderDistance_baseline;

    public float leftHandToShoulderDistance;
    public float rightHandToShoulderDistance;

    public float baseline_shoulder_midpoint;
    private bool baselineSet = false;

    [Header("Calculated Thresholds")]
    public float Threshold_Landmark_ShouldersZ;
    public float Threshold_ShouldersToHandsY;


    void Start()
    {
        // Initialize the locked Y and Z values if not set in the Inspector
        lockedY = Player.position.y;
        lockedZ = Player.position.z;
    }

    void Update()
    {
        // Ensure that pose landmarks are available
        if (poseLandmarkListAnnotation != null)
        {
            // Get the landmarks
            var leftShoulder = poseLandmarkListAnnotation[leftShoulderIndex];
            var rightShoulder = poseLandmarkListAnnotation[rightShoulderIndex];

            var leftHand = poseLandmarkListAnnotation[leftHandIndex];
            var rightHand = poseLandmarkListAnnotation[rightHandIndex];

            if (leftShoulder != null && rightShoulder != null && leftHand != null && rightHand != null)
            {
                // Calculate the midpoint between the shoulders
                Vector3 shoulderMidpoint = (leftShoulder.transform.position + rightShoulder.transform.position) / 2.0f;

                // Scale down the movement
                shoulderMidpoint *= scalingFactor;

                // Apply offset if needed
                shoulderMidpoint += positionOffset;

                // Update the X position to match the shoulder midpoint X position
                Vector3 currentPosition = Player.position;
                currentPosition.x = -shoulderMidpoint.x;
                Z_MidPointShoulders = shoulderMidpoint.z;
                // Keep the Y and Z positions locked
                currentPosition.y = lockedY;
                currentPosition.z = lockedZ;

                // Clamp the position within screen bounds
                currentPosition = ClampToScreenBounds(currentPosition);

                // Apply the new position to the cube
                Player.position = currentPosition;

                // Check for baseline setting
                if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.JoystickButton0))
                {
                    baseline_shoulder_midpoint = shoulderMidpoint.z;

                    baselineSet = true;
                    Debug.Log("Baseline set to: " + baseline_shoulder_midpoint);

                    leftHandToShoulderDistance_baseline = Vector3.Distance(leftHand.transform.position, leftShoulder.transform.position);
                    rightHandToShoulderDistance_baseline = Vector3.Distance(rightHand.transform.position, rightShoulder.transform.position);

                    if (baseline_shoulder_midpoint > 0)
                        Threshold_Landmark_ShouldersZ = shoulderMidpoint.z - baseline_shoulder_midpoint * 0.2f;
                    else
                        Threshold_Landmark_ShouldersZ = shoulderMidpoint.z + baseline_shoulder_midpoint * 0.2f;
                }

                // Check for crouch condition
                if (baselineSet && shoulderMidpoint.y < Threshold_Landmark_ShouldersZ && leftHandToShoulderDistance_baseline * .5 > leftHandToShoulderDistance
                    && rightHandToShoulderDistance_baseline * .5 > rightHandToShoulderDistance)
                {
                    Debug.Log("Threshold");
                    Cannon.transform.rotation = Quaternion.Euler(140, Cannon.transform.rotation.eulerAngles.y, Cannon.transform.rotation.eulerAngles.z);
                }
                else
                    Cannon.transform.rotation = Quaternion.Euler(90, Cannon.transform.rotation.eulerAngles.y, Cannon.transform.rotation.eulerAngles.z);

                // Calculate distances between hands and shoulders
                leftHandToShoulderDistance = Vector3.Distance(leftHand.transform.position, leftShoulder.transform.position);
                rightHandToShoulderDistance = Vector3.Distance(rightHand.transform.position, rightShoulder.transform.position);

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
