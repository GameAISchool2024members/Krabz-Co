using UnityEngine;

public class GameProgression : MonoBehaviour
{
    public GamePhases GamePhase ;


    void OnEnable()
    {
        EventManager.OnRecordingComplete += RecordingComplete;
    }

    void OnDisable()
    {
        EventManager.OnStartRecording -= RecordingComplete;
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
                if (Input.GetKeyDown(KeyCode.A))
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
    
    
    private void RecordingComplete()
    {
        GamePhase = GamePhases.CannonballGeneration;
    }

}
