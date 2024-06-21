using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class AudioCapturing : MonoBehaviour
{
    private bool CapturingInProgress = false;
    public Text outputText; 
    private AudioSource audioSource;

    private const string url = "http://localhost:5000/transcribe";
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        EventManager.OnStartRecording += StartRecording;
    }

    void OnDisable()
    {
        EventManager.OnStartRecording -= StartRecording;
    }

    private void StartRecording()
    {
        StartCoroutine(CaptureAudio());
        CapturingInProgress = true;
    }
    
    private IEnumerator CaptureAudio()
    {
        // Start recording from the microphone
        audioSource.clip = Microphone.Start(null, false, 5, 44100);
        yield return new WaitForSeconds(5);
    }

    public void Update()
    {
        if (CapturingInProgress && !Microphone.IsRecording(null))
        {
            Microphone.End(null);

            string filePath = Application.dataPath + "/recording.wav";
            SavWav.Save(filePath, audioSource.clip);
            CapturingInProgress = false;
            EventManager.CompleteRecording(filePath);

            // Send the audio file to the server for transcription
            //StartCoroutine(SendAudioToServer(filePath));
        }
    }

    private IEnumerator SendAudioToServer(string filePath)
    {
        byte[] audioData = System.IO.File.ReadAllBytes(filePath);
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", audioData, "recording.wav", "audio/wav");

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                // Process the response from the server
                string jsonResponse = www.downloadHandler.text;
                TranscriptionResponse response = JsonUtility.FromJson<TranscriptionResponse>(jsonResponse);
                outputText.text = response.transcription;
            }
        }
    }

    [System.Serializable]
    private class TranscriptionResponse
    {
        public string transcription;
    }
}
