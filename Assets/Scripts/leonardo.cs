using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using UnityEngine.Networking;


public class Leonardo : MonoBehaviour
{
    private string url = "http://localhost:8008/get_image";
    private string audioUrl = "http://localhost:8008/transcribe";
    // Start is called before the first frame update
    public void RequestImage(string prompt, IImageReceiver imageReceiver)
    {
        StartCoroutine(GenerateImage(prompt, imageReceiver));
    }

    public void RequestAudioDescription(string path, IAudioDescriptionReceiver descriptionReceiver)
    {
        StartCoroutine(GenerateDescription(path, descriptionReceiver));
    }

    public interface IImageReceiver
    {
        void SetImage(Sprite sprite);

    }

    public interface IAudioDescriptionReceiver
    {
        void SetDescription(string description);

    }

    public IEnumerator GenerateImage(string prompt, IImageReceiver imageReceiver)
    {
        string processed_prompt = "{\"prompt\": \"" + prompt + " trapped in a glass ball\"}";

        Debug.Log(processed_prompt);
        // Create a new UnityWebRequest for a POST request
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        // Set the request headers
        request.SetRequestHeader("Content-Type", "application/json");

        // Convert prompt to a byte array
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(processed_prompt);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        // Create a DownloadHandler to receive the response
        request.downloadHandler = new DownloadHandlerBuffer();

        // Send the request and wait for a response
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);

            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("file://"+request.downloadHandler.text.Replace("\"", "")))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                    imageReceiver.SetImage(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f)));
                }
            }
        }
    }


    public IEnumerator GenerateDescription(string path, IAudioDescriptionReceiver descriptionReceiver)
    {
        string processed_prompt = "{\"file\": \"" + path + "\"}";

        Debug.Log(processed_prompt);
        // Create a new UnityWebRequest for a POST request
        UnityWebRequest request = new UnityWebRequest(audioUrl, "POST");

        // Set the request headers
        request.SetRequestHeader("Content-Type", "application/json");

        // Convert prompt to a byte array
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(processed_prompt);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        // Create a DownloadHandler to receive the response
        request.downloadHandler = new DownloadHandlerBuffer();

        // Send the request and wait for a response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);

            descriptionReceiver.SetDescription(request.downloadHandler.text);
        }
    }
}
