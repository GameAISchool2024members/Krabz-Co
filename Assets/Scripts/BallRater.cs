using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LLMUnity;
    
public class BallRater : MonoBehaviour
{
    public LLM llm;
    private string model_output;
    private bool completed = false;

    void Start()
    {
        //RateCannonBall("turtle");
    }
    
    void HandleReply(string reply)
    {
        model_output = reply;
    }
    
    void ReplyCompleted()
    {
        completed = true;
        Debug.Log(model_output);
        //Debug.Log("Finished");
    }
    
    public void RateCannonBall(string cannonball)
    {
        //Debug.Log("Start");
        completed = false;
        var test = llm.Chat(cannonball, HandleReply,ReplyCompleted);
    }
    
    // get the current output of the LLM, if the LLM is still generating this will log a warning
    public string GetOutput()
    {
        if (!completed)
        {
            Debug.LogWarning("Called GetOutput while model was still running");
        }
        return model_output;
    }
}