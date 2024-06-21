using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using UnityEngine.Networking;


public class leonardo : MonoBehaviour
{
    private string url = "http://localhost:8008/get_image";
    // Start is called before the first frame update
    public void RequestImage(string prompt, IImageReceiver imageReceiver)
    {
        StartCoroutine(GenerateImage(prompt, imageReceiver));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public interface IImageReceiver
    {
        void SetImage(Sprite sprite);

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

            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("file:///home/bogdan/Documents/Projekti/Krabz-Co/" + request.downloadHandler.text.Replace("\"", "")))
            {
                Debug.Log("file:///home/bogdan/Documents/Projekti/Krabz-Co/" + request.downloadHandler.text.Replace("\"", ""));
                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    var texture = DownloadHandlerTexture.GetContent(uwr);
                    Debug.Log("Text: "+texture.width+ " "+texture.height);
                    imageReceiver.SetImage(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero));
                }
            }

            // You can handle the response here (e.g., parse JSON, download an image, etc.)
        }
    }

}
