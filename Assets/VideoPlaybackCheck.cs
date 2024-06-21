using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoPlaybackCheck : MonoBehaviour
{
    VideoPlayer video;

        void Awake()
        {
            video = GetComponent<VideoPlayer>();
            video.Play();
            video.loopPointReached += CheckOver;
        }


        void CheckOver(VideoPlayer vp)
        {
            SceneManager.LoadScene("MainMenu");
        }
}
