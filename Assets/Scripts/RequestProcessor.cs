using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.Networking;


public class RequestProcessor : MonoBehaviour
{
    private static string imageUrl = "http://localhost:8008/get_image";
    private static string audioUrl = "http://localhost:8008/transcribe";
    private static string fireUrl = "http://localhost:8008/listen";

    public bool isFiring = false;

    // Start is called before the first frame update
    public void RequestImage(string prompt, IImageReceiver imageReceiver)
    {
        StartCoroutine(GenerateImage(prompt, imageReceiver));
    }

    public void RequestAudioDescription(string path, IAudioDescriptionReceiver descriptionReceiver)
    {
        StartCoroutine(GenerateDescription(path, descriptionReceiver));
    }

    public void RequestFire(IFireReceiver fireReceiver)
    {
        isFiring = true;
        StartCoroutine(GenerateFire(fireReceiver));
    }

    public interface IImageReceiver
    {
        void SetImage(Sprite sprite);

    }

    public interface IAudioDescriptionReceiver
    {
        void SetDescription(string description);

    }

    public interface IFireReceiver
    {
        void Fire();

    }

    protected IEnumerator GenerateImage(string prompt, IImageReceiver imageReceiver)
    {
       
        string processedPrompt = "{\"prompt\": \"" + prompt + " trapped in a glass ball\"}";

        UnityWebRequest request = CreateRequest(imageUrl, processedPrompt);

        // Send the request and wait for a response
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            imageReceiver.SetImage(null);
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


    protected IEnumerator GenerateDescription(string path, IAudioDescriptionReceiver descriptionReceiver)
    {
        string processedPrompt = "{\"file\": \"" + path + "\"}";

        UnityWebRequest request = CreateRequest(audioUrl, processedPrompt);

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

    protected IEnumerator GenerateFire(IFireReceiver fireReceiver)
    {
        while (isFiring)
        {
            string processedPrompt = "{}";

            UnityWebRequest request = CreateRequest(fireUrl, processedPrompt);

            // Send the request and wait for a response
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Debug.Log("Response: " + request.downloadHandler.text);

                fireReceiver.Fire();
            }
        }
    }

    protected UnityWebRequest CreateRequest(string url, string body)
    {
        Debug.Log("Body: " + body);
        // Create a new UnityWebRequest for a POST request
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        // Set the request headers
        request.SetRequestHeader("Content-Type", "application/json");

        // Convert prompt to a byte array
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        // Create a DownloadHandler to receive the response
        request.downloadHandler = new DownloadHandlerBuffer();

        return request;
    }
}
