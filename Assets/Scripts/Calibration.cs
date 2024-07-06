using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe.Unity;
using UnityEngine.SceneManagement;

public class Calibration : MonoBehaviour
{
    public static Calibration Instance;


    public bool CameraIsOn;
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

    private void Start()
    {
        Instance = this;
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
                
                Z_MidPointShoulders = shoulderMidpoint.z;
                

                // Check for baseline setting
                if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.JoystickButton0))
                {
                    baseline_shoulder_midpoint = shoulderMidpoint.z;
                    
                    distanceThreshold = baselineDistance * 0.9f;
                    baselineSet_MidShoulders = true;
                    Debug.Log("Baseline set to: " + baseline_shoulder_midpoint);

                    leftHandToShoulderDistance_baseline = Vector3.Distance(leftHand.transform.position, leftShoulder.transform.position);
                    rightHandToShoulderDistance_baseline = Vector3.Distance(rightHand.transform.position, rightShoulder.transform.position);

                    CameraIsOn = true;
                    DontDestroyOnLoad(gameObject);
                    SceneManager.LoadScene("Game");

                }
                if (Input.GetKeyDown(KeyCode.N))
                {
                    CameraIsOn = false;
                    DontDestroyOnLoad(gameObject);
                    SceneManager.LoadScene("Game");
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