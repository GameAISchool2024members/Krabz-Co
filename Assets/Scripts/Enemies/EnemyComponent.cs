using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider))]
public class EnemyComponent : MonoBehaviour
{
    private Animator animator;
    private NPCMovement movement;

    private void Start()
    {
        movement = GetComponent<NPCMovement>();
        animator = GetComponent<Animator>();
    }


    private void OnTriggerEnter(Collider other)
    {
        movement.enabled = false;
        animator.SetTrigger("IsDead");
    }

}
