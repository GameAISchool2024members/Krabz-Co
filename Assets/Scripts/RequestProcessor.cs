using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;


using Debug = UnityEngine.Debug;

public class RequestProcessor : MonoBehaviour
{
    private static string imageUrl = "http://localhost:8008/get_image";
    private static string audioUrl = "http://localhost:8008/transcribe";
    private static string fireUrl = "http://localhost:8008/listen";

    public bool isFiring = false;

    private Process PythonServer;

    // Start is called before the first frame update
    private void Start()
    {
        // Automatically starts python server
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "python";
        start.Arguments = Application.dataPath + "/Leonardo/server.py --api";
        start.UseShellExecute = false;
        start.RedirectStandardOutput = false;
        
        PythonServer = Process.Start(start);

        if(isFiring)
        {
            RequestFire();
        }
    }

    private void OnApplicationQuit()
    {
        PythonServer.Kill();
        PythonServer.Close();
        PythonServer.Dispose();
    }

    public void RequestImage(string prompt)
    {
        StartCoroutine(GenerateImage(prompt));
    }

    public void RequestAudioDescription(string path)
    {
        StartCoroutine(GenerateDescription(path));
    }

    public void RequestFire()
    {
        isFiring = true;

        if(PythonServer != null)
        {
            StartCoroutine(GenerateFire());
        }
    }

    protected IEnumerator GenerateImage(string prompt)
    {
       
        string processedPrompt = "{\"prompt\": \"" + prompt + " trapped in a glass ball\"}";

        UnityWebRequest request = CreateRequest(imageUrl, processedPrompt);

        // Send the request and wait for a response
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            EventManager.ImageGenerated(null);
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
                    EventManager.ImageGenerated(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f)));
                }
            }
        }
    }


    protected IEnumerator GenerateDescription(string path)
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
            EventManager.AudioDescriptionProcess(request.downloadHandler.text);
        }
    }

    protected IEnumerator GenerateFire()
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
                EventManager.Fire();
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
