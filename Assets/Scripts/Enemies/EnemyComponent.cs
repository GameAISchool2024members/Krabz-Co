using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Animator))]
public class EnemyComponent : MonoBehaviour
{
    public int Score
    {
        get
        {
            return score;
        }
    }

    private Animator animator;
    private NPCMovement movement;

    [MinAttribute(1)]
    [SerializeField]
    private int score = 10;

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
