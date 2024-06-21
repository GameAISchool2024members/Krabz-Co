using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void EventHandler();
    public delegate void RecordingCompleteHandler(string path);

    public static event EventHandler OnStartRecording;
    public static event EventHandler OnImageGenerated;
    public static event RecordingCompleteHandler OnRecordingComplete;

    public static void StartRecording()
    {
        Debug.Log("EventManager: Start Recording event triggered");
        OnStartRecording?.Invoke();
    }

    public static void ImageGenerated()
    {
        Debug.Log("EventManager: Image Generated event triggered");
        OnImageGenerated?.Invoke();
    }

    public static void CompleteRecording(string path)
    {
        Debug.Log("EventManager: Recording Complete event triggered");
        OnRecordingComplete?.Invoke(path);
    }
}
