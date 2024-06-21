using UnityEngine;

public class GameProgression : MonoBehaviour, Leonardo.IAudioDescriptionReceiver
{
    public GamePhases GamePhase ;
    public CannonComponent cannon;

    void OnEnable()
    {
        EventManager.OnRecordingComplete += RecordingComplete;
        EventManager.OnImageGenerated += ImageGenerated;
    }

    void OnDisable()
    {
        EventManager.OnRecordingComplete -= RecordingComplete;
        EventManager.OnImageGenerated -= ImageGenerated;
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
        GetComponent<Leonardo>().RequestImage(description, cannon);
    }

    private string description;
    
    private void RecordingComplete(string path)
    {
        GamePhase = GamePhases.CannonballGeneration;
        GetComponent<Leonardo>().RequestAudioDescription(path, this);
        
    }


    private void ImageGenerated()
    {
        ChangeState(GamePhases.CannonBallChoosingInfo);
    }
}
