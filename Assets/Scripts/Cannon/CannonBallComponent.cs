using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class CannonBallComponent : MonoBehaviour
{
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

    [SerializeField]
    private GameObject Explosion;

    private SpriteRenderer ballRenderer;

    private AimingComponent.SplineData splineData = null;

    private float trajectoryTime = 0f;

    private bool isSpecial = false;

    public void SetBallSprite(Sprite ballSprite)
    {
        if(ballSprite)
        {
            isSpecial = true;
            ballRenderer.sprite = ballSprite;
        }
        
        gameObject.transform.localScale *= ballSprite ? 1.5f : 0.8f;
    }

    private void Awake()
    {
        ballRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
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
        EnemyComponent enemyComponent = other.gameObject.GetComponent<EnemyComponent>();
        if (enemyComponent)
        {
            EventManager.ScorePoint(enemyComponent.Score, isSpecial);
        }


        DestroyCannonBall();
    }
}
