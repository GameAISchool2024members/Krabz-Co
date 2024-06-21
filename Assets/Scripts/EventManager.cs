using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void AudioEventHandler();
    public delegate void RecordingCompleteHandler(string path);

    public static event AudioEventHandler OnStartRecording;
    public static event RecordingCompleteHandler OnRecordingComplete;

    public static void StartRecording()
    {
        Debug.Log("EventManager: Start Recording event triggered");
        OnStartRecording?.Invoke();
    }

    public static void CompleteRecording(string path)
    {
        Debug.Log("EventManager: Recording Complete event triggered");
        OnRecordingComplete?.Invoke(path);
    }
}
