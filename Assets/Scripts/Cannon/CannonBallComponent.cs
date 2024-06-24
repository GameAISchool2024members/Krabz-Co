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
    public GameObject Explosion;
    // Start is called before the first frame update
    void Awake()
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
        NPCManager.Instance.ExplosionOfNPC(gameObject.transform);
        // Start the coroutine to instantiate the explosion after a delay


    }
   
    public void OnTriggerEnter(Collider other)
    {
        // If hit a game object with EnemyComponent invoke score point event
        if (other.gameObject.GetComponent<EnemyComponent>())
        {
            EventManager.ScorePoint(10);
        }


        DestroyCannonBall();
    }
}
