using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

public class VideoPlayerChecker : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject Ziggy;
    public GameObject Robert;
    public GameObject Pod;
    public GameObject Egglet;
    public GameObject Audio;
    public GameObject AmbientAudio;

    public PlayableDirector timeline;

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoPlayEnded;
        }
    }

    private void OnVideoPlayEnded(VideoPlayer vp)
    {
        gameObject.SetActive(false);
        Ziggy.SetActive(true);
        Robert.SetActive(true);
        Pod.SetActive(true);
        Egglet.SetActive(true);
        Audio.SetActive(true);
        AmbientAudio.SetActive(true);

        timeline.enabled = true;

        videoPlayer.loopPointReached -= OnVideoPlayEnded;
    }
}
