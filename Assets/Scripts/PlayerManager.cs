using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe.Unity;

public class PlayerManager : MonoBehaviour
{
    public bool CameraIsOn;
    // Reference to the cube you want to move
    public Transform Player;
    public GameObject cannon;

    private CannonComponent cannonComponent;
    private AimingComponent aimingComponent;

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
    public float movementSpeed = 5.0f;
    public int tiltStep = 1; // Step to increase/decrease the tilt value


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

    private int currentTiltValue = 1; // Initialize the tilt counter
    private float tiltCooldown = 0.2f; // Cooldown time to prevent rapid changes
    private float nextTiltChangeTime = 0f; // Next allowed time to change tilt



    void Start()
    {
        cannonComponent = cannon.GetComponent<CannonComponent>();
        aimingComponent = cannon.GetComponent<AimingComponent>();

        CameraIsOn = Calibration.Instance.CameraIsOn;

        // Initialize the locked Y and Z values if not set in the Inspector
        lockedY = Player.position.y;
        lockedZ = Player.position.z;

        baseline_shoulder_midpoint = Calibration.Instance.baseline_shoulder_midpoint;

        distanceThreshold = Calibration.Instance.distanceThreshold;
        baselineSet_MidShoulders = Calibration.Instance.baselineSet_MidShoulders;
        Debug.Log("Baseline set to: " + baseline_shoulder_midpoint);

        leftHandToShoulderDistance_baseline = Calibration.Instance.leftHandToShoulderDistance_baseline;
        rightHandToShoulderDistance_baseline = Calibration.Instance.rightHandToShoulderDistance_baseline;
        //DontDestroyOnLoad(gameObject);

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Joystick1Button2) || Input.GetKeyDown(KeyCode.Space))
        {
            cannonComponent.Fire();
        }
        if (CameraIsOn)
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



                    // Calculate the current distance from the baseline
                    if (baselineSet_MidShoulders)
                    {
                        //// Calculate distances between hands and shoulders
                        leftHandToShoulderDistance = Vector3.Distance(leftHand.transform.position, leftShoulder.transform.position);
                        rightHandToShoulderDistance = Vector3.Distance(rightHand.transform.position, rightShoulder.transform.position);

                        if (baseline_shoulder_midpoint - shoulderMidpoint.z > Threshold_Landmark_ShouldersZ &&
                            leftHandToShoulderDistance_baseline * .9 > leftHandToShoulderDistance && rightHandToShoulderDistance_baseline * .9 > rightHandToShoulderDistance)
                        {
                            // Logic to handle when the distance exceeds the threshold
                            Threshold_Landmark_MidShoulder = true;
                            aimingComponent.CurrentTilt = 0;
                        }
                        else if (baseline_shoulder_midpoint - shoulderMidpoint.z <= Threshold_Landmark_ShouldersZ &&
                            leftHandToShoulderDistance_baseline * .9 > leftHandToShoulderDistance && rightHandToShoulderDistance_baseline * .9 > rightHandToShoulderDistance)
                        {
                            Threshold_Landmark_MidShoulder = true;
                            aimingComponent.CurrentTilt = 2;
                        }
                        else
                        {
                            Threshold_Landmark_MidShoulder = false;
                            aimingComponent.CurrentTilt = 1;

                        }
                    }

                }
            }
        }
        else
        {
            HandleKeyboardMovement();
        }
        void HandleKeyboardMovement()
        {
            // Get the current position
            Vector3 currentPosition = Player.position;

            // Check for horizontal input (A/D or Left/Right arrow keys)
            float horizontalInput = Input.GetAxis("Horizontal");

            // Update the X position based on the horizontal input
            currentPosition.x += horizontalInput * movementSpeed * Time.deltaTime;

            // Keep the Y and Z positions locked
            currentPosition.y = lockedY;
            currentPosition.z = lockedZ;

            // Clamp the position within screen bounds
            currentPosition = ClampToScreenBounds(currentPosition);

            // Apply the new position to the player
            Player.position = currentPosition;

            // Check for vertical input (W/S or Up/Down arrow keys)
            float verticalInput = Input.GetAxis("Vertical");


            // Update the tilt value based on vertical input with cooldown to prevent rapid changes
            if (Time.time >= nextTiltChangeTime)
            {
                if (verticalInput > 0)
                {
                    currentTiltValue += tiltStep;
                    nextTiltChangeTime = Time.time + tiltCooldown;
                }
                else if (verticalInput < 0)
                {
                    currentTiltValue -= tiltStep;
                    nextTiltChangeTime = Time.time + tiltCooldown;
                }
            }

            // Clamp the tilt value between 0 and 2
            currentTiltValue = Mathf.Clamp(currentTiltValue, 0, 2);

            // Update the aiming component's tilt
            aimingComponent.CurrentTilt = currentTiltValue;
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
