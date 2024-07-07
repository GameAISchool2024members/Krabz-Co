using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LLMUnity;

public class BallRater : MonoBehaviour
{
    public class Rate
    {
        public Rate(float newMultiplier, string newDescription)
        {
            multiplier = newMultiplier;
            description = newDescription;
        }

        public float multiplier;

        public string description;
    }

    public LLM llm;
    private string modelOutput;

    private void OnEnable()
    {
        EventManager.OnAudioDescriptionProcessed += RateCannonBall;
    }

    private void OnDisable()
    {
        EventManager.OnAudioDescriptionProcessed -= RateCannonBall;
    }

    void HandleReply(string reply)
    {
        modelOutput = reply;
    }
    
    void ReplyCompleted()
    {
        modelOutput = modelOutput.Trim('\"');
        string[] processedOutput = modelOutput.Split("--");

        if(processedOutput.Length == 2)
        {
            float multiplier = 1 + (float.Parse(processedOutput[0]) / 10);
            EventManager.CompleteBallRating(new Rate(multiplier, processedOutput[1]));
        }
    }
    
    public void RateCannonBall(string cannonball)
    {
        var test = llm.Chat(cannonball, HandleReply, ReplyCompleted);
    }
}