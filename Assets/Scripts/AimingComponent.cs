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

    public class AttachableObject
    {
        public AttachableObject(GameObject obj, int index)
        {
            Transform = obj.transform;
            TrajectoryIndex = index;
        }

        public Transform Transform { get; set; } = null;

        

        public int TrajectoryIndex { get; set; } = 0;
    }

    [SerializeField]
    private List<Spline> splines;

    [SerializeField]
    [MinAttribute(0.0001f)]
    float initialVelocity = 0.0001f;

    protected AttachableObject attachedObject = null;

    private float trajectoryTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
    }

    public void AttachObject(AttachableObject newObject)
    {
        if(newObject.TrajectoryIndex < 0 || newObject.TrajectoryIndex >= splines.Count)
        {
            throw new Exception("Invalid Trajectory Index");
        }

        Vector3 position = transform.position;
        position.y += splines[newObject.TrajectoryIndex].startPoint;

        attachedObject = newObject;
        attachedObject.Transform.position = position;
        trajectoryTime = 0f;
        enabled = true;
    }

    void FixedUpdate()
    {
        trajectoryTime += Time.fixedDeltaTime * initialVelocity;

        Spline spline = splines[attachedObject.TrajectoryIndex];

        Vector3 startPosition = transform.position;
        startPosition.y += spline.startPoint;

        Vector3 endPosition = transform.position;
        endPosition.y += spline.endPoint.y;
        endPosition.z += spline.endPoint.x;

        Vector3 midPoint = (startPosition + endPosition) / 2;
        midPoint.y += spline.height;

        attachedObject.Transform.position = GetPoint(startPosition, midPoint, endPosition, trajectoryTime);

        if((attachedObject.Transform.position - endPosition).sqrMagnitude < 0.25f)
        {
            attachedObject.Transform.position = endPosition;
            attachedObject = null;
            enabled = false;
        }
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

    public Vector3 GetPoint(Vector3 P0, Vector3 P1, Vector3 P2, float T)
    {
        return (1 - T) * (1 - T) * P0 + 2 * (1 - T) * T * P1 + T * T * P2;
    }
}
