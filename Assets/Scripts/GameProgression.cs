using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RequestProcessor))]
public class GameProgression : MonoBehaviour
{
    public GamePhases GamePhase ;
    public CannonComponent cannon;
    public Text scoreText;

    private int score;

    private RequestProcessor processor;

    private string description;

    private BallRater.Rate rate;

    private void Start()
    {
        processor = GetComponent<RequestProcessor>();
        ChangeState(GamePhases.CannonBallChoosingInfo);
    }

    private void OnEnable()
    {
        EventManager.OnRecordingComplete += RecordingComplete;
        EventManager.OnImageGenerated += ImageGenerated;
        EventManager.OnScorePoint += ChangeScore;
        EventManager.OnAudioDescriptionProcessed += SetDescription;
        EventManager.OnBallRated += BallRated;
    }

    private void OnDisable()
    {
        EventManager.OnRecordingComplete -= RecordingComplete;
        EventManager.OnImageGenerated -= ImageGenerated;
        EventManager.OnScorePoint -= ChangeScore;
        EventManager.OnAudioDescriptionProcessed -= SetDescription;
        EventManager.OnBallRated -= BallRated;

    }

    private void Update()
    {
        StateLogic();
    }

    private void StateLogic()
    {
        switch (GamePhase)
        {
            case GamePhases.GameLoading:
               
                break;
            case GamePhases.CannonBallChoosingInfo:
            {
                if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Joystick1Button0))
                {
                    ChangeState(GamePhases.CannonballSpeaking);
                    EventManager.StartRecording();
                }
                break;
            }
        }
    }

    private void ChangeState(GamePhases newPhase)
    {
        ExitState(GamePhase);
        EnterState(newPhase);
    }
    
    private void EnterState(GamePhases phase)
    {
        GamePhase = phase;
        switch (phase)
        {
            case GamePhases.CannonBallChoosingInfo:
            {
                if(!processor.isFiring)
                {
                    processor.RequestFire();
                }
                //GetComponent<RequestProcessor>().isFiring = false;
                break;
            }
        }
    }
    private void ExitState(GamePhases phase)
    {
        switch (phase)
        {
            
        } 
    }

    public void SetDescription(string newDescription)
    {
        description = newDescription;
        processor.RequestImage(description);
    }

    private void BallRated(BallRater.Rate newRate)
    {
        rate = newRate;
    }
    
    private void RecordingComplete(string path)
    {
        ChangeState(GamePhases.CannonballGeneration);

        processor.RequestAudioDescription(path);
    }


    private void ImageGenerated(Sprite sprite)
    {
        ChangeState(GamePhases.CannonBallChoosingInfo);
    }

    private void ChangeScore(int points, bool isSpecial)
    {
        score += (isSpecial && rate != null) ? (int)Math.Round(points * rate.multiplier, 0) : points;
        scoreText.text = score.ToString();
    }
}
