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
        [MinAttribute(0.0001f)]
        public float scaleSpeed;

        [SerializeField]
        public float startPoint;

        [SerializeField]
        public Vector2 endPoint;

        [SerializeField]
        public float height;
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

    [SerializeField]
    private List<Spline> splines;

    [SerializeField]
    [MinAttribute(0.0001f)]
    float initialVelocity = 0.0001f;


    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
    }

    public SplineData getSplineData(int trajectoryIndex)
    {
        if (trajectoryIndex < 0 || trajectoryIndex >= splines.Count)
        {
            throw new Exception("Invalid Trajectory Index");
        }

        Spline spline = splines[trajectoryIndex];

        Vector3 startPosition = transform.position;
        startPosition.y += spline.startPoint;

        Vector3 endPosition = transform.position;
        endPosition.y += spline.endPoint.y;
        endPosition.z += spline.endPoint.x;

        Vector3 midPoint = (startPosition + endPosition) / 2;
        midPoint.y += spline.height;

        return new SplineData(startPosition, midPoint, endPosition, initialVelocity);
    }

    void OnDrawGizmosSelected()
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

            for (int j = 0; j < 10; ++j)
            {
                Gizmos.DrawLine(GetPoint(startPosition, midPoint, endPosition, j / 10f), GetPoint(startPosition, midPoint, endPosition, (j + 1) / 10f));
            }

        }
    }

    static public Vector3 GetPoint(Vector3 P0, Vector3 P1, Vector3 P2, float T)
    {
        return (1 - T) * (1 - T) * P0 + 2 * (1 - T) * T * P1 + T * T * P2;
    }
}
