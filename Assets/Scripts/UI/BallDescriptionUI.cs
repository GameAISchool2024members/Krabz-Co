using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Animator))]
public class BallDescriptionUI : MonoBehaviour
{

    private Animator animator;

    private Text ballDescription;

    private void Start()
    {
        animator = GetComponent<Animator>();
        ballDescription = GetComponentInChildren<Text>();
    }

    private void OnEnable()
    {
        EventManager.OnBallRated += BallRated;
    }

    private void OnDisable()
    {
        EventManager.OnBallRated -= BallRated;
    }

    private void BallRated(BallRater.Rate newRate)
    {
        ballDescription.text = newRate.description;
        animator.SetTrigger("BallDescriptionSet");
    }
}
