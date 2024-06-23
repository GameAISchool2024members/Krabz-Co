using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider))]
public class EnemyComponent : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Object.Destroy(gameObject);
    }

}
