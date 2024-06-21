using DefaultNamespace;
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
        switch (GamePhase)
        {
            case GamePhases.CannonBallChoosingInfo:
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    GamePhase = GamePhases.CannonballSpeaking;
                    EventManager.StartRecording();
                }

                break;
            }
        }
    }

    private void RecordingComplete()
    {
        GamePhase = GamePhases.CannonballGeneration;
    }

}
