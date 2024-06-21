using System.Linq;
using UnityEngine;

public class GameProgression : MonoBehaviour
{
    public GamePhases GamePhase ;
    private GameObject gameLoadScreen;

    public void Start()
    {
        GamePhase = GamePhases.GameLoading;
        gameLoadScreen = (Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[]).FirstOrDefault(plane => plane.name.Equals("GameLoadScreen"));
        gameLoadScreen.SetActive(true);
    }

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
            case GamePhases.GameLoading:
                if (Time.time > 1f)
                {
                    gameLoadScreen.SetActive(false);
                    GamePhase = GamePhases.CannonBallChoosingInfo;
                }

                break;
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
