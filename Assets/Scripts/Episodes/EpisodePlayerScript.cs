using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EpisodePlayerScript : MonoBehaviour
{
    public VideoPlayer vPlayer;

    public VideoClip loadedClip;

    public GameEvent startVideo;
    public GameEvent stopVideo;

    private void Awake()
    {
        if (vPlayer == null)
        {
            vPlayer = GetComponent<VideoPlayer>();
        }
    }

    private void Start()
    {
        vPlayer.loopPointReached += OnVideoEnded;
    }

    private void OnVideoEnded(VideoPlayer vp)
    {
        stopVideo.Raise();
        vPlayer.loopPointReached -= OnVideoEnded;

    }
}
