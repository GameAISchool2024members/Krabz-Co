using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void AudioEventHandler();

    public static event AudioEventHandler OnStartRecording;
    public static event AudioEventHandler OnRecordingComplete;

    public static void StartRecording()
    {
        Debug.Log("EventManager: Start Recording event triggered");
        OnStartRecording?.Invoke();
    }

    public static void CompleteRecording()
    {
        Debug.Log("EventManager: Recording Complete event triggered");
        OnRecordingComplete?.Invoke();
    }
}
