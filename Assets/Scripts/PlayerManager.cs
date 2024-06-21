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
    public bool baselineSet_MidShoulders = false;

    [Header("Calculated Thresholds")]
    public float Threshold_Landmark_ShouldersZ;
    public bool Threshold_Landmark_MidShoulder = false;

    public float distanceThreshold;
    public float baselineDistance;
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
                currentPosition.x = shoulderMidpoint.x;
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
                    baselineDistance = Mathf.Abs(baseline_shoulder_midpoint - currentPosition.z);
                    distanceThreshold = baselineDistance * 0.9f;
                    baselineSet_MidShoulders = true;
                    Debug.Log("Baseline set to: " + baseline_shoulder_midpoint);

                    leftHandToShoulderDistance_baseline = Vector3.Distance(leftHand.transform.position, leftShoulder.transform.position);
                    rightHandToShoulderDistance_baseline = Vector3.Distance(rightHand.transform.position, rightShoulder.transform.position);
                    
                }


                // Calculate the current distance from the baseline
                if (baselineSet_MidShoulders)
                {
                    //// Calculate distances between hands and shoulders
                    leftHandToShoulderDistance = Vector3.Distance(leftHand.transform.position, leftShoulder.transform.position);
                    rightHandToShoulderDistance = Vector3.Distance(rightHand.transform.position, rightShoulder.transform.position);
   
                    if (baseline_shoulder_midpoint -shoulderMidpoint.z > Threshold_Landmark_ShouldersZ && 
                        leftHandToShoulderDistance_baseline * .9 > leftHandToShoulderDistance && rightHandToShoulderDistance_baseline * .9 > rightHandToShoulderDistance)
                    {

                            // Logic to handle when the distance exceeds the threshold
                            Debug.Log("Shoulder movement exceeds threshold");
                            Threshold_Landmark_MidShoulder = true;
                            Cannon.transform.localRotation = Quaternion.Euler(50, Cannon.transform.localRotation.eulerAngles.y,
                                Cannon.transform.localRotation.eulerAngles.z);
                    }
                    else
                    {
                        Threshold_Landmark_MidShoulder = false;
                        Cannon.transform.localRotation = Quaternion.Euler(0, Cannon.transform.localRotation.eulerAngles.y, 
                            Cannon.transform.localRotation.eulerAngles.z);
                        
                    }
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

}
