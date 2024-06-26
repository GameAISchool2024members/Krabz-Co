using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class AimingComponent : MonoBehaviour
{
    [System.Serializable]
    public class Spline
    {
        [SerializeField]
        public float startPoint;

        [SerializeField]
        public Vector2 endPoint;

        [SerializeField]
        public float height;

        [SerializeField]
        public float rotation;
    }

    public class SplineData
    {
        public SplineData(Vector3 newStartPosition, Vector3 newMidPosition, Vector3 newEndPosition, float newInitialVelocity)
        {
            startPosition = newStartPosition;
            midPosition = newMidPosition;
            endPosition = newEndPosition;

            initialVelocity = newInitialVelocity;
        }

        public Vector3 startPosition;
        public Vector3 midPosition;
        public Vector3 endPosition;

        public float initialVelocity;
    }

    public int CurrentTilt
    {
        set
        {
            currentTilt = value;

            if (currentTilt < 0 || currentTilt >= splines.Count)
            {
                throw new Exception("Invalid Trajectory Index");
            }

            generatePreviewSpline(ballPreview.gameObject, 10);

            Vector3 currentRotation = cannonSprite.transform.localRotation.eulerAngles;
            cannonSprite.transform.localRotation = Quaternion.Euler(splines[currentTilt].rotation, currentRotation.y, currentRotation.z);
        }
    }

    static private int debugSplineMaxSegments = 10;

    [Header("Aiming")]
    [SerializeField]
    private List<Spline> splines;

    [SerializeField]
    [MinAttribute(0.0001f)]
    private float initialVelocity = 0.0001f;

    [SerializeField]
    private SpriteRenderer cannonSprite;

    [SerializeField]
    private MeshFilter ballPreview;

    private int currentTilt;

    private LineRenderer lineRenderer;

    public SplineData getSplineData()
    {
        if (currentTilt < 0 || currentTilt >= splines.Count)
        {
            throw new Exception("Invalid Trajectory Index");
        }

        Spline spline = splines[currentTilt];

        Vector3 startPosition = transform.position;
        startPosition.y += spline.startPoint;

        Vector3 endPosition = transform.position;
        endPosition.y += spline.endPoint.y;
        endPosition.z += spline.endPoint.x;

        Vector3 midPoint = (startPosition + endPosition) / 2;
        midPoint.y += spline.height;

        return new SplineData(startPosition, midPoint, endPosition, initialVelocity);
    }

    public void generatePreviewSpline(GameObject endPreview, int size)
    {
        if (currentTilt < 0 || currentTilt >= splines.Count)
        {
            throw new Exception("Invalid Trajectory Index");
        }

        Spline spline = splines[currentTilt];

        Vector3 startPosition = transform.position;
        startPosition.y += spline.startPoint;

        Vector3 endPosition = transform.position;
        endPosition.y += spline.endPoint.y;
        endPosition.z += spline.endPoint.x;

        Vector3 midPoint = (startPosition + endPosition) / 2;
        midPoint.y += spline.height;

        lineRenderer.positionCount = size + 1;

        for (int i = 0; i < size + 1; ++i)
        {
            Vector3 position = GetPoint(startPosition, midPoint, endPosition, (float)i / size);
            lineRenderer.SetPosition(i, position);
        }

        endPreview.transform.position = endPosition;
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < splines.Count; ++i)
        {
            Spline spline = splines[i];
            Gizmos.color = Color.Lerp(Color.yellow, Color.red, ((float)i) / splines.Count);

            Vector3 startPosition = transform.position;
            startPosition.y += spline.startPoint;
            Gizmos.DrawWireSphere(startPosition, 0.5f);

            Vector3 endPosition = transform.position;
            endPosition.y += spline.endPoint.y;
            endPosition.z += spline.endPoint.x;
            Gizmos.DrawWireSphere(endPosition, 0.5f);

            Vector3 midPoint = (startPosition + endPosition) / 2;
            midPoint.y += spline.height;
            Gizmos.DrawWireSphere(midPoint, 0.25f);

            for (int j = 0; j < debugSplineMaxSegments; ++j)
            {
                Gizmos.DrawLine(GetPoint(startPosition, midPoint, endPosition, j / (float)debugSplineMaxSegments), GetPoint(startPosition, midPoint, endPosition, (j + 1) / (float)debugSplineMaxSegments));
            }

        }
    }

    static public Vector3 GetPoint(Vector3 P0, Vector3 P1, Vector3 P2, float T)
    {
        return (1 - T) * (1 - T) * P0 + 2 * (1 - T) * T * P1 + T * T * P2;
    }
}
