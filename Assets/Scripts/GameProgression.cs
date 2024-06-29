using UnityEngine;
using UnityEngine.UI;

public class GameProgression : MonoBehaviour, RequestProcessor.IAudioDescriptionReceiver
{
    public GamePhases GamePhase ;
    public CannonComponent cannon;
    public Text scoreText;

    private int score;

    void OnEnable()
    {
        EventManager.OnRecordingComplete += RecordingComplete;
        EventManager.OnImageGenerated += ImageGenerated;
        ChangeState(GamePhases.CannonBallChoosingInfo);
        EventManager.OnScorePoint += ChangeScore;
    }

    void OnDisable()
    {
        EventManager.OnRecordingComplete -= RecordingComplete;
        EventManager.OnImageGenerated -= ImageGenerated;
        EventManager.OnScorePoint -= ChangeScore;
    }

    void Update()
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
                    if(!GetComponent<RequestProcessor>().isFiring)
                    {
                        GetComponent<RequestProcessor>().RequestFire(cannon);
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
        GetComponent<RequestProcessor>().RequestImage(description, cannon);
    }

    private string description;
    
    private void RecordingComplete(string path)
    {
        ChangeState(GamePhases.CannonballGeneration);

        GetComponent<RequestProcessor>().RequestAudioDescription(path, this);
        //GetComponent<RequestProcessor>().RequestFire(cannon);
    }


    private void ImageGenerated()
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
