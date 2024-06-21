using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class CannonBallComponent : MonoBehaviour
{

    public SpriteRenderer BallRenderer
    {
        get
        {
            return ballRenderer;
        }
    }

    public AimingComponent.SplineData SplineData
    {
        set
        {
            splineData = value;

            if(splineData != null)
            {
                trajectoryTime = 0f;
                transform.position = splineData.startPosition;
            }
        }
    }

    private SpriteRenderer ballRenderer;

    private AimingComponent.SplineData splineData = null;

    private float trajectoryTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        ballRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {

        if (splineData == null)
        {
            return;
        }

        trajectoryTime += Time.fixedDeltaTime * splineData.initialVelocity;


        transform.position = AimingComponent.GetPoint(splineData.startPosition, splineData.midPosition, splineData.endPosition, trajectoryTime);
        if ((transform.position - splineData.endPosition).sqrMagnitude < 0.25f)
        {
            transform.position = splineData.endPosition;
            gameObject.SendMessage("DestroyCannonBall");
            splineData = null;
        }
    }

    public void DestroyCannonBall()
    {
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        DestroyCannonBall();
    }
}
