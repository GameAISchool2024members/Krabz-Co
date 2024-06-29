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
    }

    private void OnDisable()
    {
        EventManager.OnRecordingComplete -= RecordingComplete;
        EventManager.OnImageGenerated -= ImageGenerated;
        EventManager.OnScorePoint -= ChangeScore;
        EventManager.OnAudioDescriptionProcessed -= SetDescription;
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
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Joystick1Button0))
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

    private string description;
    
    private void RecordingComplete(string path)
    {
        ChangeState(GamePhases.CannonballGeneration);

        processor.RequestAudioDescription(path);
    }


    private void ImageGenerated(Sprite sprite)
    {
        ChangeState(GamePhases.CannonBallChoosingInfo);
    }

    private void ChangeScore(int points)
    {
        score += points;
        Debug.Log("Score: " + score);
        scoreText.text = score.ToString();
    }
}
