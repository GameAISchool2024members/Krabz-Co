using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void EventHandler();
    public delegate void TextEventHandler(string text);
    public delegate void ScorePointHandler(int points);
    public delegate void ImageGenerationHandler(Sprite sprite);

    public static event EventHandler OnStartRecording;
    public static event ImageGenerationHandler OnImageGenerated;
    public static event TextEventHandler OnRecordingComplete;
    public static event TextEventHandler OnAudioDescriptionProcessed;
    public static event EventHandler OnFire;
    public static event ScorePointHandler OnScorePoint;

    public static void StartRecording()
    {
        Debug.Log("EventManager: Start Recording event triggered");
        OnStartRecording?.Invoke();
    }

    public static void ImageGenerated(Sprite sprite)
    {
        Debug.Log("EventManager: Image Generated event triggered");
        OnImageGenerated?.Invoke(sprite);
    }

    public static void CompleteRecording(string path)
    {
        Debug.Log("EventManager: Recording Complete event triggered");
        OnRecordingComplete?.Invoke(path);
    }

    public static void AudioDescriptionProcess(string audioDescription)
    {
        Debug.Log("EventManager: Audio Description Set event triggered");
        OnAudioDescriptionProcessed?.Invoke(audioDescription);
    }

    public static void ScorePoint(int points)
    {
        Debug.Log("EventManager: Score Point event triggered");
        OnScorePoint?.Invoke(points);
    }

    public static void Fire()
    {
        Debug.Log("EventManager: Fire event triggered");
        OnFire?.Invoke();
    }
}
