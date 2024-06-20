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

    private SpriteRenderer ballRenderer;

    // Start is called before the first frame update
    void Start()
    {
        ballRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
